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
    public class PositionsController : Controller
    {
        private readonly Client<JobPosition> client;

        public PositionsController()
        {
            SessionModel.Initialize();
            client = new Client<JobPosition>(SessionModel.AuthorizationHeader, clientadapter:
                ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var positions = new List<Position>();

            var result = await client.GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID);
            positions = (from r in result
                         orderby r.Name
                         select new Position
                         {
                             ID = r.ID,
                             Name = r.Name,
                             JobPositionRiskType = r.JobPositionRiskType,
                         }).ToList();

            return Json(positions);
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(Position data)
        {
            var lstJobPositions = new List<JobPosition>()
            {
                new JobPosition()
                {
                    Active = true,
                    company = SessionModel.CompanyID,
                    InstanceID = SessionModel.InstanceID,
                    CreationDate = DateTime.Now,
                    Description = String.Empty,
                    ID = data.ID ?? Guid.NewGuid(),
                    Name = data.Name,
                    StatusID = 1,
                    JobPositionRiskType = data.JobPositionRiskType,
                    user = SessionModel.IdentityID,
                    Timestamp = DateTime.Now
                }
            };

            if (!data.ID.HasValue)
            {
                await client.CreateAsync(lstJobPositions, SessionModel.IdentityID);
            }
            else
            {
                await client.UpdateAsync(lstJobPositions, SessionModel.IdentityID);
            }

            return Json(lstJobPositions.FirstOrDefault().ID);
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            await client.DeleteAsync(new List<Guid>() { id }, SessionModel.CompanyID);
            return Json("OK");
        }

        public class Position
        {
            public Guid? ID { get; set; }
            public String Name { get; set; }

            public JobPositionRiskType JobPositionRiskType { get; set; }
        }
    }
}
