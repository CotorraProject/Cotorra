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
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class WorkCentersController : Controller
    {
        private readonly Client<WorkCenter> client;
        public WorkCentersController()
        {
            SessionModel.Initialize();
            client = new Client<WorkCenter>(SessionModel.AuthorizationHeader,
                clientadapter: ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var result = await client.GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID);
            var periodTypes = from pt in result
                              orderby pt.Name
                              select new
                              {
                                  pt.ID,
                                  pt.Name,

                                  //Address
                                  pt.ZipCode,
                                  pt.FederalEntity,
                                  pt.Municipality,
                                  pt.Street,
                                  pt.ExteriorNumber,
                                  pt.InteriorNumber,
                                  pt.Suburb,
                                  pt.Reference,
                              };

            return Json(periodTypes);
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(WorkCenterDTO data)
        {
            var lstWorkCenter = new List<WorkCenter>();

            lstWorkCenter.Add(new WorkCenter()
            {
                ID = data.ID ?? Guid.NewGuid(),
                Name = data.Name,
                Description = String.Empty,
                Observations = String.Empty,

                //Address
                ZipCode = data.ZipCode,
                FederalEntity = data.FederalEntity,
                Municipality = data.Municipality,
                Street = data.Street,
                ExteriorNumber = data.ExteriorNumber,
                InteriorNumber = data.InteriorNumber,
                Suburb = data.Suburb,
                Reference = data.Reference,

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
                await client.CreateAsync(lstWorkCenter, SessionModel.IdentityID);
            }
            else
            {
                await client.UpdateAsync(lstWorkCenter, SessionModel.IdentityID);
            }

            return Json(lstWorkCenter.FirstOrDefault().ID);
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            await client.DeleteAsync(new List<Guid>() { id }, SessionModel.CompanyID);
            return Json("OK");
        }

        public class WorkCenterDTO
        {
            public Guid? ID { get; set; }
            public String Name { get; set; }
            public String ZipCode { get; set; }
            public String FederalEntity { get; set; }
            public String Municipality { get; set; }
            public String Street { get; set; }
            public String ExteriorNumber { get; set; }
            public String InteriorNumber { get; set; }
            public String Suburb { get; set; }
            public String Reference { get; set; }
        }
    }
}