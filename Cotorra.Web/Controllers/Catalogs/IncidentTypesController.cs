using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cotorra.Schema;
using Cotorra.Client;
using System.Linq;
using Cotorra.Web.Utils;
using System.Runtime.Serialization;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class IncidentTypesController : Controller
    {
        private readonly Client<IncidentType> client;

        public IncidentTypesController()
        {
            SessionModel.Initialize();
            client = new Client<IncidentType>(SessionModel.AuthorizationHeader,
                clientadapter: ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {

            var result = await client.GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID);
            var at = from r in result
                     orderby r.Name
                     select new
                     {
                         r.ID,
                         r.Name,
                         r.Code,
                         r.TypeOfIncident,
                         r.SalaryRight,
                         r.DecreasesSeventhDay,
                         r.ItConsiders,
                     };

            return Json(at);
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(TypeOfIndicentDTO data)
        {
            var accumulatedTypes = new List<AccumulatedType>();
            if (data.AccumulatedTypes != null)
            {
                for (var i = 0; i < data.AccumulatedTypes.Count(); i++)
                {
                    accumulatedTypes.Add(new AccumulatedType
                    {
                        ID = data.AccumulatedTypes[i]
                    });
                }
            }

            var indicentType = new IncidentType()
            {
                Code = data.Code,
                Name = data.Name,
                TypeOfIncident = (TypeOfIncident)data.TypeOfIncident,
                SalaryRight = data.SalaryRight,
                DecreasesSeventhDay = data.DecreasesSeventhDay,
                ItConsiders = (ItConsiders)data.ItConsiders,
                AccumulatedTypes = accumulatedTypes,

                //Common
                Active = true,
                company = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                CreationDate = DateTime.Now,
                StatusID = 1,
                user = SessionModel.IdentityID,
                Timestamp = DateTime.Now,
            };

            if (!data.ID.HasValue)
            {
                indicentType.ID = Guid.NewGuid();
                await client.CreateAsync(new List<IncidentType> { indicentType }, SessionModel.IdentityID);
            }
            else
            {
                indicentType.ID = data.ID.Value;
                await client.UpdateAsync(new List<IncidentType> { indicentType }, SessionModel.IdentityID);
            }

            return Json(indicentType.ID);
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            await client.DeleteAsync(new List<Guid>() { id }, SessionModel.CompanyID);
            return Json("OK");
        }

        [DataContract]
        public class TypeOfIndicentDTO
        {
            [DataMember]
            public Guid? ID { get; set; }
            [DataMember]
            public String Code { get; set; }
            [DataMember]
            public String Name { get; set; }
            [DataMember]
            public Int32 TypeOfIncident { get; set; }
            [DataMember]
            public Boolean SalaryRight { get; set; }
            [DataMember]
            public Boolean DecreasesSeventhDay { get; set; }
            [DataMember]
            public Int32 ItConsiders { get; set; }
            [DataMember]
            public List<Guid> AccumulatedTypes { get; set; }
        }
    }
}
