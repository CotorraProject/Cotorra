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
    public class EmployerRegistrationController : Controller
    {
        #region "Attributes"
        private readonly Client<Cotorra.Schema.EmployerRegistration> client;
        private readonly Client<Cotorra.Schema.Address> clientAddress;
        #endregion

        #region "Constructor"
        public EmployerRegistrationController()
        {
            SessionModel.Initialize();
            client = new Cotorra.Client.Client<Cotorra.Schema.EmployerRegistration>(SessionModel.AuthorizationHeader
                , clientadapter: ClientConfiguration.GetAdapterFromConfig());           
            clientAddress = new Cotorra.Client.Client<Cotorra.Schema.Address>(SessionModel.AuthorizationHeader
                , clientadapter: ClientConfiguration.GetAdapterFromConfig());
        }
        #endregion

        #region "Controller Methods"
        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            List<EmployerRegistration> response;

            //get all register information
            var result = await client
                .GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID, new string[] { "Address" });

            response = result.Select(x =>
                new EmployerRegistration
                {
                    ID = x.ID,
                    Code = x.Code.ToUpper(),
                    ExteriorNumber = x.Address?.ExteriorNumber,
                    FederalEntity = x.Address?.FederalEntity,
                    InteriorNumber = x.Address?.InteriorNumber,
                    Municipality = x.Address?.Municipality,
                    Reference = x.Address?.Reference,
                    RiskClass = x.RiskClass,
                    RiskClassFraction = x.RiskClassFraction,
                    Street = x.Address?.Street,
                    Suburb = x.Address?.Suburb,
                    ZipCode = x.Address?.ZipCode,
                })
                .OrderBy(x => x.Code)
                .ToList();


            return Json(response);
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(EmployerRegistration data)
        {
            var lstEmployerRegistration = new List<Cotorra.Schema.EmployerRegistration>();
            var lstAddress = new List<Address>();

            lstAddress.Add(new Address()
            {
                Active = true,
                company = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                CreationDate = DateTime.Now,
                Description = String.Empty,
                ID = data.AddressID ?? Guid.NewGuid(),
                StatusID = 1,
                user = SessionModel.IdentityID,
                Timestamp = DateTime.Now,

                ExteriorNumber = data.ExteriorNumber,
                ZipCode = data.ZipCode,
                FederalEntity = data.FederalEntity,
                InteriorNumber = data.InteriorNumber,
                Municipality = data.Municipality,
                Reference = data.Reference,
                Street = data.Street,
                Suburb = data.Suburb
            });

            lstEmployerRegistration.Add(new Cotorra.Schema.EmployerRegistration()
            {
                Active = true,
                company = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                CreationDate = DateTime.Now,
                Description = String.Empty,
                ID = data.ID ?? Guid.NewGuid(),
                StatusID = 1,
                user = SessionModel.IdentityID,
                Timestamp = DateTime.Now,

                Code = data.Code.ToUpper(),
                DeleteDate = null,
                Name = String.Empty,
                RiskClass = data.RiskClass,
                RiskClassFraction = data.RiskClassFraction,
                AddressID = data.AddressID ?? lstAddress.FirstOrDefault().ID,
            });

            if (!data.AddressID.HasValue)
            {
                await clientAddress.CreateAsync(lstAddress, SessionModel.IdentityID);
            }
            else
            {
                await clientAddress.UpdateAsync(lstAddress, SessionModel.IdentityID);
            }

            if (!data.ID.HasValue)
            {
                await client.CreateAsync(lstEmployerRegistration, SessionModel.IdentityID);
            }
            else
            {
                await client.UpdateAsync(lstEmployerRegistration, SessionModel.IdentityID);
            }

            return Json(lstEmployerRegistration.FirstOrDefault().ID);
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            await client.DeleteAsync(new List<Guid>() { id }, SessionModel.CompanyID);
            return Json("OK");
        }
        #endregion

        #region "Models"
        public class EmployerRegistration
        {
            public Guid? ID { get; set; }
            public Guid? AddressID { get; set; }
            public String RFC { get; set; }
            public String SocialReason { get; set; }
            public String CURP { get; set; }
            public String Code { get; set; }
            public String RiskClass { get; set; }
            public Decimal RiskClassFraction { get; set; }
            public String ZipCode { get; set; }
            public String FederalEntity { get; set; }
            public String Municipality { get; set; }
            public String Street { get; set; }
            public String ExteriorNumber { get; set; }
            public String InteriorNumber { get; set; }
            public String Suburb { get; set; }
            public String Reference { get; set; }          
        }
       
        #endregion
    }
}
