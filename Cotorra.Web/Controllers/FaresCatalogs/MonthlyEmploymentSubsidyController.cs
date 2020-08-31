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
    public class MonthlyEmploymentSubsidyController : Controller
    {
        private readonly Client<MonthlyEmploymentSubsidy> client;

        public MonthlyEmploymentSubsidyController()
        {
            SessionModel.Initialize();
            client = new Client<MonthlyEmploymentSubsidy>(SessionModel.AuthorizationHeader, clientadapter:
                ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var result = await client.GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID);

            var mit = (
                from r in result
                orderby r.LowerLimit
                select new
                {
                    ID = r.ID,
                    LowerLimit = r.LowerLimit,
                    MonthlySubsidy = r.MonthlySubsidy,
                    Validity = r.ValidityDate
                });

            return Json(mit.GroupBy(x => x.Validity).OrderByDescending(x => x.Key));
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(MonthlyEmploymentSubsidyModel mes)
        {
            //var m = new MonthlyEmploymentSubsidy
            //{
            //    LowerLimit = mes.LowerLimit,
            //    MonthlySubsidy = mes.MonthlySubsidy,
            //    ID = Guid.NewGuid(),
            //    Active = true, 
            //    Timestamp = DateTime.UtcNow,
            //    Description = "Some MonthlyEmploymentSubsidy",
            //    CreationDate = DateTime.Now,
            //    Name = "Monthly Employment Subsidy",
            //    StatusID = 1, 
            //    user = SessionModel.IdentityID,
            //    company = SessionModel.CompanyID,
            //    InstanceID = SessionModel.InstanceID,

            //};

            //if (!mes.ID.HasValue)
            //{
            //    m.ID = Guid.NewGuid();
            //    await client.CreateAsync(new List<MonthlyEmploymentSubsidy>() { m }, SessionModel.CompanyID);
            //}
            //else
            //{
            //    m.ID = mes.ID.Value;
            //    await client.UpdateAsync(new List<MonthlyEmploymentSubsidy>() { m }, SessionModel.CompanyID);
            //}

            //return Json(m.ID);

            return await Task.FromResult(Json("OK"));
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            //await client.DeleteAsync(new List<Guid> { id }, SessionModel.CompanyID);
            return await Task.FromResult( Json("OK"));
        }

        public class MonthlyEmploymentSubsidyModel
        {
            public Guid? ID { get; set; }
            public Decimal LowerLimit { get; set; }
            public Decimal MonthlySubsidy { get; set; }
        }

    }
}
