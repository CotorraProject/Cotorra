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
using System.Globalization;
using CommonUX = CotorraNode.App.Common.UX;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class CompanyAssistantController : Controller
    {
        private readonly InitializationClient client;

        public CompanyAssistantController()
        {
            SessionModel.Initialize();
            client = new InitializationClient(SessionModel.AuthorizationHeader, ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> Create(CompanyCreateModel ccm, EmployerRegistrationController.EmployerRegistration er)
        {
            //Create payment periodicity
            PaymentPeriodicity pp;
            if (ccm.PeriodType == "Weekly") { pp = PaymentPeriodicity.Weekly; }
            else if (ccm.PeriodType == "BiWeekly") { pp = PaymentPeriodicity.Biweekly; }
            else { pp = PaymentPeriodicity.Monthly; }

            //Create employer registration
            EmployerRegistration employerRegistration = null;
            if (ccm.HasIMSS)
            {
                var employerRegistrationAddress = new Address
                {
                    ID = Guid.NewGuid(),
                    ZipCode = er.ZipCode,
                    FederalEntity = er.FederalEntity,
                    Municipality = er.Municipality,
                    Reference = "",
                    ExteriorNumber = "",
                    InteriorNumber = "",
                    Street = "",
                    Suburb = "",

                    //Common
                    Name = String.Empty,
                    Description = String.Empty,
                    StatusID = 1,
                    user = SessionModel.IdentityID,
                    Timestamp = DateTime.Now,
                    DeleteDate = null,
                    Active = true,
                    company = SessionModel.CompanyID,
                    InstanceID = SessionModel.InstanceID,
                    CreationDate = DateTime.Now,
                };

                employerRegistration = new EmployerRegistration
                {
                    ID = Guid.NewGuid(),
                    Code = er.Code.ToUpper(),
                    RiskClass = er.RiskClass,
                    RiskClassFraction = er.RiskClassFraction,
                    AddressID = employerRegistrationAddress.ID,
                    Address = employerRegistrationAddress,

                    //Common
                    Active = true,
                    company = SessionModel.CompanyID,
                    InstanceID = SessionModel.InstanceID,
                    CreationDate = DateTime.Now,
                    Description = String.Empty,
                    StatusID = 1,
                    user = SessionModel.IdentityID,
                    Timestamp = DateTime.Now,
                    DeleteDate = null,
                    Name = String.Empty,
                };
            }

            //Create company and employer registration addresses
            var companyAddress = new Address
            {
                ID = Guid.NewGuid(),
                ZipCode = er.ZipCode,
                FederalEntity = er.FederalEntity,
                Municipality = er.Municipality,
                Reference = "",
                ExteriorNumber = "",
                InteriorNumber = "",
                Street = "",
                Suburb = "",

                //Common
                Name = String.Empty,
                Description = String.Empty,
                StatusID = 1,
                user = SessionModel.IdentityID,
                Timestamp = DateTime.Now,
                DeleteDate = null,
                Active = true,
                company = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                CreationDate = DateTime.Now,
            };

            //Create payroll configuration
            var payrollConfiguration = new PayrollCompanyConfiguration()
            {
                //Step 1
                RFC = ccm.CompanyRFC.ToUpper(),
                CURP = String.IsNullOrEmpty(ccm.CompanyCURP) ? "" : ccm.CompanyCURP.ToUpper(),

                SocialReason = ccm.CompanySocialReason,
                SalaryZone = Enum.Parse<SalaryZone>(ccm.SalaryZone),
                FiscalRegime = ccm.FiscalRegime,
                AddressID = companyAddress.ID,
                Address = companyAddress,
                CurrencyID = ccm.CurrencyID,

                //Step 2
                //EmployerRegistration

                //Step 3
                PaymentPeriodicity = pp, //This is used as PeriodType (...)
                CurrentExerciseYear = ccm.CurrentFiscalYear,
                PeriodInitialDate = ccm.InitialDate,
                CurrentPeriod = ccm.CurrentPeriod,
                //PeriodTotalDays = ccm.PeriodTotalDays, (internally calculated)
                PaymentDays = ccm.PaymentDays,
                WeeklySeventhDay = ccm.WeeklySeventhDay,
                AdjustmentPay = ccm.FortnightPaymentDays,
                NonDeducibleFactor = ccm.NonDeductibleFactor,

                //Step 4
                CompanyInformation = ccm.CompanyInformation,
                ComercialName = ccm.ComercialName,
                CompanyScope = ccm.CompanyScope,
                CompanyBusinessSector = ccm.CompanyBusinessSector,
                CompanyCreationDate = ccm.CompanyCreationDate,
                CompanyWebSite = ccm.CompanyWebSite,
                Facebook = ccm.Facebook,
                Instagram = ccm.Instagram
            };

            var res = await client.InitializeAsync(
                SessionModel.AuthorizationHeader,
                Guid.Parse(CommonUX.SecurityUX.DecryptString(ccm.LicenseServiceID)),
                ccm.CompanySocialReason,
                ccm.CompanyRFC.ToUpper(),
                payrollConfiguration,
                employerRegistration
                );

            return Json(new
            {
                CompanyID = CommonUX.SecurityUX.EncryptString(res.CompanyID.ToString(), SessionModel.EncryptKey),
                InstanceID = CommonUX.SecurityUX.EncryptString(res.InstanceID.ToString(), SessionModel.EncryptKey),
                LicenseID = CommonUX.SecurityUX.EncryptString(res.LicenseID.ToString(), SessionModel.EncryptKey),
                LicenseServiceID = CommonUX.SecurityUX.EncryptString(res.LicenseServiceID.ToString(), SessionModel.EncryptKey)
            });
        }

        public async Task<JsonResult> GetFiscalRegimes()
        {
            var fiscalRegimes = new FiscalRegimeDetails().GetFiscalRegimeInformation().OrderBy(x => x.Description);
            return await Task.FromResult(Json(fiscalRegimes));
        }

        public async Task<JsonResult> GetPeriods(PaymentPeriodicity paymentPeriodicity, DateTime initialDate, Int32 periodTotalDays)
        {
            var fixToMonth = paymentPeriodicity != PaymentPeriodicity.Weekly;
            var result = new PeriodSimulator().GetPeriodDetails(paymentPeriodicity, initialDate, periodTotalDays, fixToMonth);
            var periods = from r in result
                          orderby r.Number
                          select new
                          {
                              id = r.Number,
                              description = r.InitialDate.ToString("dd/MMM/yyyy", new CultureInfo("es-mx")).Replace(".", "")
                          };

            return await Task.FromResult(Json(periods));
        }

        public class CompanyCreateModel
        {
            //Step 1
            public String LicenseServiceID { get; set; }
            public String CompanyRFC { get; set; }
            public String CompanyCURP { get; set; }
            public String CompanySocialReason { get; set; }
            public String SalaryZone { get; set; }
            public FiscalRegime FiscalRegime { get; set; }
            public Guid CurrencyID { get; set; }

            //Step 2
            public Boolean HasIMSS { get; set; }
            //public EmployerRegistration employerRegistration { get; set; }

            //Step 3
            public String PeriodType { get; set; }
            public Int32 CurrentFiscalYear { get; set; }
            public DateTime InitialDate { get; set; }
            public Int32 CurrentPeriod { get; set; }
            public Int32 PeriodTotalDays { get; set; }
            public Decimal PaymentDays { get; set; }
            public Boolean MonthCalendarFixed { get; set; }
            public AdjustmentPay_16Days_Febrary FortnightPaymentDays { get; set; }
            public WeeklySeventhDay WeeklySeventhDay { get; set; }

            //Step 4
            public String CompanyInformation { get; set; }
            public String ComercialName { get; set; }
            public DateTime? CompanyCreationDate { get; set; }
            public CompanyScope CompanyScope { get; set; }
            public CompanyBusinessSector CompanyBusinessSector { get; set; }
            public String Facebook { get; set; }
            public String CompanyWebSite { get; set; }
            public String Instagram { get; set; }


            public Decimal NonDeductibleFactor { get; set; }
        }

    }
}
