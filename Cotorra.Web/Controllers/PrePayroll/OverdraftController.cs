using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cotorra.Schema;
using Cotorra.Client;
using System.Linq;
using Cotorra.Web.Utils;
using Cotorra.Schema.Calculation;
using Cotorra.Core.Utils;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class OverdraftController : Controller
    {
        private readonly Client<OverdraftDetail> oddclient;
        private readonly Client<Overdraft> odclient;
        private readonly CalculationClient calculationClient;
        private readonly PreviewerClient previewerClient;
        private readonly CancelStampingClient cancelStampingClient;
        private readonly Client<CancelationFiscalDocumentDetail> cancelationFiscalDocumentDetailClient;

        public OverdraftController()
        {
            SessionModel.Initialize();
            var adapterConfig = ClientConfiguration.GetAdapterFromConfig();
            oddclient = new Client<OverdraftDetail>(SessionModel.AuthorizationHeader, adapterConfig);
            odclient = new Client<Overdraft>(SessionModel.AuthorizationHeader, adapterConfig);
            cancelationFiscalDocumentDetailClient = new Client<CancelationFiscalDocumentDetail>(SessionModel.AuthorizationHeader, adapterConfig);
            calculationClient = new CalculationClient(SessionModel.AuthorizationHeader, adapterConfig);
            previewerClient = new PreviewerClient(SessionModel.AuthorizationHeader, adapterConfig);
            cancelStampingClient = new CancelStampingClient(SessionModel.AuthorizationHeader, adapterConfig);
        }

        private async Task Calculate(Guid overdraftID, bool restore)
        {
            var calculateOverdraftParams = new CalculateOverdraftParams()
            {
                IdentityWorkID = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                OverdraftID = overdraftID,
                UserID = SessionModel.IdentityID,
                ResetCalculation = restore,
                SaveOverdraft = true
            };
            await calculationClient.CalculateOverdraftAsync(calculateOverdraftParams);
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get(Guid overdraftID)
        {
            var resultOD = await odclient.FindAsync(x => x.InstanceID == SessionModel.InstanceID && x.ID == overdraftID,
                  SessionModel.CompanyID, new string[] { "OverdraftDetails", "Employee"/*, "HistoricEmployee"*/ });

            var od = resultOD.FirstOrDefault();

            var result =
                new
                {
                    Employee = od.OverdraftStatus == OverdraftStatus.None ?
                    new
                    {
                        //Overdraft Employee Header
                        od.Employee.ID,
                        od.Employee.FullName,
                        od.Employee.Code,
                        od.Employee.DailySalary,
                        od.Employee.JobPositionID,
                        od.Employee.SettlementSalaryBase
                    } : null,

                    HistoricEmployee = od.OverdraftStatus != OverdraftStatus.None ?

                    new
                    {
                        //Overdraft Employee Header
                        od.HistoricEmployee.ID,
                        od.HistoricEmployee.FullName,
                        od.HistoricEmployee.Code,
                        od.HistoricEmployee.DailySalary,
                        od.HistoricEmployee.JobPositionID,
                        od.HistoricEmployee.Active,
                        od.HistoricEmployee.EntryDate,
                        RFC = od.HistoricEmployee.RFC.ToUpper(),
                        NSS = od.HistoricEmployee.NSS.ToUpper(),
                        CURP = od.HistoricEmployee.CURP.ToUpper(),
                        od.HistoricEmployee.PeriodTypeID,
                        od.HistoricEmployee.SBCMax25UMA,
                        od.HistoricEmployee.SBCFixedPart,
                        od.HistoricEmployee.SBCVariablePart,
                        od.HistoricEmployee.DepartmentID,
                        od.HistoricEmployee.PaymentMethod,
                        WorkShiftID = od.HistoricEmployee.WorkshiftID,
                        od.HistoricEmployee.BankID,
                        od.HistoricEmployee.SalaryZone,
                        od.HistoricEmployee.RegimeType,
                        od.HistoricEmployee.BankAccount,
                        od.HistoricEmployee.BirthDate,
                        od.HistoricEmployee.BornPlace,
                        od.HistoricEmployee.CivilStatus,
                        od.HistoricEmployee.ContractType,
                        od.HistoricEmployee.ContributionBase,
                        od.HistoricEmployee.Description,
                        od.HistoricEmployee.Email,
                        od.HistoricEmployee.EmployerRegistrationID,
                        od.HistoricEmployee.ExteriorNumber,
                        od.HistoricEmployee.FederalEntity,
                        od.HistoricEmployee.FirstLastName,
                        od.HistoricEmployee.Gender,
                        od.HistoricEmployee.InteriorNumber,
                        od.HistoricEmployee.Municipality,
                        od.HistoricEmployee.Name,
                        od.HistoricEmployee.PaymentBase,
                        od.HistoricEmployee.EmployeeTrustLevel,
                        od.HistoricEmployee.Phone,
                        od.HistoricEmployee.SecondLastName,
                        od.HistoricEmployee.Street,
                        od.HistoricEmployee.Suburb,
                        od.HistoricEmployee.UMF,
                        od.HistoricEmployee.ZipCode,
                        od.HistoricEmployee.CLABE,
                        od.HistoricEmployee.BankBranchNumber,                      
                        od.HistoricEmployee.IdentityUserID,
                        IsIdentityVinculated = od.HistoricEmployee.IdentityUserID != null && od.HistoricEmployee.IdentityUserID != Guid.Empty,
                        od.HistoricEmployee.LastStatusChange,
                        od.HistoricEmployee.LocalStatus,
                        od.HistoricEmployee.BenefitType,
                        od.HistoricEmployee.ImmediateLeaderEmployeeID,
                        od.HistoricEmployee.SettlementSalaryBase,
                        //Data for employee data editing

                    } : null,

                    Details = from odd in od.OverdraftDetails
                              select new
                              {
                                  odd.ID,
                                  odd.ConceptPaymentID,
                                  odd.Value,
                                  odd.Amount,
                                  odd.IsGeneratedByPermanentMovement,
                                  odd.IsTotalAmountCapturedByUser,
                                  odd.IsValueCapturedByUser,                                  
                              }
                };

            return Json(result);
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetImports(Guid overdraftDetailID)
        {
            var result = await oddclient.FindAsync(x => x.InstanceID == SessionModel.InstanceID && x.ID == overdraftDetailID,
                    SessionModel.CompanyID, new string[] { });

            var overdraft = (from od in result

                             select new
                             {
                                 od.ID,
                                 od.Value,
                                 od.Amount,
                                 od.IsGeneratedByPermanentMovement,
                                 od.IsValueCapturedByUser,
                                 od.IsTotalAmountCapturedByUser,
                                 //od.Label1,
                                 //od.Label2,
                                 //od.Label3,
                                 //od.Label4,
                                 od.Taxed,
                                 od.Exempt,
                                 od.IMSSTaxed,
                                 od.IMSSExempt,
                                 od.IsAmount1CapturedByUser,
                                 od.IsAmount2CapturedByUser,
                                 od.IsAmount3CapturedByUser,
                                 od.IsAmount4CapturedByUser
                             });

            return Json(overdraft);
        }

        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> Restore(Guid overdraftID)
        {
            await Calculate(overdraftID, true);
            return Json("OK");
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete()
        {
            return await Task.FromResult(Json("OK"));
        }

        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> DeleteOverdrafts(List<Guid> Ids)
        {
            if (Ids != null && Ids.Any())
            {
                await odclient.DeleteAsync(Ids, SessionModel.CompanyID);
                return await Task.FromResult(Json("OK"));
            }
            return await Task.FromResult(Json("OK"));

        }

        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> UpdateAmount(Guid overdraftDetailID, Decimal? value, Decimal? amount, Boolean updateValue = false, Boolean updateAmount = false)
        {
            var result = await oddclient.FindAsync(x => x.InstanceID == SessionModel.InstanceID && x.ID == overdraftDetailID,
                    SessionModel.CompanyID, new string[] { });

            var odd = result.First();

            //Check for value
            if (updateValue)
            {
                odd.Value = value.HasValue ? value.Value : 0;
                odd.IsValueCapturedByUser = value.HasValue;
            }

            //Check for amount
            if (updateAmount)
            {
                odd.Amount = amount.HasValue ? amount.Value : 0;
                odd.IsTotalAmountCapturedByUser = amount.HasValue;
            }

            //Common
            odd.Active = true;
            odd.company = SessionModel.CompanyID;
            odd.InstanceID = SessionModel.InstanceID;
            odd.CreationDate = DateTime.Now;
            odd.StatusID = 1;
            odd.user = SessionModel.IdentityID;
            odd.Timestamp = DateTime.Now;

            await oddclient.UpdateAsync(new List<OverdraftDetail> { result.First() }, SessionModel.CompanyID);

            //calculate
            await Calculate(odd.OverdraftID, false);

            return Json("OK");
        }

        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> CalculateFormula(Guid overdraftID, string formula)
        {
            var calculateGenericParams = new CalculateGenericParams()
            {
                IdentityWorkID = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                Formula = formula,
                OverdraftID = overdraftID
            };
            var result = await calculationClient.CalculateFormulatAsync(calculateGenericParams);
            var response = $"{result.ResultText} = {result.Result}";
            return new JsonResult(response);
        }

        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> CalculateImports(Guid overdraftDetailID, Decimal amount)
        {
           
            return await Task.FromResult(Json(new
            {
                Amount1 = RandomSecure.RandomIntFromRNG(1, 9999),
                Amount2 = RandomSecure.RandomIntFromRNG(1, 9999),
                Amount3 = RandomSecure.RandomIntFromRNG(1, 9999),
                Amount4 = RandomSecure.RandomIntFromRNG(1, 9999),
            }));
        }

        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> SaveConcept(Guid overdraftID, Guid conceptPaymentID, decimal? amount, decimal? value)
        {

            var overdraftDetail = new OverdraftDetail();

            overdraftDetail.ID = Guid.NewGuid();
            overdraftDetail.OverdraftID = overdraftID;
            overdraftDetail.ConceptPaymentID = conceptPaymentID;
            overdraftDetail.Value = value.HasValue ? value.Value : 0;
            overdraftDetail.IsValueCapturedByUser = value.HasValue;
            overdraftDetail.Amount = amount.HasValue ? amount.Value : 0;
            overdraftDetail.IsTotalAmountCapturedByUser = amount.HasValue;

            overdraftDetail.IsValueCapturedByUser = false;
            overdraftDetail.IsGeneratedByPermanentMovement = false;
            overdraftDetail.Taxed = 0;
            overdraftDetail.Exempt = 0;
            overdraftDetail.IMSSTaxed = 0;
            overdraftDetail.IMSSExempt = 0;
            overdraftDetail.Label1 = "";
            overdraftDetail.Label2 = "";
            overdraftDetail.Label3 = "";
            overdraftDetail.Label4 = "";

            //Common
            overdraftDetail.Active = true;
            overdraftDetail.company = SessionModel.CompanyID;
            overdraftDetail.InstanceID = SessionModel.InstanceID;
            overdraftDetail.CreationDate = DateTime.Now;
            overdraftDetail.StatusID = 1;
            overdraftDetail.user = SessionModel.IdentityID;
            overdraftDetail.Timestamp = DateTime.Now;

            await oddclient.CreateAsync(new List<OverdraftDetail> { overdraftDetail }, SessionModel.IdentityID);

            //calculate
            await Calculate(overdraftDetail.OverdraftID, false);

            return Json("OK");
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> DeleteConcept(Guid overdraftID, Guid overdraftDetailID)
        {
            await oddclient.DeleteAsync(new List<Guid> { overdraftDetailID }, SessionModel.CompanyID);

            //calculate
            await Calculate(overdraftID, false);

            return Json("OK");
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> DownloadCFDI(Guid uuid)
        {
            var result = await previewerClient.GetPreviewUrlByUUIDAsync(SessionModel.InstanceID, uuid);
            return new JsonResult(result);
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> DownloadCancelledAcknowledgement(Guid overdraftId)
        {
            var cancelFiscalDocumentDetails = await cancelationFiscalDocumentDetailClient.FindAsync(p => p.OverdraftID == overdraftId,
                SessionModel.CompanyID, new string[] { "CancelationFiscalDocument" });

            if (!cancelFiscalDocumentDetails.Any())
            {
                throw new CotorraException(109, "109", "No se encontraron registros de cancelación para este recibo de nómina.", null);
            }
            else if (!cancelFiscalDocumentDetails.FirstOrDefault().CancelationFiscalDocument.CancelationResponseXMLID.HasValue)
            {
                throw new CotorraException(109, "109", "No tiene asignado un acuse de cancelación, por favor intente más tarde.", null);
            }

            var cancelAckID = cancelFiscalDocumentDetails.FirstOrDefault().CancelationFiscalDocument.CancelationResponseXMLID.Value;
            var result = await previewerClient.GetPreviewCancelationAckURLAsync(cancelAckID, SessionModel.InstanceID);
            return new JsonResult(result);
        }

        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> CancelCFDI(Guid overdraftID)
        {
            var cancelationParms = new CancelPayrollStampingParams()
            {
                FiscalStampingVersion = FiscalStampingVersion.CFDI33_Nom12,
                IdentityWorkID = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                user = SessionModel.IdentityID,
                OverdraftIDs = new List<Guid> { overdraftID }
            };
            var result = await cancelStampingClient.CancelPayrollStampingAsync(cancelationParms);

            if (result.WithErrors)
            {
                throw new CotorraException(108, "108", result.Message, null);
            }

            var cancelationDetail = result.CancelPayrollStampingResultDetails.FirstOrDefault();
            if (cancelationDetail.PayrollStampingResultStatus == PayrollStampingResultStatus.Fail)
            {
                throw new CotorraException(108, "108", cancelationDetail.Message, null);
            }

            return Json("OK");
        }
    }
}