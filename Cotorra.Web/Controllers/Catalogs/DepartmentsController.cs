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
    public class DepartmentsController : Controller
    {
        private readonly Client<Cotorra.Schema.Department> client;

        public DepartmentsController()
        {
            SessionModel.Initialize();
            client = new Client<Cotorra.Schema.Department>(SessionModel.AuthorizationHeader, ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var result = (await client
                .GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID)).AsParallel()
                .Select(x => new Department
                {
                    ID = x.ID,
                    Name = x.Name,
                    AreaID = x.AreaID
                })
                .OrderBy(x => x.Name)
                .ToList();

            return Json(result);
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(DepartmentDTO data)
        {
            var lstDepartment = new List<Cotorra.Schema.Department>();

            lstDepartment.Add(new Cotorra.Schema.Department()
            {
                ID = data.ID ?? Guid.NewGuid(),
                Name = data.Name,
                Description = String.Empty,
                AreaID = data.AreaID,

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
                await client.CreateAsync(lstDepartment, SessionModel.IdentityID);
            }
            else
            {
                await client.UpdateAsync(lstDepartment, SessionModel.IdentityID);
            }

            return Json(lstDepartment.FirstOrDefault().ID);
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            await client.DeleteAsync(new List<Guid>() { id }, SessionModel.CompanyID);
            return Json("OK");
        }

        public class DepartmentDTO
        {
            public Guid? ID { get; set; }
            public String Name { get; set; }
            public Guid? AreaID { get; set; }
        }
    }
}
