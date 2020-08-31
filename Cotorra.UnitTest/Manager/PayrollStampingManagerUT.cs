using Cotorra.Core;
using Cotorra.Core.Managers;
using Cotorra.Core.Utils;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;
using Cotorra.Core.Managers.FiscalPreview;
using Cotorra.Core.Managers.Calculation;
using Cotorra.Schema.Calculation;
using Org.BouncyCastle.Bcpg;
using CotorraNode.Common.Library.Public;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Text;
using System.Net.Http;

namespace Cotorra.UnitTest
{
    /// <summary>
    /// Clase para manejar el acceso a los catalogos del SAT a traves del componente fiscal 
    /// </summary>

    public class PayrollStampingManagerUT
    {
        public async Task<Overdraft> CreateRealOverdraftAsync(Guid identityWorkId, Guid instanceID)
        {
            var employee = await new EmployeeManagerUT().CreateDefaultAsync<Employee>(
                identityWorkId, instanceID);

            //MinimunSalary
            await new MinimunSalaryManagerUT().CreateAsync(identityWorkId, instanceID);

            //UMAs
            await new UMAManagerUT().CreateDefaultAsync<UMA>(identityWorkId, instanceID);

            //SGDFLimits
            await new SGDFLimitsManagerUT().CreateDefaultAsync<SGDFLimits>(identityWorkId, instanceID);

            //IMSSEmployeeTable
            await new IMSSEmployeeTableManagerUT().CreateDefaultAsync<IMSSEmployeeTable>(identityWorkId, instanceID);

            //IMSSEmployerTable
            await new IMSSEmployerTableManagerUT().CreateDefaultAsync<IMSSEmployerTable>(identityWorkId, instanceID);

            //IMSSWorkRisk
            await new IMSSWorkRiskManagerUT().CreateDefaultAsync<IMSSEmployerTable>(identityWorkId, instanceID);

            //SettlementCatalog
            await new SettlementCatalogManagerUT().CreateDefaultAsync<SettlementCatalog>(identityWorkId, instanceID);

            //MonthlyIncomeTax
            await new MonthlyIncomeTaxManagerUT().CreateDefaultAsync<MonthlyIncomeTax>(identityWorkId, instanceID);

            //Anual
            await new AnualIncomeTaxManagerUT().CreateDefaultAsync<AnualIncomeTax>(identityWorkId, instanceID);

            //MonthlyEmploymentSubsidy
            await new MonthlyEmploymentSubsidyManagerUT().CreateDefaultAsync<MonthlyEmploymentSubsidy>(identityWorkId, instanceID);

            //AnualEmploymentSubsidy
            await new AnualEmploymentSubsidyManagerUT().CreateDefaultAsync<AnualEmploymentSubsidy>(identityWorkId, instanceID);

            var middlewareManager = new MiddlewareManager<OverdraftDetail>(new BaseRecordManager<OverdraftDetail>(), new OverdraftDetailValidator());
            var overdraftDetails = await middlewareManager.FindByExpressionAsync(p => p.InstanceID == instanceID, identityWorkId,
                new string[] { "Overdraft", "ConceptPayment" });

            return overdraftDetails.FirstOrDefault().Overdraft;
        }

