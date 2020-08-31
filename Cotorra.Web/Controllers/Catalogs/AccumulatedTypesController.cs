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
    public class AccumulatedTypesController : Controller
    {
        private readonly Client<AccumulatedType> client;

        public AccumulatedTypesController()
        {
            SessionModel.Initialize();
            client = new Client<AccumulatedType>(SessionModel.AuthorizationHeader,
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
                         r.TypeOfAccumulated
                     };

            return Json(at);
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(TypeOfAccumulatedDTO data)
        {
            var accumulatedType = new AccumulatedType()
            {
                Name = data.Name,
                TypeOfAccumulated = (TypeOfAccumulated)data.TypeOfAccumulated,

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
                accumulatedType.ID = Guid.NewGuid();
                await client.CreateAsync(new List<AccumulatedType> { accumulatedType }, SessionModel.IdentityID);
            }
            else
            {
                accumulatedType.ID = data.ID.Value;
                await client.UpdateAsync(new List<AccumulatedType> { accumulatedType }, SessionModel.IdentityID);
            }

            return Json(accumulatedType.ID);
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            await client.DeleteAsync(new List<Guid>() { id }, SessionModel.CompanyID);
            return Json("OK");
        }

        [DataContract]
        public class TypeOfAccumulatedDTO
        {
            [DataMember]
            public Guid? ID { get; set; }
            [DataMember]
            public String Name { get; set; }
            [DataMember]
            public Int32 TypeOfAccumulated { get; set; }
        }
    }
}
