using Cotorra.Core.Managers.FiscalStamping;
using Cotorra.Core.Utils;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Org.BouncyCastle.Asn1.IsisMtt.X509;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core
{
    public class CancelStampingManager
    {
        /*https://www.sat.gob.mx/consultas/91447/nuevo-esquema-de-cancelacion*/
        public CancelStampingManager()
        {

        }

        public class CancelationStoreProcedureParams
        {
            public Guid OverdraftID { get; set; }

            public CancelationFiscalDocumentStatus CancelationFiscalDocumentStatus { get; set; }
        }

        private (byte[], byte[], string) Crypto(CancelPayrollStampingParams cancelPayrollStampingParams, string certificateCER,
          string certificateKey, string certPassword)
        {
            var clsCryptoToCreate = new ClsCrypto(
                    cancelPayrollStampingParams.IdentityWorkID.ToString().ToLower().Replace("-", ""),
                    cancelPayrollStampingParams.InstanceID.ToString().ToLower().Replace("-", ""),
                    cancelPayrollStampingParams.InstanceID.ToString().ToLower().Replace("-", "").Substring(0, 19));
            var certificatebytesCER = Convert.FromBase64String(clsCryptoToCreate.Decrypt(certificateCER));
            var certificatebytesKEY = Convert.FromBase64String(clsCryptoToCreate.Decrypt(certificateKey));
            var certPasswordToResult = StringCipher.Decrypt(certPassword);

            return (certificatebytesCER, certificatebytesKEY, certPasswordToResult);
        }

        private async Task saveCancelFiscalStamping(CancelPayrollStampingParams cancelPayrollStampingParams,
            Guid cancelationRequestXMLID,
            Guid cancelationResponseXMLID,
           List<CancelationStoreProcedureParams> cancelationStoreProcedureParams)
        {
            using (var connection = new SqlConnection(ConnectionManager.ConfigConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "CreateCancelationStamp";

                    DataTable dtGuidList = new DataTable();
                    dtGuidList.Columns.Add("OverdraftID", typeof(string));
                    dtGuidList.Columns.Add("CancelationFiscalDocumentStatus", typeof(int));
                    cancelationStoreProcedureParams.ForEach(p =>
                    {
                        dtGuidList.Rows.Add(p.OverdraftID, (int)p.CancelationFiscalDocumentStatus);
                    });
                    SqlParameter param = new SqlParameter("@Cancelstampoverdraft", SqlDbType.Structured)
                    {
                        TypeName = "dbo.cancelstampoverdrafttabletype",
                        Value = dtGuidList
                    };
                    command.Parameters.Add(param);

                    command.Parameters.AddWithValue("@CancelationRequestXMLID", cancelationRequestXMLID);
                    command.Parameters.AddWithValue("@CancelationResponseXMLID", cancelationResponseXMLID);
                    command.Parameters.AddWithValue("@InstanceId", cancelPayrollStampingParams.InstanceID);
                    command.Parameters.AddWithValue("@company", cancelPayrollStampingParams.IdentityWorkID);
                    command.Parameters.AddWithValue("@user", cancelPayrollStampingParams.user);

                    //Execute SP de autorización
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<CancelPayrollStampingResult> CancelPayrollStampingAsync(CancelPayrollStampingParams cancelPayrollStampingParams)
        {
            //Create cancelation instance
            ICancelStamping cancelStamping = CancelStampingFactory.CreateInstance(cancelPayrollStampingParams.FiscalStampingVersion);

            //Get overdrafts and validate
            var overdraftMiddlewareManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(),
                new OverdraftValidator());
            var overdraftsIds = cancelPayrollStampingParams.OverdraftIDs;
            var overdraftsToCancel = await overdraftMiddlewareManager.FindByExpressionAsync(p =>
                p.company == cancelPayrollStampingParams.IdentityWorkID &&
                p.InstanceID == cancelPayrollStampingParams.InstanceID &&
                overdraftsIds.Contains(p.ID) &&
                p.OverdraftStatus == OverdraftStatus.Stamped &&
                p.Active,
                cancelPayrollStampingParams.IdentityWorkID);

            if (!overdraftsToCancel.Any())
            {
                throw new CotorraException(106, "106", "No existen recibos a cancelar con los datos proporcionados.", null);
            }

            //Get configuration company
            var payrollMiddlewareManager = new MiddlewareManager<PayrollCompanyConfiguration>(new BaseRecordManager<PayrollCompanyConfiguration>(),
                new PayrollCompanyConfigurationValidator());
            var payrollCompanyConfiguration = await payrollMiddlewareManager.FindByExpressionAsync(p => p.InstanceID == cancelPayrollStampingParams.InstanceID,
                cancelPayrollStampingParams.IdentityWorkID, new string[] { "Address" });

            if (!payrollCompanyConfiguration.Any())
            {
                throw new CotorraException(106, "106", "No existe configuración de la compañia.", null);
            }

            //Get configuration company
            var fiscalInformationManager = new MiddlewareManager<EmployerFiscalInformation>(new BaseRecordManager<EmployerFiscalInformation>(),
                new EmployerFiscalInformationValidator());
            var employerFiscalInformations = await fiscalInformationManager.FindByExpressionAsync(p =>
                    p.InstanceID == cancelPayrollStampingParams.InstanceID,
                cancelPayrollStampingParams.IdentityWorkID);

            if (!employerFiscalInformations.Any())
            {
                throw new CotorraException(106, "106", "No hay certificados válidos registrados para la compañia.", null);
            }

            //Zipcode Manager
            IEnumerable<string> zipCodesToFind = payrollCompanyConfiguration.Select(p => p.Address?.ZipCode);
            var zipCodeMiddlewareManager = new MiddlewareManager<catCFDI_CodigoPostal>(
             new BaseRecordManager<catCFDI_CodigoPostal>(),
            new catCFDI_CodigoPostalValidator());
            var zipCodes = await zipCodeMiddlewareManager.FindByExpressionAsync(p =>
                    zipCodesToFind.Contains(p.c_CodigoPostal)
                , cancelPayrollStampingParams.IdentityWorkID);

            if (!zipCodes.Any())
            {
                throw new CotorraException(106, "106", "No hay codigo postal registrado para la compañia.", null);
            }

            //UUIDs to Cancel
            var uuids = overdraftsToCancel.Select(p => p.UUID).ToList();

            //3. Sign XML
            var certificateCER = employerFiscalInformations.FirstOrDefault().CertificateCER;
            var certificateKey = employerFiscalInformations.FirstOrDefault().CertificateKEY;
            var certPassword = employerFiscalInformations.FirstOrDefault().CertificatePwd;

            //Decrypt and get certificate
            (var certificatebytesCER, var certificatebytesKEY, var certPasswordResult) = Crypto(cancelPayrollStampingParams,
                certificateCER, certificateKey, certPassword);

            //Cancel Document Details
            var cancelDocumentParamsDetails = new List<CancelDocumentParamsDetail>();
            uuids.ForEach(p =>
            {
                var cancelDetail = new CancelDocumentParamsDetail();
                cancelDetail.OverdraftID = overdraftsToCancel.FirstOrDefault(q => q.UUID == p).ID;
                cancelDetail.UUID = p;
                cancelDocumentParamsDetails.Add(cancelDetail);
            });

            CancelDocumentParams cancelDocumentParams = new CancelDocumentParams()
            {
                ZipCodes = zipCodes,
                IssuerZipCode = payrollCompanyConfiguration.FirstOrDefault().Address?.ZipCode,
                IssuerRFC = payrollCompanyConfiguration.FirstOrDefault().RFC,
                CertificateCER = certificatebytesCER,
                CertificateKey = certificatebytesKEY,
                Password = certPasswordResult,
                IdentityWorkID = cancelPayrollStampingParams.IdentityWorkID,
                InstanceID = cancelPayrollStampingParams.InstanceID,
                user = cancelPayrollStampingParams.user,
                CancelDocumentParamsDetails = cancelDocumentParamsDetails
            };

            //cancel cfdi document
            var result = await cancelStamping.CancelDocumetAsync(cancelDocumentParams);

            if (result.WithErrors)
            {
                throw new CotorraException(107, "107", $"Error al cancelar el recibo: {result.Message}", null);
            }

            //Fill Save Cancelation Object
            var cancelationRequestXMLID = Guid.NewGuid();
            var cancelationResponseXMLID = Guid.NewGuid();
            
            var cancelSPFiscalParams = new List<CancelationStoreProcedureParams>();
            result.CancelPayrollStampingResultDetails.ForEach(p => {
                var cancelationStoreProcedure = new CancelationStoreProcedureParams();
                cancelationStoreProcedure.OverdraftID = overdraftsToCancel.FirstOrDefault(q => q.UUID == p.UUID).ID;
                cancelationStoreProcedure.CancelationFiscalDocumentStatus = 
                p.PayrollStampingResultStatus == PayrollStampingResultStatus.Success ? 
                        CancelationFiscalDocumentStatus.Done : CancelationFiscalDocumentStatus.ErrorInRequest;
                cancelSPFiscalParams.Add(cancelationStoreProcedure);
            });

            //Database
            await saveCancelFiscalStamping(cancelPayrollStampingParams, cancelationRequestXMLID, cancelationResponseXMLID, cancelSPFiscalParams);

            //Save XML of cancelation  in blob (request)
            var blobStorageUtil = new BlobStorageUtil(cancelDocumentParams.InstanceID);
            await blobStorageUtil.InitializeAsync();
            await blobStorageUtil.UploadDocumentAsync($"{cancelationRequestXMLID}.xml", result.CancelacionXMLRequest);

            //Save XML of cancelation acknowledgement in blob (response)
            await blobStorageUtil.InitializeAsync();
            await blobStorageUtil.UploadDocumentAsync($"{cancelationResponseXMLID}.xml", result.CancelacionXMLAcknowledgeResponse);

            return result;
        }
    }
}
