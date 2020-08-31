using Microsoft.AspNetCore.Mvc;
using Cotorra.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cotorra.Schema;
using Cotorra.Client;
using MoreLinq;
using System.Globalization;

namespace Cotorra.Web.Controllers
{
    public class VacationController : Controller
    {
        private readonly Client<Vacation> client;
        private readonly Client<VacationDaysOff> vacationDaysOffClient;
        private readonly Client<PeriodDetail> pdClient;
        private readonly VacationClient vacationClientUtils;
        private readonly Client<PeriodType> _clientPeriodType;
        private readonly CalculationClient _calculationClient;

        public VacationController()
        {
            SessionModel.Initialize();
            var clientAdapter = ClientConfiguration.GetAdapterFromConfig();
            client = new Client<Vacation>(SessionModel.AuthorizationHeader, clientAdapter);
            pdClient = new Client<PeriodDetail>(SessionModel.AuthorizationHeader, clientAdapter);
            vacationClientUtils = new VacationClient(SessionModel.AuthorizationHeader, clientAdapter);
            vacationDaysOffClient = new Client<VacationDaysOff>(SessionModel.AuthorizationHeader, clientAdapter);
            _clientPeriodType = new Client<PeriodType>(SessionModel.AuthorizationHeader, clientAdapter);
            _calculationClient = new CalculationClient(SessionModel.AuthorizationHeader, clientAdapter);
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get(Guid employeeID, Guid PeriodDetailID)
        {
            var periodDetail = (await pdClient.FindAsync(p => p.ID == PeriodDetailID, SessionModel.CompanyID)).FirstOrDefault();
            var initialDate = periodDetail.InitialDate;

            var findResult = await client.FindAsync(x => x.InstanceID == SessionModel.InstanceID
                    && x.EmployeeID == employeeID && x.InitialDate >= initialDate, SessionModel.CompanyID, new String[] { "VacationDaysOff" });

            var vacations = findResult
              .Select(x => new
              {
                  x.ID,
                  x.VacationsCaptureType,
                  x.InitialDate,
                  x.FinalDate,
                  x.PaymentDate,
                  x.Break_Seventh_Days,
                  x.VacationsDays,
                  x.VacationsBonusDays,
                  x.VacationsBonusPercentage,                  
                  DaysOff = GetDaysOff(x),
                  x.EmployeeID
              })
              .OrderBy(x => x.InitialDate)
              .ToList();

            return Json(vacations);
        }

        private async Task recalculateAsync(Guid employeeID)
        {
            //recalcular debido a modificación en incremento de salario
            var ids = new List<Guid> { employeeID };
            await _calculationClient.CalculationFireAndForgetByEmployeesAsync(new CalculationFireAndForgetByEmployeeParams()
            {
                EmployeeIds = ids,
                IdentityWorkID = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                UserID = SessionModel.IdentityID
            });
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(Vacation vacation)
        {
            var vacations = new List<Vacation>();
            Guid priorID = vacation.ID;
            var configurationHoliday = (HolidayPaymentConfiguration)await GetPeriodTypesHolidayConfiguration(SessionModel.CompanyID,
              SessionModel.InstanceID);

            //Common
            vacation.Active = true;
            vacation.company = SessionModel.CompanyID;
            vacation.InstanceID = SessionModel.InstanceID;
            vacation.CreationDate = DateTime.Now;
            vacation.StatusID = 1;
            vacation.user = SessionModel.IdentityID;
            vacation.Timestamp = DateTime.Now;
            vacation.EmployeeID = vacation.Employee.ID;
            vacation.PaymentDate = vacation.InitialDate;
            vacation.Employee = null;

            var vacationDaysOff = GetVacationDaysOff(vacation);
            vacation.VacationDaysOff = vacationDaysOff;

            // vacations.Add(vacation);
            vacations = await BreakOrNotVacation(vacation, SessionModel.CompanyID, SessionModel.InstanceID, configurationHoliday);
            if (priorID == Guid.Empty)
            {

                await client.CreateAsync(vacations, SessionModel.CompanyID); 
            }
            else
            {
                await client.DeleteAsync(new List<Guid>() { priorID }, SessionModel.CompanyID);
                await client.CreateAsync(vacations, SessionModel.CompanyID); 
            } 

            //Recalculate
            await recalculateAsync(vacation.EmployeeID);

            return Json(vacations.Count > 1);
        }

        private async Task SaveVacationDaysOff(List<Vacation> vacations, List<VacationDaysOff> vacationDaysOff)
        {
            if (vacationDaysOff.Any())
            {
                vacationDaysOff.ForEach(item => item.Vacation = null);
                var ids = vacations.Select(x => x.ID);
                var found = (await vacationDaysOffClient.FindAsync(x => ids.Contains( x.ID )&&
                            x.InstanceID == SessionModel.InstanceID, SessionModel.CompanyID)).Select(p => p.ID);
                if (found.Any())
                {
                    await vacationDaysOffClient.DeleteAsync(found.ToList(), SessionModel.CompanyID);
                }
                await vacationDaysOffClient.CreateAsync(vacationDaysOff, SessionModel.CompanyID);
            }
        }

        private async Task<List<Vacation>> BreakOrNotVacation(Vacation vacationCard, Guid identityWorkID,
            Guid instanceID, HolidayPaymentConfiguration configuration)
        {

            var res = await vacationClientUtils.BrekeOrNotAsync(vacationCard, identityWorkID, instanceID,
                 configuration);
            return res;
        }

        private List<VacationDaysOff> GetVacationDaysOff(Vacation vacation)
        {
            if (string.IsNullOrEmpty(vacation.DaysOff))
            {
                return new List<VacationDaysOff>();
            }
            var splited = vacation.DaysOff.Split(',');
            string format = "dd/MM/yyyy";

            List<VacationDaysOff> result = new List<VacationDaysOff>();

            splited.ForEach(day =>
            {
                DateTime dateTime;
                if (DateTime.TryParseExact(day, format, CultureInfo.InvariantCulture,
                         DateTimeStyles.None, out dateTime))
                {
                    result.Add(new VacationDaysOff()
                    {
                        Active = true,
                        company = vacation.company,
                        Date = dateTime,
                        DeleteDate = null,
                        ID = Guid.NewGuid(),
                        InstanceID = vacation.InstanceID,
                        VacationID = vacation.ID
                    });
                }
            });
            
            return result;
        }

        private string GetDaysOff(Vacation vacation)
        {
            if (vacation.VacationDaysOff == null || vacation.VacationDaysOff.Count == 0)
            {
                return string.Empty;
            }
            return String.Join(',', vacation.VacationDaysOff.Select(x => x.Date.ToString("dd/MM/yyyy")));
        }


        private async Task<int> GetPeriodTypesHolidayConfiguration(Guid companyID, Guid instanceID)
        {
            var periodTypes = await _clientPeriodType.GetAllAsync(companyID, instanceID);
            return (int)periodTypes.FirstOrDefault()?.HolidayPremiumPaymentType;
        }


        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id, Guid employeeID)
        {
            await client.DeleteAsync(new List<Guid>() { id }, SessionModel.CompanyID);

            //Recalculate
            await recalculateAsync(employeeID);

            return Json("OK");
        }
    }
}