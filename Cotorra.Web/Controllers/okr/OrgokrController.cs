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
using System.Runtime.Serialization;
using CotorraNode.Common.Service.Provisioning.API.DependencesDTO;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Internal;
using System.Diagnostics;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class OrgokrController : Controller
    {

        public OrgokrController()
        {
     
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get(Guid periodTypeID, Guid? periodDetailID = null)
        {
           
            var organizationalObjectives = new List<OrgObjectiveDTO>()
            {
                new OrgObjectiveDTO()
                {
                    ID = Guid.NewGuid(), 
                    Description = "Incrementar las ventas trimestrales en un 30%",
                    Name = "Incrementar las ventas",
                    DueDate = DateTime.Now,
                },
                new OrgObjectiveDTO()
                {
                    ID = Guid.NewGuid(),
                    Description = "Decremetar los gastos en un 10%",
                    Name = "Decrementar los gastos",
                    DueDate = DateTime.Now,
                }
            };

            var organizationalObjectivesInactive = new List<OrgObjectiveDTO>()
            {
                new OrgObjectiveDTO()
                {
                    ID = Guid.NewGuid(),
                    Name = "51",
                    Description = "Soy la descripcion inactiva"
                },
                new OrgObjectiveDTO()
                {
                    ID = Guid.NewGuid(),
                    Name = "52",
                    Description = "Soy la descripcion 2 inactiva"
                }
            };

            var result = new
            {
                Active = organizationalObjectives,
                Inactive = organizationalObjectivesInactive
            };

            return await Task.FromResult( Json(result));
 
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(OrgObjectiveDTO data)
        {
            //var lstDepartment = new List<Cotorra.Schema.Department>();

            //lstDepartment.Add(new Cotorra.Schema.Department()
            //{
            //    ID = data.ID ?? Guid.NewGuid(),
            //    Name = data.Name,
            //    Description = String.Empty,
            //    AreaID = data.AreaID,

            //    //Common
            //    Active = true,
            //    company = SessionModel.CompanyID,
            //    InstanceID = SessionModel.InstanceID,
            //    CreationDate = DateTime.Now,
            //    StatusID = 1,
            //    user = SessionModel.IdentityID,
            //    Timestamp = DateTime.Now,
            //});

            //if (!data.ID.HasValue)
            //{
            //    await client.CreateAsync(lstDepartment, SessionModel.IdentityID);
            //}
            //else
            //{
            //    await client.UpdateAsync(lstDepartment, SessionModel.IdentityID);
            //}

            return await Task.FromResult( Json(Guid.NewGuid()));
        }


    }

    public class OrgObjectiveDTO
    {
        public Guid? ID { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public DateTime DueDate { get; set; }
    }
}
