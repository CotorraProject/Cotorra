using CotorraNode.Common.Library.Public;
using Cotorra.Core;
using Cotorra.Core.Managers;
using Cotorra.Core.Managers.Calculation;
using Cotorra.Core.Managers.FiscalPreview;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Cotorra.Schema.Calculation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Cotorra.UnitTest
{
    public class CancelStampingManagerUT
    {
        [Fact]
        public async Task Should_Stamp_Payroll_CFDI_Valid()
        {
            var xmlCancelacion = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Acuse xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" Fecha=\"2020-07-02T16:02:07.2838838\" RfcEmisor=\"KAHO641101B39\">  <Folios xmlns=\"http://cancelacfd.sat.gob.mx\">    <UUID>3377E0AA-C54B-4E9E-BFF2-1D8BA96D5DD4</UUID>    <EstatusUUID>201</EstatusUUID>  </Folios>  <Signature Id=\"SelloSAT\" xmlns=\"http://www.w3.org/2000/09/xmldsig#\">    <SignedInfo>      <CanonicalizationMethod Algorithm=\"http://www.w3.org/TR/2001/REC-xml-c14n-20010315\" />      <SignatureMethod Algorithm=\"http://www.w3.org/2001/04/xmldsig-more#hmac-sha512\" />      <Reference URI=\"\">        <Transforms>          <Transform Algorithm=\"http://www.w3.org/TR/1999/REC-xpath-19991116\">            <XPath>not(ancestor-or-self::*[local-name()='Signature'])</XPath>          </Transform>        </Transforms>        <DigestMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#sha512\" />        <DigestValue>BThmOyTf26Ax25v8Li0oqcP3wyrhW3kjjxOcO1zamRYasNIPcHSnBiRyxmJ449a3gdgAWaz/UKVil3pqcper+g==</DigestValue>      </Reference>    </SignedInfo>    <SignatureValue>6qDGoWoHW5tK2MGNiXU7fI6hfpkbrYTMHafVvIsGRkl9xq2H2YQRvId4CO7B9GGJFbuVMku2IBkpKU/Tscqo9Q==</SignatureValue>    <KeyInfo>      <KeyName>BF66E582888CC845</KeyName>      <KeyValue>        <RSAKeyValue>          <Modulus>n5YsGT0w5Z70ONPbqszhExfJU+KY3Bscftc2jxUn4wxpSjEUhnCuTd88OK5QbDW3Mupoc61jr83lRhUCjchFAmCigpC10rEntTfEU+7qtX8ud/jJJDB1a9lTIB6bhBN//X8IQDjhmHrfKvfen3p7RxLrFoxzWgpwKriuGI5wUlU=</Modulus>          <Exponent>AQAB</Exponent>        </RSAKeyValue>      </KeyValue>    </KeyInfo>  </Signature></Acuse>";
            var acuse = SerializerXml.DeserializeObject<Schema.CFDI33Nom12.Acuse>(xmlCancelacion);

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var identityWorkId = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkId, instanceID);
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

                //cancel payroll
                var cancelStampingManager = new CancelStampingManager();
                var cancelParams = new CancelPayrollStampingParams()
                {
                    FiscalStampingVersion = FiscalStampingVersion.CFDI33_Nom12,
                    IdentityWorkID = identityWorkId,
                    InstanceID = instanceID,
                    OverdraftIDs = overdraftsPrevious.Select(p => p.ID).ToList(),
                    user = Guid.Empty
                };

                await Task.Delay(10000);
                var cancelationResult = await cancelStampingManager.CancelPayrollStampingAsync(cancelParams);
                Assert.False(cancelationResult.WithErrors);

                var cancelManager = new MiddlewareManager<CancelationFiscalDocument>(new BaseRecordManager<CancelationFiscalDocument>(),
                    new CancelationFiscalDocumentValidator());
                var cancelations = await cancelManager.FindByExpressionAsync(p => p.InstanceID == instanceID, identityWorkId, 
                    new string[] { "CancelationFiscalDocumentDetails" });
                Assert.True(cancelations.Any());

                var overIds = overdraftsPrevious.Select(p => p.ID).ToList();
                var olderOverdrafts = await overdraftManager.FindByExpressionAsync(p => overIds.Contains(p.ID), identityWorkId);
                Assert.False(olderOverdrafts.Any(p=>p.OverdraftStatus != OverdraftStatus.Canceled));

                var newOverdrafts = await overdraftManager.FindByExpressionAsync(p => overIds.Contains(p.OverdraftPreviousCancelRelationshipID.Value), identityWorkId);
                Assert.False(newOverdrafts.Any(p => p.OverdraftPreviousCancelRelationshipID == null));
            }
            catch (Exception ex)
            {
                var t = ex.ToString();
                Assert.True(false, ex.ToString());
            }
        }
    }
}
