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
using System.Transactions;

namespace Cotorra.Web.Controllers
{
    /// <summary>
    /// Fonacot Controller
    /// </summary>
    [Authentication]
    public class FonacotController : Controller
    {
        #region "Attributes"
        private readonly Client<FonacotMovement> _client;
        private readonly Client<EmployeeConceptsRelation> _clientEmployeeConceptsRelation;
        private readonly Client<ConceptPayment> _conceptPaymentclient;
        private readonly CalculationClient _calculationClient;
        private readonly IMapper _mapper;
        #endregion

        #region "Constructor"
        public FonacotController()
        {
            SessionModel.Initialize();
            var clientAdapter = ClientConfiguration.GetAdapterFromConfig();
            var authorizationHeader = SessionModel.AuthorizationHeader;
            _client = new Client<FonacotMovement>(authorizationHeader, clientAdapter);
            _clientEmployeeConceptsRelation = new Client<EmployeeConceptsRelation>(authorizationHeader, clientAdapter);
            _conceptPaymentclient = new Client<ConceptPayment>(authorizationHeader, clientAdapter);
            _calculationClient = new CalculationClient(authorizationHeader, clientAdapter);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FonacotMovementDTO, FonacotMovement>();
                cfg.CreateMap<FonacotMovementDTO, EmployeeConceptsRelation>();
            });

