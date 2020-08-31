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
    public class UMAController : Controller
    {
        private readonly Client<UMA> client;

        public UMAController()
        {
            SessionModel.Initialize();
            client = new Client<UMA>(SessionModel.AuthorizationHeader, clientadapter:
                ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var result = await client.GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID);

            return Json(
                from r in result
                orderby r.ValidityDate descending
                select new
                {
                    ID = r.ID,
                    InitialDate = r.ValidityDate,
                    Value = r.Value
                });
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(UMAModel uma)
        {
            var u = new UMA
            {
                ValidityDate = uma.InitialDate,
                Value = uma.Value,
                ID = Guid.NewGuid(),
                Active = true, 
                Timestamp = DateTime.UtcNow,
                Description = "Some Fee",
                CreationDate = DateTime.Now,
                Name = "Nominas fee",
                StatusID = 1,
                user = SessionModel.IdentityID, 
                company = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
            };

            if (!uma.ID.HasValue)
            {
                u.ID = Guid.NewGuid();
                await client.CreateAsync(new List<UMA>() { u }, SessionModel.CompanyID);
            }
            else
            {
                u.ID = uma.ID.Value;
                await client.UpdateAsync(new List<UMA>() { u }, SessionModel.CompanyID);
            }

            return Json(u.ID);
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            await client.DeleteAsync(new List<Guid> { id }, SessionModel.CompanyID);
            return Json("OK");
        }

        public class UMAModel
        {
            public Guid? ID { get; set; }
            public DateTime InitialDate { get; set; }
            public Decimal Value { get; set; }
        }
    }
}
