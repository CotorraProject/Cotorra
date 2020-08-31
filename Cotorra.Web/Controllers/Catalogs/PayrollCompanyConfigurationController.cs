using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cotorra.Schema;
using Cotorra.Client;
using System.Linq;
using Cotorra.Web.Utils;
using System.Runtime.Serialization;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Diagnostics;
using CotorraNode.TelemetryComponent.Attributes;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class PayrollCompanyConfigurationController : Controller
    {
        #region "Attributes"
        private readonly Client<PayrollCompanyConfiguration> _clientConfiguration;
        private readonly Client<Address> _clientAddress;
        private readonly Client<PeriodType> _clientPeriodType;
        private readonly IMapper _mapper;
        #endregion

        #region "Constructor"
        public PayrollCompanyConfigurationController()
        {
            SessionModel.Initialize();
            var clientAdapter = ClientConfiguration.GetAdapterFromConfig();
            _clientConfiguration = new Client<PayrollCompanyConfiguration>(SessionModel.AuthorizationHeader,
                clientadapter: clientAdapter);
            _clientAddress = new Client<Address>(SessionModel.AuthorizationHeader,
                clientadapter: clientAdapter);
            _clientPeriodType = new Client<PeriodType>(SessionModel.AuthorizationHeader,
                clientadapter: clientAdapter);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PayrollCompanyConfiguration, PayrollCompanyConfigurationDTO>();
                cfg.CreateMap<PayrollCompanyConfigurationDTO, PayrollCompanyConfiguration>();
            });

            _mapper = config.CreateMapper();
        }
        #endregion

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var payrollConfigurations = await _clientConfiguration.GetAllAsync(SessionModel.CompanyID,
                SessionModel.InstanceID, new string[] { "Address" });
            var singleConfiguration = payrollConfigurations.FirstOrDefault();

            var result = _mapper.Map<PayrollCompanyConfiguration, PayrollCompanyConfigurationDTO>(singleConfiguration);
            result.ExteriorNumber = singleConfiguration.Address?.ExteriorNumber;
            result.FederalEntity = singleConfiguration.Address?.FederalEntity;
            result.InteriorNumber = singleConfiguration.Address?.InteriorNumber;
            result.Municipality = singleConfiguration.Address?.Municipality;
            result.Reference = singleConfiguration.Address?.Reference;
            result.Street = singleConfiguration.Address?.Street;
            result.Suburb = singleConfiguration.Address?.Suburb;
            result.ZipCode = singleConfiguration.Address?.ZipCode;
            result.SalaryZone = singleConfiguration.SalaryZone;
            result.CompanyCreationDate = singleConfiguration.CompanyCreationDate.HasValue ? singleConfiguration.CompanyCreationDate.Value.ToString("yyyy-MM-ddT00:00:00") : string.Empty;

            result.HolidayPremiumPaymentType = await GetPeriodTypesHolidayConfiguration(SessionModel.CompanyID,
                SessionModel.InstanceID);

            return Json(new List<PayrollCompanyConfigurationDTO> { result });
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(PayrollCompanyConfigurationDTO data)
        {
            var lstPayrollCompanyConfig = new List<PayrollCompanyConfiguration>();
            var id = data.ID.HasValue ? data.ID.Value : Guid.NewGuid();

            //Assign id and Map
            var result = _mapper.Map<PayrollCompanyConfigurationDTO, PayrollCompanyConfiguration>(data);
            result.ID = id;

            lstPayrollCompanyConfig.Add(result);

            var lstAddress = new List<Address>()
                {
                    new Address()
                    {
                        ID = data.AddressID.HasValue ? data.AddressID.Value : Guid.NewGuid(),
                        Active = true,
                        company = SessionModel.CompanyID,
                        CompanyID = SessionModel.CompanyID,
                        CreationDate = DateTime.Now,
                        DeleteDate = null,
                        Description = "",
                        ExteriorNumber = data.ExteriorNumber,
                        FederalEntity = data.FederalEntity,
                        IdentityID = SessionModel.IdentityID,
                        InstanceID = SessionModel.InstanceID,
                        InteriorNumber = data.InteriorNumber,
                        Municipality = data.Municipality,
                        Name = "",
                        Reference = data.Reference,
                        StatusID = 1,
                        Street = data.Street,
                        Suburb = data.Suburb,
                        Timestamp = DateTime.Now,
                        user = SessionModel.IdentityID,
                        ZipCode = data.ZipCode
                    }
                };

            //Address
            if (!data.AddressID.HasValue)
            {
                await _clientAddress.CreateAsync(lstAddress, SessionModel.CompanyID);
            }
            else
            {
                await _clientAddress.UpdateAsync(lstAddress, SessionModel.CompanyID);
            }

            //Configuration
            if (!data.ID.HasValue)
            {
                throw new CotorraException(101, "101", "No es posible crear otra configuración, sólo se puede tener 1 por empresa", null);
            }
            else
            {
                var payrollConfiguration = (await _clientConfiguration.FindAsync(p => p.ID == id, SessionModel.CompanyID)).FirstOrDefault();

                //Data that cannot be replaced
                lstPayrollCompanyConfig.FirstOrDefault().company = payrollConfiguration.company;
                lstPayrollCompanyConfig.FirstOrDefault().InstanceID = payrollConfiguration.InstanceID;
                lstPayrollCompanyConfig.FirstOrDefault().user = SessionModel.IdentityID;
                lstPayrollCompanyConfig.FirstOrDefault().Timestamp = DateTime.Now;
                lstPayrollCompanyConfig.FirstOrDefault().Name = "";
                lstPayrollCompanyConfig.FirstOrDefault().Description = "";
                lstPayrollCompanyConfig.FirstOrDefault().RFC = payrollConfiguration.RFC;
                lstPayrollCompanyConfig.FirstOrDefault().AddressID = lstAddress.FirstOrDefault().ID;
                lstPayrollCompanyConfig.FirstOrDefault().CurrencyID = payrollConfiguration.CurrencyID;
                lstPayrollCompanyConfig.FirstOrDefault().CurrentExerciseYear = payrollConfiguration.CurrentExerciseYear;
                lstPayrollCompanyConfig.FirstOrDefault().CurrentPeriod = payrollConfiguration.CurrentPeriod;

                await _clientConfiguration.UpdateAsync(lstPayrollCompanyConfig, SessionModel.CompanyID);
            }

            await  SavePeriodTypes(SessionModel.CompanyID, SessionModel.InstanceID, data.HolidayPremiumPaymentType);

            return Json(data);
        }

        private async Task SavePeriodTypes(Guid companyID, Guid instanceID, int holidayPaymentConfiguration)
        {
            HolidayPaymentConfiguration selected = (HolidayPaymentConfiguration)holidayPaymentConfiguration;
            var periodTypes = await _clientPeriodType.GetAllAsync(companyID, instanceID);
            periodTypes.AsParallel().ForAll(pt => pt.HolidayPremiumPaymentType = selected);

            await _clientPeriodType.UpdateAsync(periodTypes, companyID);
        }


        private async Task<int> GetPeriodTypesHolidayConfiguration(Guid companyID, Guid instanceID)
        { 
            var periodTypes = await _clientPeriodType.GetAllAsync(companyID, instanceID);
            return (int)periodTypes.FirstOrDefault()?.HolidayPremiumPaymentType;
        }

        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> SaveGeneralData(PayrollCompanyConfigurationDTO data)
        {
            var id = data.ID.HasValue ? data.ID.Value : Guid.NewGuid();

            var lstAddress = new List<Address>()
                {
                    new Address()
                    {
                        ID = data.AddressID.HasValue ? data.AddressID.Value : Guid.NewGuid(),
                        Active = true,
                        company = SessionModel.CompanyID,
                        CompanyID = SessionModel.CompanyID,
                        CreationDate = DateTime.Now,
                        DeleteDate = null,
                        Description = "",
                        ExteriorNumber = data.ExteriorNumber,
                        FederalEntity = data.FederalEntity,
                        IdentityID = SessionModel.IdentityID,
                        InstanceID = SessionModel.InstanceID,
                        InteriorNumber = data.InteriorNumber,
                        Municipality = data.Municipality,
                        Name = "",
                        Reference = data.Reference,
                        StatusID = 1,
                        Street = data.Street,
                        Suburb = data.Suburb,
                        Timestamp = DateTime.Now,
                        user = SessionModel.IdentityID,
                        ZipCode = data.ZipCode
                    }
                };

            //Address
            if (!data.AddressID.HasValue)
            {
                await _clientAddress.CreateAsync(lstAddress, SessionModel.CompanyID);
            }
            else
            {
                await _clientAddress.UpdateAsync(lstAddress, SessionModel.CompanyID);
            }

            //Configuration
            if (!data.ID.HasValue)
            {
                throw new CotorraException(101, "101", "No es posible crear otra configuración, sólo se puede tener 1 por empresa", null);
            }
            else
            {
                var payrollConfiguration = (await _clientConfiguration.FindAsync(p => p.ID == id, SessionModel.CompanyID)).FirstOrDefault();

                payrollConfiguration.CURP = data.CURP;
                payrollConfiguration.SalaryZone = data.SalaryZone;
                payrollConfiguration.FiscalRegime = data.FiscalRegime;
                payrollConfiguration.AddressID = lstAddress.FirstOrDefault().ID;

                var resultJSON = SaveConfiguration(payrollConfiguration);
            }

            return Json(data);
        }

        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> SaveAboutData(PayrollCompanyConfigurationDTO data)
        {
            var id = data.ID.HasValue ? data.ID.Value : Guid.NewGuid();

            if (!data.ID.HasValue)
            {
                throw new CotorraException(101, "101", "No es posible crear otra configuración, sólo se puede tener 1 por empresa", null);
            }
            else
            {
                var payrollConfiguration = (await _clientConfiguration.FindAsync(p => p.ID == id, SessionModel.CompanyID)).FirstOrDefault();

                payrollConfiguration.ComercialName = data.ComercialName;
                payrollConfiguration.CompanyBusinessSector = data.CompanyBusinessSector;
                payrollConfiguration.CompanyCreationDate = string.IsNullOrEmpty(data.CompanyCreationDate) ? (DateTime?)null : CotorraTools.ValidateDate(data.CompanyCreationDate);
                payrollConfiguration.CompanyWebSite = data.CompanyWebSite;
                payrollConfiguration.Facebook = data.Facebook;
                payrollConfiguration.Instagram = data.Instagram;
                payrollConfiguration.CompanyScope = data.CompanyScope;
                payrollConfiguration.CompanyInformation = data.CompanyInformation;

                var resultJSON = SaveConfiguration(payrollConfiguration);
            }

            return Json(data);
        }

        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> SaveVacationConfigData(PayrollCompanyConfigurationDTO data)
        {

            await SavePeriodTypes(SessionModel.CompanyID, SessionModel.InstanceID, data.HolidayPremiumPaymentType);

            return Json(data);
        }

        private async Task<JsonResult> SaveConfiguration(PayrollCompanyConfiguration data)
        {
            var lstPayrollCompanyConfig = new List<PayrollCompanyConfiguration>();
            lstPayrollCompanyConfig.Add(data);

            await _clientConfiguration.UpdateAsync(lstPayrollCompanyConfig, SessionModel.CompanyID);

            return Json(data);
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            throw new Exception("No es posible borrar la configuración de la empresa.");
        }
    }

    [DataContract]
    public class PayrollCompanyConfigurationDTO
    {
        [DataMember]
        public Guid? ID { get; set; }

        [DataMember]
        public Guid InstanceID { get; set; }

        [DataMember]
        public Guid company { get; set; }

        /// <summary>
        /// RFC
        /// </summary>
        [DataMember]
        public String RFC { get; set; }

        /// <summary>
        /// Razón Social
        /// </summary>
        [DataMember]
        public String SocialReason { get; set; }

        /// <summary>
        /// Para personas físicas
        /// </summary>
        [DataMember]
        public String CURP { get; set; }

        /// <summary>
        /// Zona de salario general
        /// </summary>
        [DataMember]
        public SalaryZone SalaryZone { get; set; }

        /// <summary>
        /// Régimen Fiscal
        /// </summary>
        [DataMember]
        public FiscalRegime FiscalRegime { get; set; }

        /// <summary>
        /// Factor no deducible por ingresos excentos
        /// </summary>
        [DataMember]
        public decimal NonDeducibleFactor { get; set; }

        /// <summary>
        /// Moneda
        /// </summary>
        [DataMember]
        public Guid CurrencyID { get; set; }

        /// <summary>
        /// Dirección
        /// </summary>
        public Guid? AddressID { get; set; }

        /*DATOS ADICIONALES DE LA COMPAÑIA NO OBLIGATORIOS*/
        /// <summary>
        /// Pkaticanos sobre tu empresa, información libre.
        /// </summary>
        [DataMember]
        public string CompanyInformation { get; set; }

        /// <summary>
        /// Nombre comercial
        /// </summary>
        [DataMember]
        public string ComercialName { get; set; }

        /// <summary>
        /// Fecha de fundación
        /// </summary>
        [DataMember]
        public string CompanyCreationDate { get; set; }

        /// <summary>
        /// Tu empresa tiene ámbito
        /// </summary>
        [DataMember]
        public CompanyScope CompanyScope { get; set; }

        /// <summary>
        /// Email de la empresa
        /// </summary>
        [DataMember]
        public string CompanyContactEmail { get; set; }

        /// <summary>
        /// Teléfono de contacto de la empresa
        /// </summary>
        [DataMember]
        public string CompanyContactPhone { get; set; }

        /// <summary>
        /// Teléfono de contacto de la empresa
        /// </summary>
        [DataMember]
        public CompanyBusinessSector CompanyBusinessSector { get; set; }

        /// <summary>
        /// Company WebSite
        /// </summary>
        [DataMember]
        public string CompanyWebSite { get; set; }

        /// <summary>
        /// Facebook
        /// </summary>
        [DataMember]
        public string Facebook { get; set; }

        /// <summary>
        /// Instagram
        /// </summary>
        [DataMember]
        public string Instagram { get; set; }

        /// <summary>
        /// Twitter
        /// </summary>
        [DataMember]
        public string Twitter { get; set; }

        /// <summary>
        /// Youtube
        /// </summary>
        [DataMember]
        public string Youtube { get; set; }

        //ADDRESS
        [DataMember]
        public String ZipCode { get; set; }

        [DataMember]
        public String FederalEntity { get; set; }

        [DataMember]
        public String Municipality { get; set; }

        [DataMember]
        public String Street { get; set; }

        [DataMember]
        public String ExteriorNumber { get; set; }

        [DataMember]
        public String InteriorNumber { get; set; }

        [DataMember]
        public String Suburb { get; set; }

        [DataMember]
        public String Reference { get; set; }

        [DataMember]
        public int HolidayPremiumPaymentType { get; set; }
    }
}
