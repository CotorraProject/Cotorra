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
    public class AnualIncomeTaxController : Controller
    {
        private readonly Client<AnualIncomeTax> client;

        public AnualIncomeTaxController()
        {
            SessionModel.Initialize();
            client = new Client<AnualIncomeTax>(SessionModel.AuthorizationHeader, clientadapter:
                ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var result = await client.GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID);
            var seniorities = new List<AnualIncomeTax>();

            var ait =
                from r in result
                orderby r.LowerLimit
                select new
                {
                    ID = r.ID,
                    LowerLimit = r.LowerLimit,
                    FixedFee = r.FixedFee,
                    Rate = r.Rate,
                    Validity = r.ValidityDate
                };

            return Json(ait.GroupBy(x => x.Validity).OrderByDescending(x => x.Key));
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(AnualIncomeTaxModel mit)
        {
            var m = new AnualIncomeTax
            {
                InstanceID = SessionModel.InstanceID,
                CompanyID = SessionModel.CompanyID,
                IdentityID = SessionModel.IdentityID,

                LowerLimit = mit.LowerLimit,
                FixedFee = mit.FixedFee,
                Rate = mit.Rate,
            };

            if (!mit.ID.HasValue)
            {
                m.ID = Guid.NewGuid();
                await client.CreateAsync(new List<AnualIncomeTax>() { m }, SessionModel.CompanyID);
            }
            else
            {
                m.ID = mit.ID.Value;
                await client.UpdateAsync(new List<AnualIncomeTax>() { m }, SessionModel.CompanyID);
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

        public class AnualIncomeTaxModel
        {
            public Guid? ID { get; set; }
            public Decimal LowerLimit { get; set; }
            public Decimal FixedFee { get; set; }
            public Decimal Rate { get; set; }
        }

    }
}