        public async Task<Overdraft> CreateRealOverdraftAsync(Guid identityWorkId, Guid instanceID,
            decimal diarySalary, decimal SBCFixed)
        {
            var employee = await new EmployeeManagerUT().CreateDefaultAsync<Employee>(
                identityWorkId, instanceID, diarySalary, SBCFixed);

            //MinimunSalary
            await new MinimunSalaryManagerUT().CreateAsync(identityWorkId, instanceID);

            //UMAs
            await new UMAManagerUT().CreateDefaultAsync<UMA>(identityWorkId, instanceID);

            //SGDFLimits
            await new SGDFLimitsManagerUT().CreateDefaultAsync<SGDFLimits>(identityWorkId, instanceID);

            //IMSSEmployeeTable
            await new IMSSEmployeeTableManagerUT().CreateDefaultAsync<IMSSEmployeeTable>(identityWorkId, instanceID);

            //IMSSEmployerTable
            await new IMSSEmployerTableManagerUT().CreateDefaultAsync<IMSSEmployerTable>(identityWorkId, instanceID);

            //IMSSWorkRisk
            await new IMSSWorkRiskManagerUT().CreateDefaultAsync<IMSSEmployerTable>(identityWorkId, instanceID);

            //SettlementCatalog
            await new SettlementCatalogManagerUT().CreateDefaultAsync<SettlementCatalog>(identityWorkId, instanceID);

            //MonthlyIncomeTax
            await new MonthlyIncomeTaxManagerUT().CreateDefaultAsync<MonthlyIncomeTax>(identityWorkId, instanceID);

            //Anual
            await new AnualIncomeTaxManagerUT().CreateDefaultAsync<AnualIncomeTax>(identityWorkId, instanceID);

            //MonthlyEmploymentSubsidy
            await new MonthlyEmploymentSubsidyManagerUT().CreateDefaultAsync<MonthlyEmploymentSubsidy>(identityWorkId, instanceID);

            //AnualEmploymentSubsidy
            await new AnualEmploymentSubsidyManagerUT().CreateDefaultAsync<AnualEmploymentSubsidy>(identityWorkId, instanceID);

            var middlewareManager = new MiddlewareManager<OverdraftDetail>(new BaseRecordManager<OverdraftDetail>(), new OverdraftDetailValidator());
            var overdraftDetails = await middlewareManager.FindByExpressionAsync(p => p.InstanceID == instanceID, identityWorkId,
                new string[] { "Overdraft", "ConceptPayment" });

            return overdraftDetails.FirstOrDefault().Overdraft;
        }


        private async Task<OverdraftDetail> createIfNotExistsOverdraftDetailAsync(Guid identityWorkId, Guid instanceID, Guid overdraftID,
            int code, decimal amount, decimal taxed, decimal exempt, ConceptType conceptType = ConceptType.DeductionPayment, int satGroupCode = 0)
        {
            var middlewareManager = new MiddlewareManager<OverdraftDetail>(new BaseRecordManager<OverdraftDetail>(), new OverdraftDetailValidator());

            var middlewareConcepts = new MiddlewareManager<ConceptPayment>(new BaseRecordManager<ConceptPayment>(), new ConceptPaymentValidator());
            var conceptsAll = await middlewareConcepts.FindByExpressionAsync(p => p.InstanceID == instanceID, identityWorkId,
                new string[] { });

            if (!conceptsAll.Any(p => p.ConceptType == conceptType && p.Code == code))
            {
                var conceptPayment = new ConceptPayment()
                {
                    Active = true,
                    AutomaticDismissal = true,
                    Code = code,
                    company = identityWorkId,
                    ConceptType = conceptType,
                    CreationDate = DateTime.Now,
                    DeleteDate = null,
                    Description = $"{code}",
                    Name = $"{code}",
                    GlobalAutomatic = true,
                    ID = Guid.NewGuid(),
                    InstanceID = instanceID,
                    Kind = true,
                    Print = true,
                    SATGroupCode = "D-004",
                    StatusID = 1,
                    Timestamp = DateTime.Now,
                    user = Guid.NewGuid()
                };
                await middlewareConcepts.CreateAsync(new List<ConceptPayment> { conceptPayment }, identityWorkId);

                conceptsAll.Add(conceptPayment);
            }


            var overdraftDetail = new OverdraftDetail()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceID,
                Description = "Sobrerecibo detalle",
                CreationDate = DateTime.Now,
                Name = "Sobrerecibo detalle",
                StatusID = 1,
                user = Guid.NewGuid(),
                ConceptPaymentID = conceptsAll.FirstOrDefault(p => p.ConceptType == conceptType && p.Code == code).ID,
                Amount = amount,
                Taxed = taxed,
                Exempt = exempt,
                IMSSTaxed = 0,
                IMSSExempt = 0,
                Label1 = "Label1",
                Label2 = "Label2",
                Label3 = "Label3",
                Label4 = "Label4",
                OverdraftID = overdraftID,
                Value = amount,
                ConceptPayment = null,
                Overdraft = null
            };

            await middlewareManager.CreateAsync(new List<OverdraftDetail> { overdraftDetail }, identityWorkId);

            return overdraftDetail;

        }

