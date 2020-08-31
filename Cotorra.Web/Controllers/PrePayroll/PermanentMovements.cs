using Microsoft.AspNetCore.Mvc;
using Cotorra.Client;
using Cotorra.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Cotorra.Schema;
using System.Threading.Tasks;

namespace Cotorra.Web.Controllers
{
    public class PermanentMovements : Controller
    {
        private readonly Client<PermanentMovement> client;

        public PermanentMovements()
        {
            SessionModel.Initialize();
            client = new Client<PermanentMovement>(SessionModel.AuthorizationHeader, ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get(Guid employeeID)
        {
            var findResult = await client.FindAsync(x => x.InstanceID == SessionModel.InstanceID
               && x.EmployeeID == employeeID, SessionModel.CompanyID, new String[] { });

            var pms = (from x in findResult
                       orderby x.ControlNumber
                       select new
                       {
                           x.ID,
                           x.Description,
                           x.ConceptPaymentID,
                           x.InitialApplicationDate,
                           x.PermanentMovementType,
                           x.Amount,
                           x.TimesToApply,
                           x.TimesApplied,
                           x.LimitAmount,
                           x.AccumulatedAmount,
                           x.RegistryDate,
                           x.ControlNumber,
                           x.PermanentMovementStatus
                       });

            return Json(pms);
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(PermanentMovement pm)
        {
            var lstPms = new List<PermanentMovement>();

            //Common
            pm.Active = true;
            pm.company = SessionModel.CompanyID;
            pm.InstanceID = SessionModel.InstanceID;
            pm.CreationDate = DateTime.Now;
            pm.StatusID = 1;
            pm.user = SessionModel.IdentityID;
            pm.Timestamp = DateTime.Now;

            lstPms.Add(pm);

            if (pm.ID == Guid.Empty)
            {
                pm.ID = Guid.NewGuid();
                await client.CreateAsync(lstPms, SessionModel.IdentityID);
            }
            else
            {
                await client.UpdateAsync(lstPms, SessionModel.IdentityID);
            }

            return Json(lstPms.FirstOrDefault().ID);
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            await client.DeleteAsync(new List<Guid>() { id }, SessionModel.CompanyID);
            return Json("OK");
        }
    }
}