using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;

namespace Cotorra.Core
{
    public class VacationCardManager
    {
        public async Task<List<Vacation>> BrekeOrNotAsync(Vacation vacation, Guid identityWorkID, Guid instanceID,
                 HolidayPaymentConfiguration holidayPaymentConfiguration)
        {
            if (holidayPaymentConfiguration == HolidayPaymentConfiguration.PayVacationInitially)
            {
                var brker = new VacationCardBreakerPayInitial();
                return await brker.BreakAsync(vacation, identityWorkID, instanceID, holidayPaymentConfiguration);
            }
            else
            {
                var brker = new VacationCardBreakerWhenTook();
                return await brker.BreakAsync(vacation, identityWorkID, instanceID, holidayPaymentConfiguration);
            }
        }
    }
    public interface IVacationCardBreaker
    {
        Task<List<Vacation>> BreakAsync(Vacation vacation, Guid identityWorkID, Guid instanceID, HolidayPaymentConfiguration config);
    }

    public class VacationCardBreakerPayInitial : IVacationCardBreaker
    {
        public async Task<List<Vacation>> BreakAsync(Vacation vacation, Guid identityWorkID, Guid instanceID, HolidayPaymentConfiguration config)
        {
            return await Task.FromResult(new List<Vacation>() { vacation });
        }
    }

    public class VacationCardBreakerWhenTook : IVacationCardBreaker
    {
        public async Task<List<Vacation>> BreakAsync(Vacation vacation, Guid identityWorkID, Guid instanceID, HolidayPaymentConfiguration config)
        {
            var initialVacationDate = vacation.InitialDate;
            var finalVacationDate = vacation.FinalDate;
            List<Vacation> result = new List<Vacation>();
            MiddlewareManager<PeriodType> middlewareManager = new MiddlewareManager<PeriodType>(new BaseRecordManager<PeriodType>(), new PeriodTypeValidator());
            MiddlewareManager<Employee> employeeMiddlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());

            var employee = (await employeeMiddlewareManager.FindByExpressionAsync(p => p.ID == vacation.EmployeeID &&
         p.Active == true && p.InstanceID == instanceID, identityWorkID, new string[] { "PeriodType", "PeriodType.Periods", "PeriodType.Periods.PeriodDetails" })).FirstOrDefault();

            var periodTypes = employee.PeriodType;

            if (periodTypes != null)
            {
                var allDetails = periodTypes.Periods.SelectMany(x => x.PeriodDetails).ToList();

                var periodDetails = allDetails.Where(p =>
                                (initialVacationDate >= p.InitialDate &&
                                initialVacationDate <= p.FinalDate) ||
                                (finalVacationDate >= p.FinalDate &&
                                 p.InitialDate >= initialVacationDate) ||
                                finalVacationDate >= p.InitialDate &&
                                finalVacationDate <= p.FinalDate).OrderBy("InitialDate").ToList();

                var totalCards = periodDetails.Count();

                if (totalCards == 1)
                {
                    result.Add(vacation);
                    return result;
                }

                int index = 0;
                periodDetails.ForEach(periodDetail =>
                {
                    Vacation clonedVacation = (Vacation)vacation.Clone();
                    clonedVacation.ID = Guid.NewGuid();
                    if (index == 0)
                    {

                        if (periodDetail.InitialDate != initialVacationDate)
                        {
                            clonedVacation.InitialDate = initialVacationDate;
                        }
                        else
                        {
                            clonedVacation.InitialDate = periodDetail.InitialDate;
                        }
                        clonedVacation.FinalDate = periodDetail.FinalDate;

                    }

                    else if (index == totalCards - 1)
                    {
                        if (periodDetail.FinalDate != finalVacationDate)
                        {
                            clonedVacation.FinalDate = finalVacationDate;
                        }
                        else
                        {
                            clonedVacation.FinalDate = periodDetail.FinalDate;
                        }
                        clonedVacation.InitialDate = periodDetail.InitialDate;
                    }
                    else
                    {
                        clonedVacation.InitialDate = periodDetail.InitialDate;
                        clonedVacation.FinalDate = periodDetail.FinalDate;
                    }

                    index++;
                    result.Add(clonedVacation);
                    SetDaysOffAndVacationDays(clonedVacation, vacation, config);
                });


                return result;
            }

            return result;
        }

        private void SetDaysOffAndVacationDays(Vacation clonedVacation, Vacation vacation, HolidayPaymentConfiguration holidayPaymentConfiguration)
        {
            int daysoffInPeriod = 0;
            if (vacation.VacationDaysOff != null && vacation.VacationDaysOff.Any())
            {
                var inPeriod = vacation.VacationDaysOff.Where(x => x.Date >= clonedVacation.InitialDate && x.Date <= clonedVacation.FinalDate).ToList();
                daysoffInPeriod = inPeriod.ToList().Count;
                if (inPeriod.Any())
                {
                    inPeriod.ForEach(item => { item.VacationID = clonedVacation.ID; item.Vacation = null; });
                    clonedVacation.VacationDaysOff = inPeriod;
                }
                else
                {
                    clonedVacation.VacationDaysOff = null;
                }
            }

            clonedVacation.VacationsDays = (decimal)(clonedVacation.FinalDate - clonedVacation.InitialDate).TotalDays + 1 - daysoffInPeriod;
            if (holidayPaymentConfiguration == HolidayPaymentConfiguration.PayVacationsAndBonusInPeriod)
            {
                clonedVacation.VacationsBonusDays = clonedVacation.VacationsDays * clonedVacation.VacationsBonusPercentage / 100;
            }
        }



    }

}
