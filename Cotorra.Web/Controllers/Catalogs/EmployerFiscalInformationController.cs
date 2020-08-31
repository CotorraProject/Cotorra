using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cotorra.Schema;
using Cotorra.Client;
using System.Linq;
using Cotorra.Web.Utils;
using Cotorra.Core.Utils;
using CotorraNode.Common.Service.Provisioning.API.DependencesDTO;

namespace Cotorra.Web.Controllers.Catalogs
{
    public class EmployerFiscalInformationController : Controller
    {
        #region "Attributes"
        private readonly Client<Cotorra.Schema.EmployerFiscalInformation> clientFiscal;
        private readonly Client<Cotorra.Schema.PayrollCompanyConfiguration> clientPayrollConfiguration;
        #endregion

        #region "Constructor"
        public EmployerFiscalInformationController()
        {
            SessionModel.Initialize();
            clientFiscal = new Cotorra.Client.Client<Cotorra.Schema.EmployerFiscalInformation>(SessionModel.AuthorizationHeader
                   , clientadapter: ClientConfiguration.GetAdapterFromConfig());
            clientPayrollConfiguration = new Cotorra.Client.Client<Cotorra.Schema.PayrollCompanyConfiguration>(SessionModel.AuthorizationHeader
                   , clientadapter: ClientConfiguration.GetAdapterFromConfig());
        }
        #endregion

        #region "Private Methods"
        private RequiredData getRequireData()
        {
            var requiredData = new RequiredData();
            requiredData.Ps = SessionModel.SessionID.ToString().ToLower().Replace("-", "");
            requiredData.IV = SessionModel.IdentityID.ToString().ToLower().Replace("-", "");
            requiredData.Salt = SessionModel.InstanceID.ToString().ToLower().Replace("-", "").Substring(0, 19);
            return requiredData;
        }

        private async Task<EmployerFiscalInformationData> SaveFiscalInformationAsync(EmployerFiscalInformationData data)
        {
            //Decrypt cer and key files
            try
            {
                var employerFiscalInformations = new List<EmployerFiscalInformation>();

                var digitalSign = new DigitalSign.DigitalSign();
                var requiredData = getRequireData();
                var clsCrypto = new ClsCrypto(requiredData.Ps, requiredData.IV, requiredData.Salt);

                data.cfb = clsCrypto.Decrypt(data.cfb).Replace("data:application/x-x509-ca-cert;base64,", String.Empty).Replace("data:application/octet-stream;base64,",String.Empty);
                data.kfb = clsCrypto.Decrypt(data.kfb).Replace("data:application/x-x509-ca-cert;base64,", String.Empty).Replace("data:application/octet-stream;base64,", String.Empty);

                if (!String.IsNullOrEmpty(data.cfb) && !String.IsNullOrEmpty(data.kfb))
                {
                    var fileCER = Convert.FromBase64String(data.cfb);
                    var fileKEY = Convert.FromBase64String(data.kfb);

                    //validate Cer and Key
                    digitalSign.validateCerKEYContent(fileCER, fileKEY, data.pfb, data.RFC);

                   var clsCryptoToCreate = new ClsCrypto(
                        SessionModel.CompanyID.ToString().ToLower().Replace("-", ""),
                        SessionModel.InstanceID.ToString().ToLower().Replace("-", ""),
                        SessionModel.InstanceID.ToString().ToLower().Replace("-", "").Substring(0, 19));

                    var certificateCER = clsCryptoToCreate.Encrypt(data.cfb);
                    var certificateKEY = clsCryptoToCreate.Encrypt(data.kfb);
                    var certificatePwd = StringCipher.Encrypt(data.pfb);

                    //Certificate number
                    data.CertificateNumber = digitalSign.GetCertificateNumber(fileCER);

                    //expiration date
                    var resultExpirationDate = digitalSign.GetExpirationDate(fileCER);
                    data.StartDate = resultExpirationDate.Item1.ToShortDateString();
                    data.ExpirationDate = resultExpirationDate.Item2.ToShortDateString();

                    employerFiscalInformations.Add(new EmployerFiscalInformation()
                    {
                        ID = data.ID == null ? Guid.NewGuid() : (Guid)data.ID,
                        Active = true,
                        company = SessionModel.CompanyID,
                        Timestamp = DateTime.UtcNow,
                        InstanceID = SessionModel.InstanceID,
                        Description = $"InfoFiscal de {data.RFC}",
                        CreationDate = DateTime.UtcNow,
                        user = SessionModel.IdentityID,
                        Name = "InfoFiscal",
                        StatusID = 1,
                        RFC = data.RFC,
                        CertificateCER = certificateCER,
                        CertificateKEY = certificateKEY,
                        CertificatePwd = certificatePwd,
                        CertificateNumber = data.CertificateNumber,
                        StartDate = resultExpirationDate.Item1,
                        EndDate = resultExpirationDate.Item2
                    });

                    if (data.ID != null)
                    {
                        await clientFiscal.UpdateAsync(employerFiscalInformations, SessionModel.CompanyID);
                    }
                    else
                    {
                        await clientFiscal.CreateAsync(employerFiscalInformations, SessionModel.CompanyID);
                    }

                    data.ID = employerFiscalInformations.FirstOrDefault().ID;
                }
                else
                {
                    throw new CotorraException(3012, "3012", "Es necesario especificar el certificado y el key.", null);
                }
            }
            catch (Exception ex)
            {
                throw new CotorraException(3011, "3011", $"Ocurrió un error al cargar el certificado: {ex.Message}", ex);
            }

            return data;
        }
        #endregion