            _mapper = config.CreateMapper();
        }
        #endregion

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get(Guid employeeID)
        {
            try
            {
                var result = await _client.FindAsync(x =>
                        x.InstanceID == SessionModel.InstanceID &&
                        x.EmployeeID == employeeID,
                SessionModel.CompanyID, new string[] { "EmployeeConceptsRelation" });

                var fonacots = result
                    .Select(x => new
                    {
                        x.ID,
                        x.Name,
                        x.CreditNumber,
                        x.Description,
                        x.Month,
                        x.Year,
                        x.RetentionType,
                        x.FonacotMovementStatus,
                        x.Observations,
                        x.ConceptPaymentID,
                        x.EmployeeID,
                        x.EmployeeConceptsRelationID,
                        x.EmployeeConceptsRelation.CreditAmount,
                        x.EmployeeConceptsRelation.OverdraftDetailAmount,
                        x.EmployeeConceptsRelation.OverdraftDetailValue,
                        x.EmployeeConceptsRelation.PaymentsMadeByOtherMethod,
                        x.EmployeeConceptsRelation.AccumulatedAmountWithHeldCalculated,
                        x.EmployeeConceptsRelation.BalanceCalculated,
                    })
                    .OrderBy(x => x.Name)
                    .ToList();

                return Json(fonacots);
            }
            catch(Exception ex)
            {
                var msg = ex.ToString();
                return null;
            }
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(FonacotMovementDTO fonacotDTO)
        {
            //Fonacot
            var fonacot = _mapper.Map<FonacotMovementDTO, FonacotMovement>(fonacotDTO);
            fonacot.Name = "FONACOT " + fonacot.CreditNumber;
            fonacot.ConceptPaymentID = await GetFonacotConceptID();
            fonacot.Description = String.IsNullOrEmpty(fonacotDTO.Description) ? String.Empty : fonacotDTO.Description;

            //Common
            fonacot.Active = true;
            fonacot.company = SessionModel.CompanyID;
            fonacot.InstanceID = SessionModel.InstanceID;
            fonacot.CreationDate = DateTime.Now;
            fonacot.StatusID = 1;
            fonacot.user = SessionModel.IdentityID;
            fonacot.Timestamp = DateTime.Now;

            var employeeConceptRelation = _mapper.Map<FonacotMovementDTO, EmployeeConceptsRelation>(fonacotDTO);
            employeeConceptRelation.Active = true;
            employeeConceptRelation.Name = String.Empty;
            employeeConceptRelation.Description = String.Empty;
            employeeConceptRelation.company = SessionModel.CompanyID;
            employeeConceptRelation.InstanceID = SessionModel.InstanceID;
            employeeConceptRelation.CreationDate = DateTime.Now;
            employeeConceptRelation.StatusID = 1;
            employeeConceptRelation.user = SessionModel.IdentityID;
            employeeConceptRelation.Timestamp = DateTime.Now;
            employeeConceptRelation.ConceptPaymentID = fonacot.ConceptPaymentID;
            employeeConceptRelation.InitialCreditDate = new DateTime(fonacot.Year, fonacot.Month, 1);

            if (fonacot.FonacotMovementStatus == FonacotMovementStatus.Active)
            {
                employeeConceptRelation.ConceptPaymentStatus = ConceptPaymentStatus.Active;
            }
            else
            {
                employeeConceptRelation.ConceptPaymentStatus = ConceptPaymentStatus.Inactive;
            }

            if (fonacot.ID == Guid.Empty)
            {
                fonacot.ID = Guid.NewGuid();
                employeeConceptRelation.ID = Guid.NewGuid();
                fonacot.EmployeeConceptsRelationID = employeeConceptRelation.ID;
                fonacot.EmployeeConceptsRelation = employeeConceptRelation;
                await _client.CreateAsync(new List<FonacotMovement>() { fonacot }, SessionModel.IdentityID);
            }
            else
            {
                employeeConceptRelation.ID = fonacotDTO.EmployeeConceptsRelationID;
                fonacot.EmployeeConceptsRelation = employeeConceptRelation;
                await _client.UpdateAsync(new List<FonacotMovement>() { fonacot }, SessionModel.IdentityID);
            }

            return Json(new
            {
                ID = fonacot.ID,
                EmployeeConceptsRelationID = fonacot.EmployeeConceptsRelationID
            });
        }

        private async Task<Guid> GetFonacotConceptID()
        {
            var result = await _conceptPaymentclient.FindAsync(x =>
                    x.InstanceID == SessionModel.InstanceID &&
                    x.Code == 61 &&
                    x.ConceptType == ConceptType.DeductionPayment,
                SessionModel.CompanyID);

            return result.Any() ? result.First().ID : Guid.Empty;
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id, Guid employeeConceptsRelationID, Guid employeeID)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await _client.DeleteAsync(new List<Guid>() { id }, SessionModel.CompanyID);
                await _clientEmployeeConceptsRelation.DeleteAsync(new List<Guid> { employeeConceptsRelationID }, SessionModel.CompanyID);
                scope.Complete();
            }

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

        [DataContract]
        public class FonacotMovementDTO
        {
            /// <summary>
            /// ID
            /// </summary>
            [DataMember]
            public Guid ID { get; set; }

            /// <summary>
            /// Empleado ID
            /// </summary>
            [DataMember]
            public Guid EmployeeID { get; set; }

            /// <summary>
            /// Concepto (Deducción 61)
            /// </summary>
            [DataMember]
            public Guid ConceptPaymentID { get; set; }

            /// <summary>
            /// Relación a los conceptos del empleado
            /// </summary>
            [DataMember]
            public Guid EmployeeConceptsRelationID { get; set; }

            /// <summary>
            /// No. crédito
            /// </summary>
            [DataMember]
            public string CreditNumber { get; set; }

            /// <summary>
            /// Mes
            /// </summary>
            [DataMember]
            public int Month { get; set; }

            /// <summary>
            /// Año
            /// </summary>
            [DataMember]
            public int Year { get; set; }

            /// <summary>
            /// Tipo de retención
            /// </summary>
            [DataMember]
            public RetentionType RetentionType { get; set; }

            /// <summary>
            /// Estado: Activo / Inactivo
            /// </summary>
            [DataMember]
            public FonacotMovementStatus FonacotMovementStatus { get; set; }

            /// <summary>
            /// Observaciones
            /// </summary>
            [DataMember]
            public string Observations { get; set; }

            /// <summary>
            /// Observaciones
            /// </summary>
            [DataMember]
            public string Description { get; set; }

            //----------------------EmployeeConceptsRelation
            /// <summary>
            /// Monto del crédito
            /// </summary>
            [DataMember]
            public decimal CreditAmount { get; set; }

            /// <summary>
            /// Valor --- Descuento / Incremento mensual/periodo
            /// </summary>
            [DataMember]
            public decimal OverdraftDetailValue { get; set; }

            /// <summary>
            /// Monto --- Descuento / Incremento mensual/periodo
            /// </summary>
            [DataMember]
            public decimal OverdraftDetailAmount { get; set; }

            /// <summary>
            /// Pagos hechos por fuera
            /// </summary>
            [DataMember]
            public decimal PaymentsMadeByOtherMethod { get; set; }
        }
    }
}