using MoreLinq;
using Cotorra.Core.Managers.Calculation;
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
    public class VacationValidator : IValidator<Vacation>
    {
        static List<IValidationRule> createRules = new List<IValidationRule>();
        public IMiddlewareManager<Vacation> MiddlewareManager { get; set; }
        static VacationValidator()
        {
            createRules.Add(new GuidRule("EmployeeID", "El empleado es obligatorio", 16001));
            createRules.Add(new DecimalRule("Break_Seventh_Days", "Número de días de descanso y/o séptimos", 0, 9999, 16002));
        }

        private async Task<HolidayPaymentConfiguration> GetPeriodTypesHolidayConfiguration(Guid companyID, Guid instanceID)
        {
            var _clientPeriodType = new MiddlewareManager<PeriodType>(new BaseRecordManager<PeriodType>(), new PeriodTypeValidator());
            var periodTypes = await _clientPeriodType.GetAllAsync(companyID, instanceID);
            return periodTypes.FirstOrDefault().HolidayPremiumPaymentType;
        }

        public void CreateVacationsConcepts(Vacation vacation)
        {
            var instanceID = vacation.InstanceID;
            var company = vacation.company;
            var user = vacation.user;
            var employeeID = vacation.EmployeeID;
            var initialDate = vacation.InitialDate;
            var finalDate = vacation.FinalDate;
            var holidayEnum = GetPeriodTypesHolidayConfiguration(company, instanceID).Result;

            //aqui se paga prima vacacional
            decimal? primaDays = null;
            if (holidayEnum == HolidayPaymentConfiguration.PayVacationsAndBonusInPeriod)
            {
                primaDays = vacation.VacationsBonusDays;
            }
            var days = vacation.VacationsDays;

            using (var connection = new SqlConnection(
                    ConnectionManager.ConfigConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "CreateVacationsConcepts";
                    command.Parameters.AddWithValue("@InstanceId", instanceID);
                    command.Parameters.AddWithValue("@company", company);
                    command.Parameters.AddWithValue("@user", user);
                    command.Parameters.AddWithValue("@EmployeeID", employeeID);
                    command.Parameters.AddWithValue("@InitialDate", initialDate);
                    command.Parameters.AddWithValue("@FinalDate", finalDate);
                    command.Parameters.AddWithValue("@Days", days);
                    command.Parameters.AddWithValue("@PrimaDays", primaDays);
                    //Execute SP de unapply salary adjustment
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AfterCreate(List<Vacation> lstObjectsToValidate)
        {
            //all good
            //Crear el detalle con el concepto 19
            //[CreateVacationsConcepts]
            var instanceID = lstObjectsToValidate.FirstOrDefault().InstanceID;
            var company = lstObjectsToValidate.FirstOrDefault().company;
            var user = lstObjectsToValidate.FirstOrDefault().user;
            foreach (var vacation in lstObjectsToValidate)
            {
                CreateVacationsConcepts(vacation);
            }
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {

        }

        public void AfterUpdate(List<Vacation> lstObjectsToValidate)
        {

        }

        public void BeforeCreate(List<Vacation> lstObjectsToValidate)
        {
            var validator = new RuleValidator<Vacation>();
            validator.ValidateRules(lstObjectsToValidate, createRules);

            var employeesIDs = lstObjectsToValidate.Select(x => x.EmployeeID).ToList();

            var middlewareManager = new MiddlewareManager<Vacation>(new BaseRecordManager<Vacation>(), this);

            var entities = middlewareManager.FindByExpression(p => employeesIDs.Contains(p.EmployeeID), Guid.Empty, new string[] { });

            lstObjectsToValidate.AsParallel().ForEach(toSave =>
            {
                var badEntities = entities.Where(x => (toSave.InitialDate >= x.InitialDate && toSave.FinalDate <= x.FinalDate) ||
                            (toSave.InitialDate >= x.InitialDate && toSave.InitialDate <= x.FinalDate) ||
                            (toSave.FinalDate <= x.FinalDate && toSave.FinalDate >= x.InitialDate));

                if (badEntities.Any())
                {
                    throw new CotorraException(601, "601", "Ya se han registrado vacaciones en esas fechas para el empleado", null);
                }
            });
            entities.AsParallel().ForEach(toSave =>
              {
                  var badEntities = lstObjectsToValidate.Where(x => (toSave.InitialDate >= x.InitialDate && toSave.FinalDate <= x.FinalDate) ||
                              (toSave.InitialDate >= x.InitialDate && toSave.InitialDate <= x.FinalDate) ||
                              (toSave.FinalDate <= x.FinalDate && toSave.FinalDate >= x.InitialDate));

                  if (badEntities.Any())
                  {
                      throw new CotorraException(601, "601", "Ya se han registrado vacaciones en esas fechas para el empleado", null);
                  }
              });

            ValidateInDate(lstObjectsToValidate);
          
        }

        private void ValidateInDate(List<Vacation> lstObjectsToValidate)
        {
            VacationInhabilityIncidentHelperValidator helperValidator = new VacationInhabilityIncidentHelperValidator();
            var employeesIDs = lstObjectsToValidate.Select(x => x.EmployeeID);

            var inhabilityManager = new MiddlewareManager<Inhability>(new BaseRecordManager<Inhability>(), new InhabilityValidator());
            var incidentManager = new MiddlewareManager<Incident>(new BaseRecordManager<Incident>(), new IncidentValidator());

            var orderListInitialDate = lstObjectsToValidate.Select(x => x.InitialDate).OrderBy(p => p).FirstOrDefault();
           

            var inhabilities = inhabilityManager.FindByExpression(x => (x.InitialDate >= orderListInitialDate || x.InitialDate.AddDays(x.AuthorizedDays - 1) >= orderListInitialDate) && employeesIDs.Contains(x.EmployeeID), Guid.Empty, new string[] { });


            var incidents = incidentManager.FindByExpression(x => x.Date >= orderListInitialDate && employeesIDs.Contains(x.EmployeeID), Guid.Empty, new string[] { });

            helperValidator.ValidateInDate(lstObjectsToValidate, inhabilities, incidents);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            if (lstObjectsToValidate.Any())
            {
                var middlewareManager = new MiddlewareManager<Vacation>(new BaseRecordManager<Vacation>(), this);
                var vacationDaysOffManager = new MiddlewareManager<VacationDaysOff>(new BaseRecordManager<VacationDaysOff>(), new VacationDaysOffValidator());
                var entities = middlewareManager.FindByExpression(p => lstObjectsToValidate.Contains(p.ID), Guid.Empty, new string[] { "VacationDaysOff" });

                var daysOff = entities.SelectMany(p => p.VacationDaysOff).Select(p => p.ID);

                if (daysOff.Any())
                {
                    vacationDaysOffManager.Delete(daysOff.ToList(), Guid.Empty);

                }
            }
        }

        public void BeforeUpdate(List<Vacation> lstObjectsToValidate)
        {
            var validator = new RuleValidator<Vacation>();
            validator.ValidateRules(lstObjectsToValidate, createRules);

            ValidateInDate(lstObjectsToValidate);
        }


    }
}