        [Fact]
        public async Task Should_Deseralize()
        {
            string xml = "<tfd:TimbreFiscalDigital xmlns:tfd=\"http://www.sat.gob.mx/TimbreFiscalDigital\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.sat.gob.mx/TimbreFiscalDigital http://www.sat.gob.mx/sitio_internet/cfd/timbrefiscaldigital/TimbreFiscalDigitalv11.xsd\" Version=\"1.1\" UUID=\"3330E934-B635-4561-AFD8-7A49B75F4AAA\" FechaTimbrado=\"2020-06-17T11:38:16\" RfcProvCertif=\"MAS0810247C0\" SelloCFD=\"QLNSw2sKG8WQws0KH5sw3ciasgXQnEIbpiwPJ62vhEtF8JSS3YA17BkSNZ7qKI4j72CAjdjmtYvDS8CoM4e+AIOFQBs7D0USAKClDJear/dJ6QBT4NVGP2o7YHrl4w5hh4zWmCdS5MA2BnATzWuluyLXhdFLWNHQTG4uLl9ZnZ3hhgxgpkxoVGuIZ0O0igYn9d/imj6jODLg2+goMXr39+nE9gnGM7bm0KkjPuv3TjeG+DmE1W+UmWzqKGOgh/1h0OwaE9RyJ3B58xD3+nMQPTdxfm/eotEFLpaw17ANbS/Uoxr5KQR7WCh3ooxLdBk4emGppbuAMn70ToL15/lv+w==\" NoCertificadoSAT=\"30001000000400002495\" SelloSAT=\"WEDtOHwRJFMYbMeF75KDheWP5eKPU/OErwoyKIchcoAfUUUDCFh3NdIzwEEVebklAPS0Nh7bctdOyDQehAb/GV15vG9iRrh45P/P86+ZNKBMln+bQhwe22trycXU0xduY9xdJWE244dObYEayROYuFoe1NyJsoifbn4dUTh8nZyqLrupOwhGY7+NOGDqskfWsSGh8/KMxJGza2upGVAcSH8D0grfA1HLQegqtSFUBTKCFnJzFJwiUToHP3q4ZXHnsDkN03+usGkTII6M8aoswcogB3+RTSfKaXz9pJLRTN6j0Blyhbz8MySbxl+C71ZfZByj7pjPK8JiIxjqKldX9g==\" />";
            var tfd = SerializerXml.DeserializeObject<Schema.CFDI33Nom12.TimbreFiscalDigital>($"<?xml version=\"1.0\" encoding=\"utf-8\"?>{xml}");

            List<string> xsdsFiles = new List<string> {
                @"C:\Projects\Cotorra\Cotorra.Schema\bin\Debug\netstandard2.1\fiscal\cfdi33nom12\xsd\cfdv33.xsd" ,
                @"C:\Projects\Cotorra\Cotorra.Schema\bin\Debug\netstandard2.1\fiscal\cfdi33nom12\xsd\catCFDI.xsd",
                @"C:\Projects\Cotorra\Cotorra.Schema\bin\Debug\netstandard2.1\fiscal\cfdi33nom12\xsd\catNomina.xsd",
                @"C:\Projects\Cotorra\Cotorra.Schema\bin\Debug\netstandard2.1\fiscal\cfdi33nom12\xsd\nomina12.xsd",
                @"C:\Projects\Cotorra\Cotorra.Schema\bin\Debug\netstandard2.1\fiscal\cfdi33nom12\xsd\tdCFDI.xsd",
                @"C:\Projects\Cotorra\Cotorra.Schema\bin\Debug\netstandard2.1\fiscal\cfdi33nom12\xsd\TimbreFiscalDigitalv11.xsd",
            };
            string xmlFile = @"C:\Users\Hector.Ramirez\Desktop\cfdi.xml";

            var xdoc = XDocument.Load(xmlFile);
            var schemas = new XmlSchemaSet();
            foreach (var xsdFile in xsdsFiles)
            {
                using (FileStream stream = File.OpenRead(xsdFile))
                {
                    schemas.Add(XmlSchema.Read(stream, (s, e) =>
                    {
                        var x = e.Message;
                    }));
                }
            }

            bool isvalid = true;
            StringBuilder sb = new StringBuilder();
            try
            {
                xdoc.Validate(schemas, (s, e) =>
                {
                    isvalid = false;
                    sb.AppendLine(string.Format("Line : {0}, Message : {1} ",
                        e.Exception.LineNumber, e.Exception.Message));
                });
            }
            catch (XmlSchemaValidationException ex)
            {
                isvalid = false;
            }
        }

