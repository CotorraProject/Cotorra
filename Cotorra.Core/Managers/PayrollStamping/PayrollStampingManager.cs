using AutoMapper;
using Cotorra.Core.Managers;
using Cotorra.Core.Managers.FiscalStamping;
using Cotorra.Core.Managers.FiscalValidator;
using Cotorra.Core.Utils;
using Cotorra.Core.Utils.Mail;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cotorra.Core.Utils;
using System.Collections.Concurrent;

namespace Cotorra.Core
{
    public class PayrollStampingManager
    {
        private readonly IMapper _mapper;
        private const string STORED_PROCEDURE_SAVEOVERDRAFT = "StampOverdraft";
        /*
         * http://omawww.sat.gob.mx/tramitesyservicios/Paginas/documentos/guianomina12_3_3.pdf
         * https://contadormx.com/2016/10/04/descarga-catalogo-conceptos-cfdi-nominas-partir-2017/
         xsd.exe C:\Projects\Cotorra\Cotorra.Core\Files\catNomina.xsd C:\Projects\Cotorra\Cotorra.Core\Files\tdCFDI.xsd C:\Projects\Cotorra\Cotorra.Core\Files\catCFDI.xsd C:\Projects\Cotorra\Cotorra.Core\Files\nomina12.xsd /classes /language:CS /out:C:\Projects\Cotorra\Cotorra.Core\Files\
         xsd.exe C:\Projects\Cotorra\Cotorra.Core\Files\tdCFDI.xsd C:\Projects\Cotorra\Cotorra.Core\Files\catCFDI.xsd C:\Projects\Cotorra\Cotorra.Core\Files\cfdv33.xsd /classes /language:CS /out:C:\Projects\Cotorra\Cotorra.Core\Files\
        */
        public PayrollStampingManager()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PayrollIndividualStampingParams, PayrollStampingParams>();
            });

            _mapper = config.CreateMapper();

        }
        private async Task sendMailAsync(Overdraft overdraftToStamp, Guid UUID, string XML, byte[] PDF, ISendMailProvider sendMailProvider,
            List<PayrollCompanyConfiguration> payrollCompanyConfigurations)
        {
            if (!String.IsNullOrEmpty(overdraftToStamp.HistoricEmployee.Email))
            {
                var message = new StringBuilder();
                message.Append($"Estimado(a) {overdraftToStamp.HistoricEmployee.FullName},");
                message.AppendLine();
                message.AppendLine("Por parte de tu empresa en la que estas colaborando te mandamos los datos de tu recibo de nómina.");
                message.AppendLine($"Periodo: <strong>{overdraftToStamp.HistoricEmployee.PeriodTypeDescription} {overdraftToStamp.PeriodDetail.InitialDate.ToShortDateString()} al {overdraftToStamp.PeriodDetail.FinalDate.ToShortDateString()}.</strong>");
                message.AppendLine($"RFC Emisor: <strong>{payrollCompanyConfigurations.FirstOrDefault().RFC}</strong>");
                message.AppendLine($"Razón Social: <strong>{payrollCompanyConfigurations.FirstOrDefault().SocialReason}</strong>");
                message.AppendLine($"UUID: <strong>{UUID}</strong>");

                var sendindParams = new SendMailParams()
                {
                    HTMLContent = message.ToString(),
                    SendMailAddresses = new List<SendMailAddress>()
                        {
                                    new SendMailAddress()
                                    {
                                        Email = overdraftToStamp.HistoricEmployee.Email,
                                        Name = overdraftToStamp.HistoricEmployee.FullName
                                    }
                        },
                    PlainContentText = message.ToString(),
                    SendMailAttachments = new List<SendMailAttachment>()
                            {
                                        new SendMailAttachment()
                                        {
                                            Attachment = Encoding.UTF8.GetBytes(XML),
                                            Filename = $"{UUID}.xml",
                                            TypeAttachment = TypeAttachment.XML
                                        },
                            },
                    Subject = "Cotorria - Envío de recibo de nómina",
                };

                if (PDF != null)
                {
                    sendindParams.SendMailAttachments.Add(new SendMailAttachment()
                    {
                        Attachment = PDF,
                        Filename = $"{UUID}.pdf",
                        TypeAttachment = TypeAttachment.PDF
                    });
                }

                await sendMailProvider.SendMailAsync(sendindParams);
            }
        }

        private async Task saveOverdraftStampedAsync(PayrollStampingParams payrollStampingParams, 
            PayrollStampingResult payrollStampingResult)
        {
            var details = payrollStampingResult.PayrollStampingResultDetails
                        .Where(p => p.PayrollStampingResultStatus == PayrollStampingResultStatus.Success);
            
            using (var connection = new SqlConnection(ConnectionManager.ConfigConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = STORED_PROCEDURE_SAVEOVERDRAFT;

                    //PeriodDetails Parameter
                    var periodDetailIds = details
                        .Select(p => p.PeriodDetailID).ToList();
                    if (periodDetailIds.Any())
                    {
                        DataTable dtGuidList = new DataTable();
                        dtGuidList.Columns.Add("ID", typeof(string));
                        periodDetailIds.ForEach(p =>
                        {
                            dtGuidList.Rows.Add(p);
                        });
                        SqlParameter param = new SqlParameter("@PeriodDetailIds", SqlDbType.Structured)
                        {
                            TypeName = "dbo.guidlisttabletype",
                            Value = dtGuidList
                        };
                        command.Parameters.Add(param);

                        //OverdraftIds Parameter
                        var overdraftIds = details
                            .Select(p => new { p.OverdraftID, p.UUID }).ToList();
                        DataTable dtOverdraftGuidList = new DataTable();
                        dtOverdraftGuidList.Columns.Add("ID", typeof(string));
                        dtOverdraftGuidList.Columns.Add("UUID", typeof(string));
                        overdraftIds.ForEach(p =>
                        {
                            dtOverdraftGuidList.Rows.Add(p.OverdraftID, p.UUID);
                        });
                        SqlParameter paramOverdraftIdsTable = new SqlParameter("@OverdraftIds", SqlDbType.Structured)
                        {
                            TypeName = "dbo.stampoverdrafttabletype",
                            Value = dtOverdraftGuidList
                        };
                        command.Parameters.Add(paramOverdraftIdsTable);

                        command.Parameters.AddWithValue("@InstanceId", payrollStampingParams.InstanceID);
                        command.Parameters.AddWithValue("@company", payrollStampingParams.IdentityWorkID);
                        command.Parameters.AddWithValue("@user", payrollStampingParams.user);

                        //Execute SP de autorización
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        private async Task<PreviewTransformResult> xmlTransformationAsync(PayrollStampingParams payrollStampingParams, Overdraft overdraftToStamp, string XML)
        {
            var fiscalPreviewManager = new FiscalPreviewerManager();
            var previewTransformParams = new PreviewTransformParams();
            previewTransformParams.FiscalStampingVersion = payrollStampingParams.FiscalStampingVersion;
            previewTransformParams.InstanceID = payrollStampingParams.InstanceID;
            previewTransformParams.IdentityWorkID = payrollStampingParams.IdentityWorkID;

            previewTransformParams.PreviewTransformParamsDetails.Add(new PreviewTransformParamsDetail()
            {
                OverdraftID = overdraftToStamp.ID,
                Overdraft = overdraftToStamp,
                XML = XML
            });

            var resultTransformation = await fiscalPreviewManager.TransformAsync(previewTransformParams);
            return resultTransformation;
        }

        private (byte[], byte[], string) Crypto(PayrollStampingParams payrollStampingParams, string certificateCER,
            string certificateKey, string certPassword)
        {
            var clsCryptoToCreate = new ClsCrypto(
                    payrollStampingParams.IdentityWorkID.ToString().ToLower().Replace("-", ""),
                    payrollStampingParams.InstanceID.ToString().ToLower().Replace("-", ""),
                    payrollStampingParams.InstanceID.ToString().ToLower().Replace("-", "").Substring(0, 19));
            var certificatebytesCER = Convert.FromBase64String(clsCryptoToCreate.Decrypt(certificateCER));
            var certificatebytesKEY = Convert.FromBase64String(clsCryptoToCreate.Decrypt(certificateKey));
            var certPasswordToResult = StringCipher.Decrypt(certPassword);

            return (certificatebytesCER, certificatebytesKEY, certPasswordToResult);
        }

        private async Task fireAndForgetAsync(PayrollStampingParams payrollStampingParams,
            Overdraft overdraftToStamp,
            List<PayrollCompanyConfiguration> payrollConfigurations,
            BlobStorageUtil blobStorageUtil,
            ISendMailProvider sendMailProvider,
            Guid UUID, string XML)
        {
            try
            {
                //7. Save XML to BlobStorage
                await blobStorageUtil.UploadDocumentAsync($"{UUID}.xml", XML);

                ////8. Transforma el XML a HTML y PDF
                var resultTransformation = await xmlTransformationAsync(payrollStampingParams, overdraftToStamp,
                    XML);

                ////Save PDF to BlobStorage
                await blobStorageUtil.UploadDocumentAsync($"{UUID}.pdf",
                    resultTransformation.PreviewTransformResultDetails.FirstOrDefault().TransformPDFResult);

                ////9. Send email with the payroll to Employee
                await sendMailAsync(overdraftToStamp, UUID, XML,
                    resultTransformation.PreviewTransformResultDetails.FirstOrDefault().TransformPDFResult, sendMailProvider,
                    payrollConfigurations);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"No fue posible transformar / enviar el comprobante al colaborador. {ex.Message}");
            }
        }

        private async Task<List<PayrollStampingResultDetail>> doWorkAsync(Overdraft overdraftToStamp, RoundUtil roundUtil,
            ZipCodeManager zipCodeManager, PayrollStampingParams payrollStampingParams,
            BlobStorageUtil blobStorageUtil,
            ISendMailProvider sendMailProvider,
            List<Incident> incidents,
            List<Inhability> inhabilities,
            List<EmployerFiscalInformation> employerFiscalInformations,
            List<PayrollCompanyConfiguration> payrollConfigurations)
        {
            var payrollStampingResultDetail = new PayrollStampingResultDetail();
            var payrollStampingResultDetails = new List<PayrollStampingResultDetail>();

            //Obtiene los xmls de los comprobantes segun la version de CFDI especificada
            IFiscalStamping fiscalStamping = FiscalStampingFactory.CreateInstance(payrollStampingParams.FiscalStampingVersion);

            //1. Get totals                
            var overdraftResults = new OverdraftManager().GetTotals(overdraftToStamp, roundUtil);

            //1.1 Datetime for zipCode
            (var zipcode, var datetimeFromZipCode) = await zipCodeManager.GetZipCode(overdraftToStamp, payrollConfigurations.FirstOrDefault());

            //2. Get XML - Creates comprobante
            var payrolllStampingDetail = payrollStampingParams.Detail
                .FirstOrDefault(detail => detail.OverdraftID == overdraftToStamp.ID);
            ICFDINomProvider cfdi = null;
            try
            {
                cfdi = fiscalStamping.CreateComprobante(new CreateComprobanteParams()
                {
                    PayrollStampingDetail = payrolllStampingDetail,
                    PayrollStampingParams = payrollStampingParams,
                    Overdraft = overdraftToStamp,
                    OverdraftResults = overdraftResults,
                    PayrollCompanyConfiguration = payrollConfigurations.FirstOrDefault(),
                    CFDIDateTimeStamp = datetimeFromZipCode,
                    ZipCode = zipcode,
                    RoundUtil = roundUtil,
                    Incidents = incidents,
                    Inhabilities = inhabilities
                });
            }
            catch(Exception ex)
            {
                //Errores en validaciónes de armado / fiscales
                payrollStampingResultDetail.Message = ex.Message;
                payrollStampingResultDetail.PayrollStampingResultStatus = PayrollStampingResultStatus.Fail;
                payrollStampingResultDetail.HistoricEmployeeID = overdraftToStamp.HistoricEmployeeID.Value;
                payrollStampingResultDetail.EmployeeID = overdraftToStamp.HistoricEmployee.EmployeeID;
                payrollStampingResultDetail.OverdraftID = overdraftToStamp.ID;
                payrollStampingResultDetail.Overdraft = overdraftToStamp;
                payrollStampingResultDetail.PeriodDetailID = overdraftToStamp.PeriodDetailID;
                payrollStampingResultDetail.PeriodDetail = overdraftToStamp.PeriodDetail;

                payrollStampingResultDetails.Add(payrollStampingResultDetail);
                return payrollStampingResultDetails;
            }

            //3. Sign XML
            var certificateCER = employerFiscalInformations.FirstOrDefault().CertificateCER;
            var certificateKey = employerFiscalInformations.FirstOrDefault().CertificateKEY;
            var certPassword = employerFiscalInformations.FirstOrDefault().CertificatePwd;

            //Decrypt 
            (var certificatebytesCER, var certificatebytesKEY, var certPasswordResult) = Crypto(payrollStampingParams, certificateCER, certificateKey, certPassword);

            var stampingResult = fiscalStamping.SignDocument(cfdi, certificatebytesCER, certificatebytesKEY, certPasswordResult);

            //Set the employer
            stampingResult.EmployerRFC = payrollConfigurations.FirstOrDefault().RFC;

            //4. Stamp XML
            stampingResult = await fiscalStamping.StampDocumetAsync(stampingResult);

            if (stampingResult.WithErrors)
            {
                //error en el timbrado
                var errrorMessage = $"\nPara el empleado <strong>'{overdraftToStamp.HistoricEmployee.FullName}'</strong> encontramos los siguientes errores de timbrado: '{stampingResult.Details}'";
                payrollStampingResultDetail.Message = errrorMessage;
                payrollStampingResultDetail.PayrollStampingResultStatus = PayrollStampingResultStatus.Fail;

            }
            else
            {
                //5. Return the complete XML
                stampingResult.XML = fiscalStamping.CreateXml<ICFDINomProvider>(stampingResult.CFDI, true);

                //5.5 Fill the result data     
                payrollStampingResultDetail.Message = String.Empty;
                payrollStampingResultDetail.UUID = stampingResult.UUID;
                payrollStampingResultDetail.XML = stampingResult.XML;
                payrollStampingResultDetail.PayrollStampingResultStatus = PayrollStampingResultStatus.Success;

                //6. Fill the result data   
                overdraftToStamp.UUID = stampingResult.UUID;
                overdraftToStamp.OverdraftStatus = OverdraftStatus.Stamped;

                //Fire and forget convertion and sending email
                fireAndForgetAsync(payrollStampingParams, overdraftToStamp, payrollConfigurations,
                    blobStorageUtil, sendMailProvider, payrollStampingResultDetail.UUID, payrollStampingResultDetail.XML);

                //Fill the result object
                payrollStampingResultDetail.HistoricEmployeeID = overdraftToStamp.HistoricEmployeeID.Value;
                payrollStampingResultDetail.EmployeeID = overdraftToStamp.HistoricEmployee.EmployeeID;
                payrollStampingResultDetail.OverdraftID = overdraftToStamp.ID;
                payrollStampingResultDetail.Overdraft = overdraftToStamp;
                payrollStampingResultDetail.PeriodDetailID = overdraftToStamp.PeriodDetailID;
                payrollStampingResultDetail.PeriodDetail = overdraftToStamp.PeriodDetail;
            }


            payrollStampingResultDetails.Add(payrollStampingResultDetail);

            return payrollStampingResultDetails;
        }

        private async Task<PayrollStampingResult> payrollStampingCoreAsync(PayrollStampingParams payrollStampingParams,
            List<Overdraft> historicOverdraftsToStamp,
            List<Incident> incidents,
            List<Inhability> inhabilities,
            List<EmployerFiscalInformation> employerFiscalInformations,
            List<PayrollCompanyConfiguration> payrollConfigurations)
        {
            var payrollStampingResult = new PayrollStampingResult();

            List<string> zipCodesToFind =
            historicOverdraftsToStamp.Select(p => p.HistoricEmployee.EmployerRegistrationZipCode).ToList();
            zipCodesToFind.AddRange(payrollConfigurations.Select(p => p.Address?.ZipCode));

            //Obtener los zipCodes
            var zipCodeMiddlewareManager = new MiddlewareManager<catCFDI_CodigoPostal>(
                new BaseRecordManager<catCFDI_CodigoPostal>(),
               new catCFDI_CodigoPostalValidator());
            var zipCodes = await zipCodeMiddlewareManager.FindByExpressionAsync(p =>
                    zipCodesToFind.Contains(p.c_CodigoPostal)
                , payrollStampingParams.IdentityWorkID);

            //Round for currency     
            var roundUtil = new RoundUtil(payrollStampingParams.Currency.ToString());

            //Zip Code manager
            var zipCodeManager = new ZipCodeManager(zipCodes);

            //Blob Storage Util
            var blobStorageUtil = new BlobStorageUtil(payrollStampingParams.InstanceID);
            await blobStorageUtil.InitializeAsync();

            ISendMailProvider sendMailProvider = FactoryMailProvider.CreateInstance(SendMailProvider.SendGrid);

            var tasks = new List<Task<List<PayrollStampingResultDetail>>>();
            foreach (var overdraftToStamp in historicOverdraftsToStamp)
            {
                tasks.Add(doWorkAsync(overdraftToStamp, roundUtil, zipCodeManager,
                    payrollStampingParams, blobStorageUtil, sendMailProvider, incidents,
                    inhabilities, employerFiscalInformations, payrollConfigurations));
            }

            ConcurrentBag<PayrollStampingResultDetail> payrollStampingDetails = new ConcurrentBag<PayrollStampingResultDetail>();
            foreach (var task in await Task.WhenAll(tasks))
            {
                foreach (var insideTask in task)
                {
                    payrollStampingDetails.Add(insideTask);
                }
            }
            payrollStampingResult.PayrollStampingResultDetails = payrollStampingDetails.ToList();

            //Update DB indicate that Overdraft was stamped correctly / PeriodDetail
            await saveOverdraftStampedAsync(payrollStampingParams, payrollStampingResult);

            //Errors preparation
            if (payrollStampingResult.PayrollStampingResultDetails.Any(p => p.PayrollStampingResultStatus == PayrollStampingResultStatus.Fail))
            {
                var errorMessages = payrollStampingResult.PayrollStampingResultDetails.Select(p => p.Message);
                throw new CotorraException(109, "109", string.Join("\n", errorMessages), null);
            }

            return payrollStampingResult;
        }

        public async Task<PayrollStampingResult> PayrollStampingAsync(PayrollStampingParams payrollStampingParams)
        {
            Trace.WriteLine("Llega al PayrollStampingAsync");
            //Obtiene todos los historic overdrafts del periodo seleccionado
            var historicOverdraftManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(),
                new OverdraftValidator());
            var incidentsManager = new MiddlewareManager<Incident>(new BaseRecordManager<Incident>(),
                new IncidentValidator());
            var inhabilitiesManager = new MiddlewareManager<Inhability>(
                new BaseRecordManager<Inhability>(),
               new InhabilityValidator());
            List<Overdraft> historicOverdraftsToStamp = null;
            List<Incident> incidents = null;
            List<Inhability> inhabilities = null;
            List<EmployerFiscalInformation> employerFiscalInformations = null;
            List<PayrollCompanyConfiguration> payrollConfigurations = null;

            if (payrollStampingParams.Detail.Any())
            {
                var overdraftsIds = payrollStampingParams.Detail.Select(p => p.OverdraftID);
                historicOverdraftsToStamp = await historicOverdraftManager.FindByExpressionAsync(p =>
                    p.company == payrollStampingParams.IdentityWorkID &&
                    p.InstanceID == payrollStampingParams.InstanceID &&
                    p.PeriodDetailID == payrollStampingParams.PeriodDetailID &&
                    overdraftsIds.Contains(p.ID) &&
                    p.OverdraftStatus == OverdraftStatus.Authorized &&
                    p.Active,
                    payrollStampingParams.IdentityWorkID, new string[] {
                    "OverdraftPreviousCancelRelationship",
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "HistoricEmployee",
                    "HistoricEmployee.Employee",
                    "HistoricEmployee.Employee.Workshift",
                    "HistoricEmployee.Employee.EmployerRegistration"
                 });

                incidents = await incidentsManager.FindByExpressionAsync(p => p.PeriodDetailID == payrollStampingParams.PeriodDetailID
                        && p.InstanceID == payrollStampingParams.InstanceID,
                    payrollStampingParams.IdentityWorkID,
                    new string[] { "IncidentType" });

                var initialDate = historicOverdraftsToStamp.FirstOrDefault().PeriodDetail.InitialDate;
                var finalDate = historicOverdraftsToStamp.FirstOrDefault().PeriodDetail.FinalDate;

                //Incapacidades dentro del periodo
                inhabilities = await inhabilitiesManager.FindByExpressionAsync(p =>
                p.InstanceID == payrollStampingParams.InstanceID &&
                    (
                    (p.InitialDate >= initialDate && p.InitialDate.AddDays(p.AuthorizedDays - 1) <= finalDate) ||
                    (p.InitialDate >= initialDate && p.InitialDate.AddDays(p.AuthorizedDays - 1) > finalDate) ||
                    (p.InitialDate < initialDate && p.InitialDate.AddDays(p.AuthorizedDays - 1) <= finalDate) ||
                    (p.InitialDate < initialDate && p.InitialDate.AddDays(p.AuthorizedDays - 1) > finalDate)
                    ),
                payrollStampingParams.IdentityWorkID);

                //Obtiene la información fiscal del patrón, certificados
                var employerFiscalInformationManager = new MiddlewareManager<EmployerFiscalInformation>(new BaseRecordManager<EmployerFiscalInformation>(),
                    new EmployerFiscalInformationValidator());
                employerFiscalInformations = await employerFiscalInformationManager.FindByExpressionAsync(p =>
                    p.company == payrollStampingParams.IdentityWorkID &&
                    p.InstanceID == payrollStampingParams.InstanceID,
                    payrollStampingParams.IdentityWorkID);

                if (!employerFiscalInformations.Any())
                {
                    throw new CotorraException(105, "105", "No se han configurado los certificados (CSD) de la empresa para poder timbrar. Ve al menú Catálogos -> Certificados -> Agregar nuevo", null);
                }

                //Obtiene la configuración general de la empresa
                var payrollCompanyConfigurationManager = new MiddlewareManager<PayrollCompanyConfiguration>(new BaseRecordManager<PayrollCompanyConfiguration>(),
                    new PayrollCompanyConfigurationValidator());
                payrollConfigurations = await payrollCompanyConfigurationManager.FindByExpressionAsync(p => p.company == payrollStampingParams.IdentityWorkID &&
                    p.InstanceID == payrollStampingParams.InstanceID, payrollStampingParams.IdentityWorkID,
                    new string[] { "Address" });
            }
            else
            {
                throw new CotorraException(101, "101", "No se proporcionó los detalles correspondientes para timbrar.", null);
            }

            if (!historicOverdraftsToStamp.Any())
            {
                throw new CotorraException(101, "101", "No hay ningún sobrerecibo a timbrar en estatus autorizado, con los parámetros proporcionados.", null);
            }

            return await payrollStampingCoreAsync(payrollStampingParams,
                historicOverdraftsToStamp, incidents, inhabilities, employerFiscalInformations,
                payrollConfigurations);
        }

        public async Task<PayrollStampingResult> PayrollIndividualStampingAsync(PayrollIndividualStampingParams payrollIndividualStampingParams)
        {
            //Obtiene todos los historic overdrafts del periodo seleccionado
            var historicOverdraftManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(),
                new OverdraftValidator());
            var incidentsManager = new MiddlewareManager<Incident>(new BaseRecordManager<Incident>(),
               new IncidentValidator());
            var inhabilitiesManager = new MiddlewareManager<Inhability>(
             new BaseRecordManager<Inhability>(),
            new InhabilityValidator());

            var historicOverdraftsToStamp = await historicOverdraftManager.FindByExpressionAsync(p =>
                p.company == payrollIndividualStampingParams.IdentityWorkID &&
                p.InstanceID == payrollIndividualStampingParams.InstanceID &&
                p.PeriodDetailID == payrollIndividualStampingParams.PeriodDetailID &&
                p.ID == payrollIndividualStampingParams.OverdraftID &&
                p.OverdraftStatus == OverdraftStatus.Authorized &&
                p.Active,
                payrollIndividualStampingParams.IdentityWorkID, new string[] {
                    "OverdraftPreviousCancelRelationship",
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "HistoricEmployee",
                    "HistoricEmployee.Employee",
                     "HistoricEmployee.Employee.Workshift",
                    "HistoricEmployee.Employee.EmployerRegistration"
                     });

            var incidents = await incidentsManager.FindByExpressionAsync(p =>
                    p.PeriodDetailID == payrollIndividualStampingParams.PeriodDetailID
                    && p.InstanceID == payrollIndividualStampingParams.InstanceID,
                payrollIndividualStampingParams.IdentityWorkID,
                new string[] { "IncidentType" });

            var initialDate = historicOverdraftsToStamp.FirstOrDefault().PeriodDetail.InitialDate;
            var finalDate = historicOverdraftsToStamp.FirstOrDefault().PeriodDetail.FinalDate;

            //Incapacidades dentro del periodo
            var inhabilities = await inhabilitiesManager.FindByExpressionAsync(p =>
            p.InstanceID == payrollIndividualStampingParams.InstanceID &&
                (
                (p.InitialDate >= initialDate && p.InitialDate.AddDays(p.AuthorizedDays - 1) <= finalDate) ||
                (p.InitialDate >= initialDate && p.InitialDate.AddDays(p.AuthorizedDays - 1) > finalDate) ||
                (p.InitialDate < initialDate && p.InitialDate.AddDays(p.AuthorizedDays - 1) <= finalDate) ||
                (p.InitialDate < initialDate && p.InitialDate.AddDays(p.AuthorizedDays - 1) > finalDate)
                ),
            payrollIndividualStampingParams.IdentityWorkID);

            //Obtiene la información fiscal del patrón, certificados
            var employerFiscalInformationManager = new MiddlewareManager<EmployerFiscalInformation>(new BaseRecordManager<EmployerFiscalInformation>(),
                new EmployerFiscalInformationValidator());
            var employerFiscalInformations = await employerFiscalInformationManager.FindByExpressionAsync(p =>
                p.company == payrollIndividualStampingParams.IdentityWorkID &&
                p.InstanceID == payrollIndividualStampingParams.InstanceID,
                payrollIndividualStampingParams.IdentityWorkID);

            if (!employerFiscalInformations.Any())
            {
                throw new CotorraException(105, "105", "No se han configurado los certificados (CSD) de la empresa para poder timbrar. Ve al menú Catálogos -> Certificados -> Agregar nuevo", null);
            }

            //Obtiene la configuración general de la empresa
            var payrollCompanyConfigurationManager = new MiddlewareManager<PayrollCompanyConfiguration>(new BaseRecordManager<PayrollCompanyConfiguration>(),
                new PayrollCompanyConfigurationValidator());
            var payrollConfigurations = await payrollCompanyConfigurationManager.FindByExpressionAsync(p =>
                p.company == payrollIndividualStampingParams.IdentityWorkID &&
                p.InstanceID == payrollIndividualStampingParams.InstanceID, payrollIndividualStampingParams.IdentityWorkID,
                new string[] { "Address" });

            if (!historicOverdraftsToStamp.Any())
            {
                throw new CotorraException(101, "101", "No hay ningún sobrerecibo a timbrar en estatus autorizado, con los parámetros proporcionados.", null);
            }

            var payrollStampingParams = _mapper.Map<PayrollIndividualStampingParams, PayrollStampingParams>(payrollIndividualStampingParams);
            return await payrollStampingCoreAsync(payrollStampingParams,
                historicOverdraftsToStamp, incidents, inhabilities,
                employerFiscalInformations, payrollConfigurations);
        }

    }
}
