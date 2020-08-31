using MoreLinq;
using Cotorra.Core.Managers.Calculation;
using Cotorra.Core.Utils;
using Cotorra.Schema;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core.Validator
{
    public class VacationInhabilityIncidentHelperValidator
    {
        static List<IValidationRule> createRules = new List<IValidationRule>();

        static VacationInhabilityIncidentHelperValidator()
        {

        }

        private async Task<HolidayPaymentConfiguration> GetPeriodTypesHolidayConfiguration(Guid companyID, Guid instanceID)
        {
            var _clientPeriodType = new MiddlewareManager<PeriodType>(new BaseRecordManager<PeriodType>(), new PeriodTypeValidator());
            var periodTypes = await _clientPeriodType.GetAllAsync(companyID, instanceID);
            return periodTypes.FirstOrDefault().HolidayPremiumPaymentType;
        }

        public void ValidateInDate(List<Vacation> vacations, List<Inhability> inhabilities, List<Incident> incidents)
        {
            var dateTimeUtil = new DateTimeUtil();

            vacations.AsParallel().ForEach(vacation =>
            {
                DateTime vacationInitialDate = vacation.InitialDate;
                DateTime vacationFinalDate = vacation.FinalDate;

                var inhabilitiesFromEmployee = inhabilities.Where(x => x.EmployeeID == vacation.EmployeeID);

                inhabilitiesFromEmployee.ForEach(inhability =>
                {
                    int inclusiveDays = dateTimeUtil.InclusiveDays(vacationInitialDate, vacationFinalDate, inhability.InitialDate, inhability.FinalDate);
                    if (inclusiveDays > 0)
                    {
                        int daysInDaysOff = GetDaysInDaysOff(vacation.VacationDaysOff, inhability.InitialDate, inhability.FinalDate);
                        if (daysInDaysOff < inclusiveDays)
                        {
                            throw new CotorraException(1024, "1024", "No es posible registrar vacaciones e incidencias al colaborador en el mismo día.");
                        }
                    }

                    var incidentsFromEmployee = incidents.Where(x => x.EmployeeID == inhability.EmployeeID);

                    incidentsFromEmployee.AsParallel().ForEach(incident =>
                    {

                        int inclusiveDays = dateTimeUtil.InclusiveDays(inhability.InitialDate, inhability.FinalDate, incident.Date, incident.Date);
                        if (inclusiveDays > 0)
                        {

                            throw new CotorraException(1026, "1026", "No es posible registrar vacaciones e incidencias al colaborador en el mismo día.");

                        }
                    });

                });

                var incidentsFromEmployee = incidents.Where(x => x.EmployeeID == vacation.EmployeeID);

                incidentsFromEmployee.AsParallel().ForEach(incident =>
                {
                    int inclusiveDays = dateTimeUtil.InclusiveDays(vacationInitialDate, vacationFinalDate, incident.Date, incident.Date);
                    if (inclusiveDays > 0)
                    {
                        int daysInDaysOff = GetDaysInDaysOff(vacation.VacationDaysOff, incident.Date, incident.Date);
                        if (daysInDaysOff < inclusiveDays)
                        {
                            throw new CotorraException(1025, "1025", "No es posible registrar vacaciones e incidencias al colaborador en el mismo día.");
                        }
                    }
                });


            });

            inhabilities.ForEach(inhability =>
            {
                
                var incidentsFromEmployee = incidents.Where(x => x.EmployeeID == inhability.EmployeeID);

                incidentsFromEmployee.AsParallel().ForEach(incident =>
                {

                    int inclusiveDays = dateTimeUtil.InclusiveDays(inhability.InitialDate, inhability.FinalDate, incident.Date, incident.Date);
                    if (inclusiveDays > 0)
                    {

                        throw new CotorraException(1026, "1026", "No es posible registrar vacaciones e incidencias al colaborador en el mismo día.");

                    }
                });

            });
        }

        private int GetDaysInDaysOff(List<VacationDaysOff> daysOff, DateTime initial, DateTime final)
        {
            int counter = 0;
            DateTime index = new DateTime(initial.Year, initial.Month, initial.Day);
            while (index <= final)
            {
                if (daysOff.Any(x => x.Date == index))
                {
                    counter++;
                }
                index = index.AddDays(1);
            }

            return counter;
        }




    }
}
