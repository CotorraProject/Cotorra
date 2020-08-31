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
using Cotorra.Schema.Calculation;

namespace Cotorra.Web.Controllers
{
    /// <summary>
    /// Fonacot Controller
    /// </summary>
    [Authentication]
    public class EmployeeConceptsRelationDetailController : Controller
    {
        #region "Attributes"
        private readonly Client<FonacotMovement> _client;

        private readonly Client<OverdraftDetail> _clientOverdraftDetail;
        private readonly Client<EmployeeConceptsRelationDetail> _clientEmployeeConceptsRelationDetail; 
        private readonly CalculationClient calculationClient;
        private readonly IEmployeeConceptsRelationDetailClient employeeConceptsRelationDetailClient;

        #endregion

        #region "Constructor"
        public EmployeeConceptsRelationDetailController()
        {
            SessionModel.Initialize();
            var clientAdapter = ClientConfiguration.GetAdapterFromConfig();
            var authorizationHeader = SessionModel.AuthorizationHeader;

            _client = new Client<FonacotMovement>(authorizationHeader, clientAdapter);
            _clientEmployeeConceptsRelationDetail = new Client<EmployeeConceptsRelationDetail>(authorizationHeader, clientAdapter);
            _clientOverdraftDetail = new Client<OverdraftDetail>(authorizationHeader, clientAdapter);
            calculationClient = new CalculationClient(authorizationHeader, clientAdapter);
            employeeConceptsRelationDetailClient = new EmployeeConceptsRelationDetailClient(authorizationHeader, clientAdapter);
        }
        #endregion

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get(Guid OverdraftID, Guid ConceptPaymentID)
        {

            var fonacot = await _client.FindAsync(x =>
                x.InstanceID == SessionModel.InstanceID &&
                x.EmployeeConceptsRelation.ConceptPaymentID == ConceptPaymentID &&
                x.EmployeeConceptsRelation.EmployeeConceptsRelationDetails.Any(y => y.OverdraftID == OverdraftID),
                SessionModel.CompanyID, new string[] { "EmployeeConceptsRelation", "EmployeeConceptsRelation.EmployeeConceptsRelationDetails" });

            var details = fonacot.Select(x => x.EmployeeConceptsRelation).SelectMany(p => p.EmployeeConceptsRelationDetails).Where(y => y.OverdraftID == OverdraftID);
            var conceptRelation = fonacot.Select(x => x.EmployeeConceptsRelation);

            var fonacots = details
                .Select(x => new
                {
                    x.ID,
                    x.AmountApplied,
                    fonacot.FirstOrDefault(p => p.EmployeeConceptsRelationID == x.EmployeeConceptsRelationID).CreditNumber,
                    fonacot.FirstOrDefault(p => p.EmployeeConceptsRelationID == x.EmployeeConceptsRelationID).Description,
                    conceptRelation.FirstOrDefault(p=> p.ID == x.EmployeeConceptsRelationID).BalanceCalculated,
                    x.IsAmountAppliedCapturedByUser,
                });

            return Json(fonacots);
        }

        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> Save(Guid OverdraftID, Guid OverdraftDetailID,  Decimal Amount, List<ConceptRelationUpdateAmountDTO> ConceptRelationsAmountApplied )
        {

           await employeeConceptsRelationDetailClient.UpdateAmountAsync(OverdraftDetailID, Amount, ConceptRelationsAmountApplied, SessionModel.InstanceID, SessionModel.CompanyID);
                 
            await Calculate(OverdraftID, false);

            return Json("OK");
        }

        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> Restore(Guid OverdraftID, Guid OverdraftDetailID, Decimal Amount, List<ConceptRelationUpdateAmountDTO> ConceptRelationsAmountApplied)
        {

            await employeeConceptsRelationDetailClient.UpdateAmountAsync(OverdraftDetailID, Amount, ConceptRelationsAmountApplied, SessionModel.InstanceID, SessionModel.CompanyID);

            await Calculate(OverdraftID, false);

            return Json("OK");
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
    }



}