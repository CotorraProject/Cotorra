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
    public class MonthlyIncomeTaxController : Controller
    {
        private readonly Client<MonthlyIncomeTax> client;

        public MonthlyIncomeTaxController()
        {
            SessionModel.Initialize();
            client = new Client<MonthlyIncomeTax>(SessionModel.AuthorizationHeader, clientadapter:
                ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var result = await client.GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID);

            var mit =
                from r in result.AsParallel()
                orderby r.LowerLimit
                select new
                {
                    ID = r.ID,
                    LowerLimit = r.LowerLimit,
                    FixedFee = r.FixedFee,
                    Rate = r.Rate,
                    Validity = r.ValidityDate
                };

            return Json(mit.GroupBy(x => x.Validity).OrderByDescending(x => x.Key));
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(MonthlyIncomeTaxModel mit)
        {
            var m = new MonthlyIncomeTax
            {
                LowerLimit = mit.LowerLimit,
                FixedFee = mit.FixedFee,
                Rate = mit.Rate,
                Description = "",
                Name = "",
                StatusID = 1,
                user = SessionModel.IdentityID,
                Active = true,
                company = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
            };

            if (!mit.ID.HasValue)
            {
                m.ID = Guid.NewGuid();
                await client.CreateAsync(new List<MonthlyIncomeTax>() { m }, SessionModel.CompanyID);
            }
            else
            {
                m.ID = mit.ID.Value;
                await client.UpdateAsync(new List<MonthlyIncomeTax>() { m }, SessionModel.CompanyID);
            }

            return Json(m.ID);
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            await client.DeleteAsync(new List<Guid> { id }, SessionModel.CompanyID);
            return Json("OK");
        }

        public class MonthlyIncomeTaxModel
        {
            public Guid? ID { get; set; }
            public Decimal LowerLimit { get; set; }
            public Decimal FixedFee { get; set; }
            public Decimal Rate { get; set; }
        }

    }
}