        #region "Controller Methods"
        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetRequiredData()
        {
            RequiredData requiredData = null;

            await Task.Run(() =>
            {
                requiredData = getRequireData();
            });

            return Json(requiredData);
        }


        /// <summary>
        /// Obtiene el RFC de la empresa
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetRFC()
        {
            string rfc = null;
            var payrollCompanyConfigurations = await clientPayrollConfiguration.GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID);
            rfc = payrollCompanyConfigurations.FirstOrDefault().RFC;

            return Json(rfc);
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            List<EmployerFiscalInformationData> response;

            //get all register information
            var result = await clientFiscal
                .GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID);
            //set null this data
            result.ForEach(p => { p.CertificateCER = null; p.CertificateKEY = null; p.CertificatePwd = null; });

            var payrollCompanyConfigurations = await clientPayrollConfiguration.GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID);

            response = result.Select(x =>
                new EmployerFiscalInformationData
                {
                    ID = x.ID,
                    RFC = payrollCompanyConfigurations.FirstOrDefault().RFC,
                    CertificateNumber = x.CertificateNumber,
                    ExpirationDate = x.EndDate.ToShortDateString(),
                    StartDate = x.StartDate.ToShortDateString(),
                    IsCertificateConfigured = true,
                })
                .OrderBy(x => x.StartDate)
                .ToList();


            return Json(response);
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(EmployerFiscalInformationData data)
        {
            data = await SaveFiscalInformationAsync(data);
            return Json(data);
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            await clientFiscal.DeleteAsync(new List<Guid>() { id }, SessionModel.CompanyID);
            return Json("OK");
        }
        #endregion
    }

    #region "Models"
    public class EmployerFiscalInformationData
    {
        public Guid? ID { get; set; }

        public string RFC { get; set; }

        public bool IsCertificateConfigured { get; set; }

        public string CertificateNumber { get; set; }

        public string StartDate { get; set; }
        public string ExpirationDate { get; set; }

        public String cfb { get; set; }

        public String kfb { get; set; }

        public String pfb { get; set; }
    }

    public class RequiredData
    {
        public string IV { get; set; }
        public string Ps { get; set; }
        public string Salt { get; set; }
    }
    #endregion
}
