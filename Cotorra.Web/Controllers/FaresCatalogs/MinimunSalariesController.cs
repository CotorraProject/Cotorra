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
using AutoMapper;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class MinimunSalariesController : Controller
    {
        private readonly Client<MinimunSalary> client;

        public MinimunSalariesController()
        {
            SessionModel.Initialize();
            client = new Client<MinimunSalary>(SessionModel.AuthorizationHeader, clientadapter:
                ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var result = await client.GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID);

            return Json(
                from r in result
                where r.ExpirationDate >= new DateTime(2017, 1, 1, 0, 0, 0)
                orderby r.ExpirationDate descending
                select new
                {
                    r.ID,
                    r.ExpirationDate,
                    r.ZoneA,
                    r.ZoneB,
                    r.ZoneC
                }
            );
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(MinimunSalaryModel minimunSalary)
        {
            //Create object to save/update
            var ms = new MinimunSalary
            {
                ExpirationDate = CotorraTools.ValidateDate(minimunSalary.ExpirationDate),
                ZoneA = minimunSalary.ZoneA,
                ZoneB = minimunSalary.ZoneB,
                ZoneC = minimunSalary.ZoneC,

                InstanceID = SessionModel.InstanceID,
                CompanyID = SessionModel.CompanyID,
                IdentityID = SessionModel.IdentityID,
            };

            if (!minimunSalary.ID.HasValue)
            {
                ms.ID = Guid.NewGuid();
                await client.CreateAsync(new List<MinimunSalary>() { ms }, SessionModel.CompanyID);
            }
            else
            {
                ms.ID = minimunSalary.ID.Value;
                await client.UpdateAsync(new List<MinimunSalary>() { ms }, SessionModel.CompanyID);
            }

            return Json(ms.ID);
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            await client.DeleteAsync(new List<Guid> { id }, SessionModel.CompanyID);
            return Json("OK");
        }

        public class MinimunSalaryModel
        {
            public Guid? ID { get; set; }
            public String ExpirationDate { get; set; }
            public Decimal ZoneA { get; set; }
            public Decimal ZoneB { get; set; }
            public Decimal ZoneC { get; set; }
        }
    }
}
