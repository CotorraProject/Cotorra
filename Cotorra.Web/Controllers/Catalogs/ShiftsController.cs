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

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class ShiftsController : Controller
    {
        private readonly Client<Workshift> client;

        public ShiftsController()
        {
            SessionModel.Initialize();
            client = new Client<Workshift>(SessionModel.AuthorizationHeader,
                clientadapter: ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var response = new List<Shift>();

            var result = await client.GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID);
            for (int i = 0; i < result.Count(); i++)
            {
                response.Add(
                    new Shift()
                    {
                        ID = result[i].ID,
                        Name = result[i].Name,
                        TotalHours = result[i].Hours,
                        ShiftWorkingDayType = result[i].ShiftWorkingDayType
                    });
            }

            return Json(response.OrderBy(x => x.Name));
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(Shift data)
        {
            var lstWorkshift = new List<Workshift>();

            lstWorkshift.Add(new Workshift()
            {
                Active = true,
                company = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                CreationDate = DateTime.Now,
                Description = String.Empty,
                ID = data.ID ?? Guid.NewGuid(),
                Name = data.Name,
                StatusID = 1,
                user = SessionModel.IdentityID,
                Timestamp = DateTime.Now,
                Hours = data.TotalHours,
                ShiftWorkingDayType = data.ShiftWorkingDayType
            });

            if (!data.ID.HasValue)
            {
                await client.CreateAsync(lstWorkshift, SessionModel.IdentityID);
            }
            else
            {
                await client.UpdateAsync(lstWorkshift, SessionModel.IdentityID);
            }

            return Json(lstWorkshift.FirstOrDefault().ID);
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            await client.DeleteAsync(new List<Guid>() { id }, SessionModel.CompanyID);
            return Json("OK");
        }

        public class Shift
        {
            public Guid? ID { get; set; }
            public String Name { get; set; }
            public Double TotalHours { get; set; }
            public ShiftWorkingDayType ShiftWorkingDayType { get; set; }
        }
    }
}
