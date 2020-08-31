using CotorraNode.Common.Config;
using Cotorra.Core.Utils;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Org.BouncyCastle.Asn1.IsisMtt.X509;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;

namespace Cotorra.Core.Managers.FiscalPreview
{
    public class FiscalPreviewCFDIv33Nom12 : FiscalPreviewBase, IFiscalPreview
    {
        public override string XsltPath { get; set; }
        public override string URLSAT { get; set; }

        private readonly string previewPath;

        #region "Constructor"tambi
        public FiscalPreviewCFDIv33Nom12()
        {
            previewPath = Path.Combine("fiscal", "cfdi33nom12", "preview");
            XsltPath = Path.Combine(DirectoryUtil.AssemblyDirectory, previewPath, "transformPayRollXMLToHTML33.xslt");
            URLSAT = ConfigManager.GetValue("QRURLSAT33");
        }

        private string GetLogoCotorra()
        {
            Byte[] imageBytes = File.ReadAllBytes(Path.Combine(DirectoryUtil.AssemblyDirectory, previewPath, "companyLogo.png"));
            var companyLogo = Convert.ToBase64String(imageBytes);
            return companyLogo;
        }
        #endregion

        public async Task<GetPreviewUrlResult> GetPreviewUrlByUUIDAsync(Guid instanceId, Guid UUID)
        {
            GetPreviewUrlResult result = new GetPreviewUrlResult();
            result.UUID = UUID;

            BlobStorageUtil blobManagerUtil = new BlobStorageUtil(instanceId);
            await blobManagerUtil.InitializeAsync();
            //check if XML exists
            if (await blobManagerUtil.ExistsFile($"{UUID.ToString()}.xml"))
            {
                result.XMLUri = blobManagerUtil.GetBlobSasUri(instanceId, $"{UUID.ToString()}.xml");

                //check if PDF exists
                if (!await blobManagerUtil.ExistsFile($"{UUID.ToString()}.pdf"))
                {
                    ////Transform 
                    var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(),
                        new OverdraftValidator());

                    var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                        p.InstanceID == instanceId && p.UUID == UUID, Guid.Empty);

                    var identityWorkId = overdrafts.FirstOrDefault().company;

                    var previewTransformParams = new PreviewTransformParams()
                    {
                        FiscalStampingVersion = FiscalStampingVersion.CFDI33_Nom12,
                        IdentityWorkID = identityWorkId,
                        InstanceID = instanceId,
                        PreviewTransformParamsDetails = new List<PreviewTransformParamsDetail>()
                        {
                            new PreviewTransformParamsDetail()
                            {
                                Overdraft = null,
                                OverdraftID = overdrafts.FirstOrDefault().ID
                            }
                        },
                    };
                    var trasformationResult = await TransformAsync(previewTransformParams);
                    var pdfResult = trasformationResult.PreviewTransformResultDetails.FirstOrDefault().TransformPDFResult;

                    //Upload
                    await blobManagerUtil.UploadDocumentAsync($"{UUID.ToString()}.pdf", pdfResult);
                }
            }
            else
            {
                throw new CotorraException(109, "109", "No fue posible recuperar el XML generado.", null);
            }

            result.PDFUri = blobManagerUtil.GetBlobSasUri(instanceId, $"{UUID.ToString()}.pdf");

