using System;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Cotorra.Schema;
using Cotorra.Client;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using CotorraNode.TelemetryComponent.Attributes;
using Cotorra.Web.Utils;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class IMSSController : Controller
    {
        private readonly Client<IMSSFare> client;

        public IMSSController()
        {
            SessionModel.Initialize();
            client = new Client<IMSSFare>(SessionModel.AuthorizationHeader, clientadapter:
                ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var result = await client.GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID);

            return Json(
                from r in result
                orderby r.IMMSBranch
                select new
                {
                    ID = r.ID,
                    IMMSBranch = r.IMMSBranch.ToUpper(),
                    EmployerShare = r.EmployerShare * 100,
                    EmployeeShare = r.EmployeeShare * 100,
                    MaxSMDF = r.MaxSMDF
                });
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(IMSSFareModel imss)
        {
            var i = new IMSSFare
            {
                InstanceID = SessionModel.InstanceID,
                CompanyID = SessionModel.CompanyID,
                IdentityID = SessionModel.IdentityID,

                IMMSBranch = imss.IMMSBranch,
                EmployerShare = imss.EmployerShare / 100,
                EmployeeShare = imss.EmployeeShare / 100,
                MaxSMDF = imss.MaxSMDF
            };

            i.ID = imss.ID.Value;
            await client.UpdateAsync(new List<IMSSFare>() { i }, SessionModel.CompanyID);

            return Json(i.ID);
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            await client.DeleteAsync(new List<Guid> { id }, SessionModel.CompanyID);
            return Json("OK");
        }

        public class IMSSFareModel
        {
            public Guid? ID { get; set; }
            public String IMMSBranch { get; set; }
            public Decimal EmployerShare { get; set; }
            public Decimal EmployeeShare { get; set; }
            public Int32 MaxSMDF { get; set; }
        }
    }
}
