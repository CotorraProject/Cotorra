using Microsoft.AspNetCore.Mvc;
using Cotorra.Client;
using Cotorra.Schema;
using Cotorra.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cotorra.Web.Controllers.PrePayroll
{
    public class InhabilityController : Controller
    {
        private readonly Client<Inhability> client;
        private readonly CalculationClient _calculationClient;

        public InhabilityController()
        {
            SessionModel.Initialize();
            var clientAdapter = ClientConfiguration.GetAdapterFromConfig();
            client = new Client<Inhability>(SessionModel.AuthorizationHeader, clientAdapter);
            _calculationClient = new CalculationClient(SessionModel.AuthorizationHeader, clientAdapter);
        }

        private async Task recalculateAsync(Guid employeeID)
        {
            //recalcular debido a modificación en incremento de salario
            var ids = new List<Guid> { employeeID };
            await _calculationClient.CalculationByEmployeesAsync(new CalculationFireAndForgetByEmployeeParams()
            {
                EmployeeIds = ids,
                IdentityWorkID = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                UserID = SessionModel.IdentityID
            });
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get(Guid employeeID)
        {
            var findResult = await client.FindAsync(x => x.InstanceID == SessionModel.InstanceID
                && x.EmployeeID == employeeID, SessionModel.CompanyID, new String[] { });

            var inhabilities = (from x in findResult
                                orderby x.Folio
                                select new
                                {
                                    x.ID,
                                    x.Folio,
                                    x.IncidentTypeID,
                                    x.AuthorizedDays,
                                    x.InitialDate,
                                    x.EmployeeID,
                                    x.CategoryInsurance,
                                    x.RiskType,
                                    x.Percentage,
                                    x.Consequence,
                                    x.InhabilityControl,
                                    x.Description
                                });

            return Json(inhabilities);
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(Inhability inhability)
        {
            var lstInhabilities = new List<Inhability>();

            //Common
            inhability.Active = true;
            inhability.company = SessionModel.CompanyID;
            inhability.InstanceID = SessionModel.InstanceID;
            inhability.CreationDate = DateTime.Now;
            inhability.StatusID = 1;
            inhability.user = SessionModel.IdentityID;
            inhability.Timestamp = DateTime.Now;
            inhability.Employee = null;

            lstInhabilities.Add(inhability);

            if (inhability.ID == Guid.Empty)
            {
                inhability.ID = Guid.NewGuid();
                await client.CreateAsync(lstInhabilities, SessionModel.CompanyID);
            }
            else
            {
                await client.UpdateAsync(lstInhabilities, SessionModel.CompanyID);
            }

            //recalculate
            await recalculateAsync(inhability.EmployeeID);

            return Json(lstInhabilities.FirstOrDefault().ID);
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id, Guid employeeID)
        {
            await client.DeleteAsync(new List<Guid>() { id }, SessionModel.CompanyID);

            //recalculate
            await recalculateAsync(employeeID);

            return Json("OK");
        }
    }
}