            return result;
        }

        /// <summary>
        /// Obtiene las nóminas a transformar
        /// </summary>
        /// <param name="previewTransformParams"></param>
        /// <returns></returns>
        private async Task<List<Overdraft>> getDataAsync(PreviewTransformParams previewTransformParams)
        {
            List<Overdraft> overdrafts = null;

            var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(),
              new OverdraftValidator());

            if (null != previewTransformParams.PreviewTransformParamsDetails.FirstOrDefault().Overdraft)
            {
                overdrafts = new List<Overdraft>() { previewTransformParams.PreviewTransformParamsDetails.FirstOrDefault().Overdraft };
            }
            else
            {
                var overdraftsIds = previewTransformParams.PreviewTransformParamsDetails.Select(p => p.OverdraftID);
                overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.company == previewTransformParams.IdentityWorkID &&
                    p.InstanceID == previewTransformParams.InstanceID &&
                    overdraftsIds.Contains(p.ID) &&
                    p.Active,
                    previewTransformParams.IdentityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "HistoricEmployee",
                    "HistoricEmployee.Employee",
                    "HistoricEmployee.Employee.EmployerRegistration"
                   });
            }
            if (!overdrafts.Any())
            {
                throw new CotorraException(106, "106", "No se encontraron sobrerecibos con los datos proporcionados.", null);
            }

            return overdrafts;
        }

        private async Task<List<PayrollCompanyConfiguration>> getPayrollCompanyConfigurationAsync(PreviewTransformParams previewTransformParams)
        {
            var middlewarePayrollCompanyConfigurationManager = new MiddlewareManager<PayrollCompanyConfiguration>(new BaseRecordManager<PayrollCompanyConfiguration>(),
                 new PayrollCompanyConfigurationValidator());
            var payrollCompanyConfiguration = await middlewarePayrollCompanyConfigurationManager.FindByExpressionAsync(p=>
                p.InstanceID == previewTransformParams.InstanceID && p.Active,
                previewTransformParams.IdentityWorkID);
            return payrollCompanyConfiguration;
        }

        private async Task<Dictionary<Guid, string>> getXMLAsync(PreviewTransformParams previewTransformParams, List<Overdraft> overdrafts)
        {
            var xmls = new Dictionary<Guid, string>();

            foreach (var overdraft in overdrafts)
            {
                var fileName = $"{overdraft.UUID}.xml";
                var overdraftInParams = previewTransformParams.PreviewTransformParamsDetails.FirstOrDefault(p => p.OverdraftID == overdraft.ID);
                if (String.IsNullOrEmpty(overdraftInParams.XML))
                {
                    var blobUtil = new BlobStorageUtil(previewTransformParams.InstanceID);
                    await blobUtil.InitializeAsync();
                    var fileContent = await blobUtil.DownloadDocumentAsync(fileName);
                    xmls.TryAdd(overdraft.UUID, fileContent);
                }
                else
                {
                    xmls.TryAdd(overdraft.UUID, overdraftInParams.XML);
                }
            }
            return xmls;
        }

        private XsltArgumentList getXSLTArguments(PreviewTransformParams previewTransformParams, 
            List<Overdraft> overdrafts, List<PayrollCompanyConfiguration> payrollCompanyConfiguration,
            PreviewTransformResultDetail previewTransformResultDetail, 
            string originalString, string stampingOriginalString)
        {
            var dateFormat = new DateTimeUtil();
            var currencyConvert = new CurrencyUtil();
            var catalogSatManager = new CatalogSATUtil(previewTransformParams.IdentityWorkID,
                previewTransformParams.InstanceID, overdrafts, payrollCompanyConfiguration);

            //QRCode
            var qrData = base.GetQRCodeWithTemplate(new XMLGet(previewTransformResultDetail.XML));
            var qrCode = new QRutil().GetQRBase64(qrData, 20);

            XsltArgumentList xsltArgumentList = new XsltArgumentList();
            //default arguments - Logo
            xsltArgumentList.AddParam("logoCotorraTemplate", "", "data:image/png;base64," + GetLogoCotorra());

            xsltArgumentList.AddParam("overdraftID", "", previewTransformResultDetail.OverdraftID.ToString());

            //fiscal arguments
            xsltArgumentList.AddParam("cbbUriTemplate", "", !string.IsNullOrEmpty(qrCode) ? "data:image/png;base64," + qrCode : string.Empty);
            xsltArgumentList.AddParam("originalstring", "", !string.IsNullOrEmpty(originalString) ? originalString : string.Empty);
            xsltArgumentList.AddParam("stamporiginalstring", "", !string.IsNullOrEmpty(stampingOriginalString) ? stampingOriginalString : string.Empty);

            //object catalog argument            
            xsltArgumentList.AddExtensionObject("urn:catalogSat", catalogSatManager);
            //object for convert total amount in words           
            xsltArgumentList.AddExtensionObject("urn:convert", currencyConvert);
            xsltArgumentList.AddExtensionObject("urn:dateFormat", dateFormat);

            return xsltArgumentList;
        }

        private (string, string) getOriginalStrings(string XML)
        {
            var cfdiProvider = new Core.Managers.FiscalStamping.FiscalStampingCFDIv33Nomv12();
            var stampingOriginalString = cfdiProvider.GetStampingOriginalString(XML);
            var originalString = cfdiProvider.GetOriginalString(XML);
            return (originalString, stampingOriginalString);
        }

        /// <summary>
        /// Transform overdraft to html - pdf
        /// </summary>
        /// <param name="previewTransformParams"></param>
        /// <returns></returns>
        public async Task<PreviewTransformResult> TransformAsync(PreviewTransformParams previewTransformParams)
        {
            //Get data to transform
            List<Overdraft> overdrafts = await getDataAsync(previewTransformParams);

            //Get payroll company configuration
            var payrollCompanyConfiguration = await getPayrollCompanyConfigurationAsync(previewTransformParams);

            //Download XML
            Dictionary<Guid, string> xmls = await getXMLAsync(previewTransformParams, overdrafts);
                        
            var previewTransformResult = new PreviewTransformResult();
            var xsltUtil = new XsltUtil();           
            var pdfUtil = new PDFUtil();

            //Arrange
            var details = new ConcurrentBag<PreviewTransformResultDetail>();
            foreach (var previewTransformDetail in previewTransformParams.PreviewTransformParamsDetails)
            {
                var previewTransformResultDetail = new PreviewTransformResultDetail();

                //Get xml 
                previewTransformDetail.Overdraft = overdrafts.FirstOrDefault(p => p.ID == previewTransformDetail.OverdraftID);
                previewTransformResultDetail.OverdraftID = previewTransformDetail.OverdraftID;

                if (null == previewTransformDetail.Overdraft)
                {
                    throw new CotorraException(105, "105", "No se encontró el sobrerecibo proporcionado para generar su PDF.", null);
                }

                xmls.TryGetValue(previewTransformDetail.Overdraft.UUID, out string xml);
                previewTransformResultDetail.XML = xml;

                //Original strings       
                (var originalString, var stampingOriginalString) = getOriginalStrings(xml);

                var xsltArgumentList = getXSLTArguments(previewTransformParams, overdrafts, payrollCompanyConfiguration, 
                    previewTransformResultDetail, originalString, stampingOriginalString);

                //Transform HTML
                var htmlResult = xsltUtil.Transform(previewTransformResultDetail.XML, XsltPath, xsltArgumentList);

                //Transform PDF
                var pdfResult = pdfUtil.Convert(htmlResult);

                //Fill the result
                previewTransformResultDetail.TransformHTMLResult = htmlResult;
                previewTransformResultDetail.TransformPDFResult = pdfResult;

                details.Add(previewTransformResultDetail);
            }
            //Fill the list
            previewTransformResult.PreviewTransformResultDetails.AddRange(details);

            return previewTransformResult;
        }

    }
}
