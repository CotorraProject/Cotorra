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
    public class SettlementController : Controller
    {
        private readonly Client<SettlementCatalog> client;

        public SettlementController()
        {
            SessionModel.Initialize();
            client = new Client<SettlementCatalog>(SessionModel.AuthorizationHeader, clientadapter:
                ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var result = await client.GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID);

            var set = (
                from r in result.AsParallel()
                orderby r.ValidityDate descending
                select new
                {
                    r.ID,
                    Validity = r.ValidityDate,
                    r.CASUSMO,
                    r.CASISR86,
                    r.CalDirecPerc,
                    r.Indem90,
                    r.Indem20,
                    r.PrimaAntig,
                });

            return Json(set.GroupBy(x => x.Validity).OrderByDescending(x => x.Key));

        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(SettlementModel settlement)
        {
            var settlementToSend = new SettlementCatalog
            {
                ID = settlement.ID.Value,
                CASUSMO = settlement.CASUSMO,
                CASISR86 = settlement.CASISR86,
                CalDirecPerc = settlement.CalDirecPerc,
                Indem90 = settlement.Indem90,
                Indem20 = settlement.Indem20,
                PrimaAntig = settlement.PrimaAntig,
                ValidityDate = settlement.Validity,

                //Common
                Active = true,
                company = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                CreationDate = DateTime.Now,
                StatusID = 1,
                user = SessionModel.IdentityID,
                Timestamp = DateTime.Now,
            };

            //Just update
            await client.UpdateAsync(new List<SettlementCatalog>() { settlementToSend }, SessionModel.CompanyID);

            return Json(settlement.ID);
        }

        public class SettlementModel
        {
            public Guid? ID { get; set; }
            public Decimal CASUSMO { get; set; }
            public Decimal CASISR86 { get; set; }
            public Decimal CalDirecPerc { get; set; }
            public Decimal Indem90 { get; set; }
            public Decimal Indem20 { get; set; }
            public Decimal PrimaAntig { get; set; }
            public DateTime Validity { get; set; }
        }

    }
}