        [Fact]
        public async Task Should_Stamp_Payroll_CFDI_Valid()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {

                var identityWorkId = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                var overdraft = await CreateRealOverdraftAsync(identityWorkId, instanceID);
                var periodDetailID = overdraft.PeriodDetailID;

                //Recalculate
                var calculateParams = new CalculateOverdraftParams()
                {
                    DeleteAccumulates = true,
                    IdentityWorkID = identityWorkId,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    ResetCalculation = true,
                    SaveOverdraft = true,
                    UserID = Guid.Empty
                };

                var calculationResult = await new OverdraftCalculationManager().CalculateAsync(calculateParams);
                overdraft = (calculationResult as CalculateOverdraftResult).OverdraftResult;
                Assert.True(overdraft.OverdraftDetails.Sum(p => p.Amount) > 0);

                //Autorización de la nómina
                var authorizationManager = new AuthorizationManager();
                var authorizationParams = new AuthorizationParams()
                {
                    IdentityWorkID = identityWorkId,
                    InstanceID = instanceID,
                    PeriodDetailIDToAuthorize = periodDetailID,
                    ResourceID = Guid.Empty,
                    user = Guid.Empty
                };

                //autorizacion de la nómina
                var historicOverdrafts = await authorizationManager.AuthorizationAsync(authorizationParams);

                //Timbrado

                var overdraftManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdraftsPrevious = await overdraftManager.FindByExpressionAsync(p => p.PeriodDetailID == periodDetailID, identityWorkId);

                var manager = new PayrollStampingManager();
                var dateTime = DateTime.Now;
                var stampingParms = new PayrollStampingParams()
                {
                    FiscalStampingVersion = FiscalStampingVersion.CFDI33_Nom12,
                    IdentityWorkID = identityWorkId,
                    InstanceID = instanceID,
                    PeriodDetailID = periodDetailID,
                    Detail = new List<PayrollStampingDetail>()
                    {
                        new PayrollStampingDetail()
                        {
                            Folio = "2020",
                            Series = "S1",
                            PaymentDate = dateTime.AddDays(-2),
                            RFCOriginEmployer = null,
                            SNCFEntity = null,
                            OverdraftID =overdraftsPrevious.FirstOrDefault().ID,
                        }
                    },

                    Currency = Currency.MXN
                };

                var payrollStampingResult = await manager.PayrollStampingAsync(stampingParms);
                Assert.Contains(payrollStampingResult.PayrollStampingResultDetails, p => p.PayrollStampingResultStatus == PayrollStampingResultStatus.Success);

                var uuid = payrollStampingResult.PayrollStampingResultDetails.FirstOrDefault().Overdraft.UUID;
                var fiscalManager = FiscalPreviewFactory.CreateInstance(FiscalStampingVersion.CFDI33_Nom12);
                var resultUrls = await fiscalManager.GetPreviewUrlByUUIDAsync(instanceID, uuid);

                HttpClient client = new HttpClient();
                var pdfBytes = await client.GetByteArrayAsync(resultUrls.PDFUri);

                //Save XML to FileSystem
                var xmlFilePath = Path.Combine(DirectoryUtil.AssemblyDirectory, "example.xml");
                await File.WriteAllTextAsync(xmlFilePath, payrollStampingResult.PayrollStampingResultDetails.FirstOrDefault().XML);
                Assert.True(File.Exists(xmlFilePath));

                //Save PDF to FileSystem
                var outPDFFilePath = Path.Combine(DirectoryUtil.AssemblyDirectory, "example.pdf");
                await File.WriteAllBytesAsync(outPDFFilePath, pdfBytes);
                Assert.True(File.Exists(outPDFFilePath));
            }
            catch (Exception ex)
            {
                var t = ex.ToString();
                Assert.True(false, ex.ToString());
            }
        }
    }
}
