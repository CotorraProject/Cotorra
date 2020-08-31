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
using CotorraNode.TelemetryComponent.Attributes;
using Cotorra.Web.Utils;
using Cotorra.Schema.nom035;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class NOMEvalPeriodsController : Controller
    {
        private Client<NOMEvaluationPeriod> client;

        public NOMEvalPeriodsController()
        {
            SessionModel.Initialize();
            client = new Client<NOMEvaluationPeriod>(SessionModel.AuthorizationHeader, ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var periods = await client.GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID);
            var result = from p in periods
                         select new
                         {
                             p.ID,
                             p.Period
                         };

            return Json(result);
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(EvalPeriod data)
        {
            var lstEvalPeriod = new List<NOMEvaluationPeriod>();

            lstEvalPeriod.Add(new NOMEvaluationPeriod()
            {
                Active = true,
                CreationDate = DateTime.Now,
                Description = String.Empty,
                InstanceID = SessionModel.InstanceID,
                company = SessionModel.CompanyID,
                user = SessionModel.IdentityID,
                ID = data.ID ?? Guid.NewGuid(),
                Name = "",
                Period = data.Period,
                StatusID = 1,
                Timestamp = DateTime.Now,
            });

            if (!data.ID.HasValue)
            {
                await client.CreateAsync(lstEvalPeriod, SessionModel.IdentityID);
            }
            else
            {
                await client.UpdateAsync(lstEvalPeriod, SessionModel.IdentityID);
            }

            return Json(lstEvalPeriod.FirstOrDefault().ID);
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            await client.DeleteAsync(new List<Guid>() { id }, SessionModel.CompanyID);
            return Json("OK");
        }

        public class EvalPeriod
        {
            public Guid? ID { get; set; }
            public String Period { get; set; }
        }
    }
}
