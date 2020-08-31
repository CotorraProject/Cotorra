using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cotorra.Schema;
using Cotorra.Client;
using System.Linq;
using Cotorra.Web.Utils;
using AutoMapper;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class InfonavitController : Controller
    {
        private readonly Client<InfonavitMovement> client;
        private readonly Client<EmployeeConceptsRelation> _clientEmployeeConceptsRelation;
        private readonly Client<ConceptPayment> _conceptPaymentclient;
        private readonly CalculationClient _calculationClient;

        private readonly IMapper _mapper;

        public InfonavitController()
        {
            SessionModel.Initialize();
            var adapterClient = ClientConfiguration.GetAdapterFromConfig();
            var authHeaderToken = SessionModel.AuthorizationHeader;

            client = new Client<InfonavitMovement>(authHeaderToken, adapterClient);
            _conceptPaymentclient = new Client<ConceptPayment>(authHeaderToken, adapterClient);
            _calculationClient = new CalculationClient(authHeaderToken, adapterClient);
            _clientEmployeeConceptsRelation = new Client<EmployeeConceptsRelation>(authHeaderToken, adapterClient);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<InfonavitMovement, EmployeeConceptsRelation>();
            });
            _mapper = config.CreateMapper();
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get(Guid employeeID)
        {
            var findResult = await client.FindAsync(x => x.InstanceID == SessionModel.InstanceID
                && x.EmployeeID == employeeID, SessionModel.CompanyID, new String[] { });

            var hasOneAcviteCredit = findResult.Any(x => x.InfonavitStatus);

            var infonavits = (from x in findResult
                              orderby x.CreditNumber
                              select new
                              {
                                  x.ID,
                                  x.CreditNumber,
                                  x.Description,
                                  x.InfonavitCreditType,
                                  x.MonthlyFactor,
                                  x.IncludeInsurancePayment_D14,
                                  InitialApplicationDate = x.InitialApplicationDate.ToString("dd/MM/yyyy"),
                                  x.AccumulatedAmount,
                                  x.AppliedTimes,
                                  RegisterDate = x.RegisterDate.ToString("dd/MM/yyyy"),
                                  InfonavitStatus = x.InfonavitStatus ? 1 : 0,
                                  x.EmployeeConceptsRelationID,
                                  x.EmployeeConceptsRelationInsuranceID
                              }).OrderByDescending(p => p.InfonavitStatus).ThenBy(y => y.CreditNumber);

            return Json(new
            {
                hasOneAcviteCredit,
                infonavits
            });
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(InfonavitMovement infonavitMovement)
        {
            infonavitMovement.ConceptPaymentID = await GetInfonavitConceptID(infonavitMovement.InfonavitCreditType);
            infonavitMovement.ConceptPayment = null;
            //Common
            infonavitMovement.Active = true;
            infonavitMovement.company = SessionModel.CompanyID;
            infonavitMovement.InstanceID = SessionModel.InstanceID;
            infonavitMovement.CreationDate = DateTime.Now;
            infonavitMovement.StatusID = 1;
            infonavitMovement.user = SessionModel.IdentityID;
            infonavitMovement.Timestamp = DateTime.Now;
            infonavitMovement.Employee = null;

            //Seguro de vivienda
            if (infonavitMovement.IncludeInsurancePayment_D14)
            {
                var employeeConceptRelationInsurancePayment = _mapper.Map<InfonavitMovement, EmployeeConceptsRelation>(infonavitMovement);
                employeeConceptRelationInsurancePayment.ConceptPaymentID = await GetInfonavitConceptID(InfonavitCreditType.HomeInsurance_D14);
                employeeConceptRelationInsurancePayment.Active = true;
                employeeConceptRelationInsurancePayment.Name = String.Empty;
                employeeConceptRelationInsurancePayment.Description = String.Empty;
                employeeConceptRelationInsurancePayment.company = SessionModel.CompanyID;
                employeeConceptRelationInsurancePayment.InstanceID = SessionModel.InstanceID;
                employeeConceptRelationInsurancePayment.CreationDate = DateTime.Now;
                employeeConceptRelationInsurancePayment.StatusID = 1;
                employeeConceptRelationInsurancePayment.user = SessionModel.IdentityID;
                employeeConceptRelationInsurancePayment.Timestamp = DateTime.Now;
                employeeConceptRelationInsurancePayment.InitialCreditDate = infonavitMovement.InitialApplicationDate;
                employeeConceptRelationInsurancePayment.ConceptPaymentStatus = infonavitMovement.InfonavitStatus ? ConceptPaymentStatus.Active : ConceptPaymentStatus.Inactive;
                employeeConceptRelationInsurancePayment.CreditAmount = 999999999;

                if (null == infonavitMovement.EmployeeConceptsRelationInsuranceID ||
                    infonavitMovement.EmployeeConceptsRelationInsuranceID == Guid.Empty)
                {
                    employeeConceptRelationInsurancePayment.ID = Guid.NewGuid();
                    infonavitMovement.EmployeeConceptsRelationInsuranceID = employeeConceptRelationInsurancePayment.ID;
                    infonavitMovement.EmployeeConceptsRelationInsurance = null;

                    //create employeeConceptsRelationInsurance
                    await _clientEmployeeConceptsRelation.CreateAsync(new List<EmployeeConceptsRelation>() { employeeConceptRelationInsurancePayment }, SessionModel.CompanyID);
                }
                else
                {
                    //update employeeConceptsRelationInsurance
                    employeeConceptRelationInsurancePayment.ID = infonavitMovement.EmployeeConceptsRelationInsuranceID.Value;
                    await _clientEmployeeConceptsRelation.UpdateAsync(new List<EmployeeConceptsRelation>() { employeeConceptRelationInsurancePayment }, SessionModel.CompanyID);
                }
            }

            var employeeConceptRelation = _mapper.Map<InfonavitMovement, EmployeeConceptsRelation>(infonavitMovement);
            employeeConceptRelation.Active = true;
            employeeConceptRelation.Name = String.Empty;
            employeeConceptRelation.Description = String.Empty;
            employeeConceptRelation.company = SessionModel.CompanyID;
            employeeConceptRelation.InstanceID = SessionModel.InstanceID;
            employeeConceptRelation.CreationDate = DateTime.Now;
            employeeConceptRelation.StatusID = 1;
            employeeConceptRelation.user = SessionModel.IdentityID;
            employeeConceptRelation.Timestamp = DateTime.Now;
            employeeConceptRelation.ConceptPaymentID = infonavitMovement.ConceptPaymentID;
            employeeConceptRelation.InitialCreditDate = infonavitMovement.InitialApplicationDate;
            employeeConceptRelation.ConceptPaymentStatus = infonavitMovement.InfonavitStatus ? ConceptPaymentStatus.Active : ConceptPaymentStatus.Inactive;
            employeeConceptRelation.CreditAmount = 999999999;

            if (infonavitMovement.ID == Guid.Empty)
            {
                infonavitMovement.ID = Guid.NewGuid();

                employeeConceptRelation.ID = Guid.NewGuid();
                infonavitMovement.EmployeeConceptsRelationID = employeeConceptRelation.ID;
                infonavitMovement.EmployeeConceptsRelation = employeeConceptRelation;

                await client.CreateAsync(new List<InfonavitMovement> { infonavitMovement }, SessionModel.IdentityID);
            }
            else
            {
                employeeConceptRelation.ID = infonavitMovement.EmployeeConceptsRelationID;
                infonavitMovement.EmployeeConceptsRelationID = employeeConceptRelation.ID;
                infonavitMovement.EmployeeConceptsRelation = employeeConceptRelation;
                await client.UpdateAsync(new List<InfonavitMovement> { infonavitMovement }, SessionModel.IdentityID);
            }

            //Recalculate
            await _calculationClient.CalculationByEmployeesAsync(new CalculationFireAndForgetByEmployeeParams()
            {
                EmployeeIds = new List<Guid> { infonavitMovement.EmployeeID },
                IdentityWorkID = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                UserID = SessionModel.IdentityID
            });

            return Json(new
            {
                infonavitMovement.ID,
                infonavitMovement.EmployeeConceptsRelationID,
            });
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id, Guid employeeID)
        {
            await client.DeleteAsync(new List<Guid>() { id }, SessionModel.CompanyID);

            //Recalculate
            await _calculationClient.CalculationFireAndForgetByEmployeesAsync(new CalculationFireAndForgetByEmployeeParams()
            {
                EmployeeIds = new List<Guid> { employeeID },
                IdentityWorkID = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                UserID = SessionModel.IdentityID
            });

            return Json("OK");
        }

        private async Task<Guid> GetInfonavitConceptID(InfonavitCreditType creditType)
        {

            var code = creditType switch
            {
                InfonavitCreditType.FixQuota_D16 => 16,
                InfonavitCreditType.DiscountFactor_D15 => 15,
                InfonavitCreditType.Percentage_D59 => 59,
                InfonavitCreditType.HomeInsurance_D14 => 14,
                _ => throw new NotImplementedException("Tipo de retención infonavit inexistente"),
            };

            var result = await _conceptPaymentclient.FindAsync(x =>
                    x.InstanceID == SessionModel.InstanceID &&
                    x.Code == code &&
                    x.ConceptType == ConceptType.DeductionPayment,
                SessionModel.CompanyID);

            return result.Any() ? result.First().ID : Guid.Empty;
        }

    }
}