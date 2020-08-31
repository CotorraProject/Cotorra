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
    public class BenefitTypesController : Controller
    {
        private readonly Client<BenefitType> client;

        public BenefitTypesController()
        {
            SessionModel.Initialize();
            client = new Client<BenefitType>(SessionModel.AuthorizationHeader, clientadapter:
                ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var result = await client.GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID);

            var set = (
               from r in result.AsParallel()
               orderby r.Name descending, r.Antiquity ascending
               select new
               {
                   r.ID,
                   Antiquity = r.Antiquity,
                   Name = r.Name,
                   Holidays = r.Holidays,
                   HolidayPremiumPortion = r.HolidayPremiumPortion,
                   DaysOfChristmasBonus = r.DaysOfChristmasBonus,
                   IntegrationFactor = r.IntegrationFactor,
                   DaysOfAntiquity = r.DaysOfAntiquity
               });

            return Json(set.GroupBy(x => x.Name).OrderBy(x => x.Key));
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(BenefitTypeModel benefitTypeModel)
        {
            var benefitTypeToSend = new BenefitType
            {
                Antiquity = benefitTypeModel.Antiquity,
                InstanceID = SessionModel.InstanceID,
                CompanyID = SessionModel.CompanyID,
                IdentityID = SessionModel.IdentityID,
                user = SessionModel.IdentityID,
                Name = benefitTypeModel.Name,
                Holidays = benefitTypeModel.Holidays,
                HolidayPremiumPortion = benefitTypeModel.HolidayPremiumPortion,
                DaysOfChristmasBonus = benefitTypeModel.DaysOfChristmasBonus,
                IntegrationFactor = benefitTypeModel.IntegrationFactor,
                DaysOfAntiquity = benefitTypeModel.DaysOfAntiquity,
            };

            //create
            if (!benefitTypeModel.ID.HasValue)
            {
                benefitTypeToSend.ID = Guid.NewGuid();
                await client.CreateAsync(new List<BenefitType>() { benefitTypeToSend }, SessionModel.CompanyID);
            }
            //update
            else
            {
                benefitTypeToSend.ID = benefitTypeModel.ID.Value;

                //update
                await client.UpdateAsync(new List<BenefitType>() { benefitTypeToSend }, SessionModel.CompanyID);
            }

            return Json(benefitTypeToSend.ID);
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            await client.DeleteAsync(new List<Guid> { id }, SessionModel.CompanyID);
            return Json("OK");
        }

        public class BenefitTypeModel
        {
            /// <summary>
            /// ID
            /// </summary>
            public Guid? ID { get; set; }

            /// <summary>
            /// Nombre
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Antigüedad
            /// </summary>
            public Int32 Antiquity { get; set; }
            /// <summary>
            /// Días de vacaciones
            /// </summary>
            public decimal Holidays { get; set; }

            /// <summary>
            /// Porcion prima vacacional
            /// </summary>
            public decimal HolidayPremiumPortion { get; set; }

            /// <summary>
            /// Días de aguinaldo
            /// </summary>
            public decimal DaysOfChristmasBonus { get; set; }

            /// <summary>
            /// Factor de integración
            /// </summary>
            public decimal IntegrationFactor { get; set; }

            /// <summary>
            /// Días Antigüedad
            /// </summary>
            public decimal DaysOfAntiquity { get; set; }
        }
    }
}
