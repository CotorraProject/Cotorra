using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cotorra.Schema;
using Cotorra.Client;
using System.Linq;
using Cotorra.Web.Utils;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class AreasController : Controller
    {
        private readonly Client<Cotorra.Schema.Area> client;

        public AreasController()
        {
            SessionModel.Initialize();
            client = new Client<Cotorra.Schema.Area>(SessionModel.AuthorizationHeader, ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var result = (await client
                .GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID))
                .Select(x => new Area
                {
                    ID = x.ID,
                    Name = x.Name,
                })
                .OrderBy(x => x.Name)
                .ToList();

            return Json(result);
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(Area data)
        {
            var lstArea = new List<Cotorra.Schema.Area>();

            lstArea.Add(new Cotorra.Schema.Area()
            {
                ID = data.ID ?? Guid.NewGuid(),
                Name = data.Name,
                Description = String.Empty,

                //Common
                Active = true,
                company = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                CreationDate = DateTime.Now,
                StatusID = 1,
                user = SessionModel.IdentityID,
                Timestamp = DateTime.Now,
            });

            if (!data.ID.HasValue)
            {
                await client.CreateAsync(lstArea, SessionModel.IdentityID);
            }
            else
            {
                await client.UpdateAsync(lstArea, SessionModel.IdentityID);
            }

            return Json(lstArea.FirstOrDefault().ID);
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            await client.DeleteAsync(new List<Guid>() { id }, SessionModel.CompanyID);
            return Json("OK");
        }

        public class Area
        {
            public Guid? ID { get; set; }
            public String Name { get; set; }
        }
    }
}
