using MoreLinq.Extensions;
using Cotorra.Core.Managers;
using Cotorra.Core.Managers.Calculation;
using Cotorra.Core.Utils;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Cotorra.Schema.Calculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;

namespace Cotorra.Core
{
    public class SettlementProcessManager
    {
        const int ISRFINIQUITOCONCEPTCODE = 101;
        List<string> COMPENSATIONSATCODES = new List<string>() { "P-022", "P-023", "P-025" };

        ISettlementProcessManagerLetterHelper letterHelper;

        public SettlementProcessManager()
        {
            letterHelper = new SettlementProcessManagerLetterHelper();
        }

        public async Task<List<Overdraft>> Calculate(CalculateSettlementProcessParams parameters)
        {
            Guid identityWorkID = parameters.IdentityWorkID;
            Guid instanceId = parameters.InstanceID;
            Guid periodDetailID = parameters.PeriodDetailID;
            Guid employeeID = parameters.EmployeeID;
            var oversGenerated = new List<Overdraft>();

            var codes = parameters.ConceptsToApply.Select(x => x.Code).ToList();
            codes.Add(ISRFINIQUITOCONCEPTCODE);

            var allConcepts = await GetAllConceptsSettlement(codes, COMPENSATIONSATCODES, identityWorkID,
               instanceId);

            var employeeManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
            var employee = employeeManager.FindByExpressionAsync(x => x.ID == employeeID && x.InstanceID == instanceId && x.Active == true, identityWorkID);

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var previousSettlementOverdrafts = await GetOverdrafts(parameters.EmployeeID,
                    parameters.InstanceID, parameters.IdentityWorkID,
                    new List<OverdraftType>() { OverdraftType.OrdinarySettlement,
                        OverdraftType.CompensationSettlement });
                await UpdateEmployee(parameters, (await employee).FirstOrDefault());

                await DeleteOverdrafts(previousSettlementOverdrafts, parameters.IdentityWorkID);

                oversGenerated.Add(await GenerateOrdinaryOverdraft(parameters, allConcepts));
                var compensation = await GenerateCompensationOverdraft(parameters, allConcepts);
                if (compensation != null)
                {
                    oversGenerated.Add(compensation);
                }

                scope.Complete();
            }

            oversGenerated.ForEach(over => over.OverdraftDetails = over.OverdraftDetails.OrderBy(x => x.ConceptPayment.Code).ToList());
            return oversGenerated;
        }


        /// <summary>
        /// Calculates the overdraft for settlement.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public async Task<Overdraft> GenerateOrdinaryOverdraft(CalculateSettlementProcessParams parameters, List<ConceptPayment> allConcepts)
        {

            var activeOverdraft = (await GetOverdrafts(parameters.EmployeeID, parameters.InstanceID,
                parameters.IdentityWorkID, parameters.PeriodDetailID, new List<OverdraftType>() { OverdraftType.Ordinary })).FirstOrDefault();

            if (activeOverdraft == null)
            {
                throw new CotorraException(45001, "45001", "No es posible finiquitar a este empleado no tiene recibos activos.", null);
            }

            var reportedForUserConceptsOrdinarySettlementDetail = activeOverdraft.OverdraftDetails.Where(x => x.IsTotalAmountCapturedByUser || x.IsValueCapturedByUser || x.IsAmount1CapturedByUser || x.IsAmount2CapturedByUser || x.IsAmount3CapturedByUser || x.IsAmount4CapturedByUser);

            var settlementConcepts = allConcepts;
            var conceptsFromSettlementScreen = settlementConcepts.Where(x => parameters.ConceptsToApply.Select(x => x.Code).Contains(x.Code));
            var automaticSettlementConcepts = settlementConcepts.Where(x => x.AutomaticDismissal);

            var selectedCodesByUser = parameters.ConceptsToApply.Where(x => x.Apply).Select(y => y.Code);
            var unSelectedCodesByUser = parameters.ConceptsToApply.Where(x => !x.Apply).Select(y => y.Code);
            var conceptsSelectedByUser = conceptsFromSettlementScreen.Where(x => selectedCodesByUser.Contains(x.Code));


            List<ConceptPayment> conceptsToADD = new List<ConceptPayment>();
            List<OverdraftDetail> overdraftDetailsToADD = new List<OverdraftDetail>();


            //Get Overdraft

            var settlementOvedraft = BuildOverdraft(parameters.IdentityWorkID, parameters.InstanceID, parameters.user, parameters.EmployeeID, parameters.PeriodDetailID, OverdraftType.OrdinarySettlement);
            ///Revisamos los capturados por el usuario en el recibo ordinario
            reportedForUserConceptsOrdinarySettlementDetail.ForEach(overdraftDetail =>
            {
                //si esta en la lista de rubros
                if (conceptsFromSettlementScreen.Any(x => x.ID == overdraftDetail.ConceptPaymentID))
                {
                    //Si aplica
                    if (conceptsSelectedByUser.Any(y => y.ID == overdraftDetail.ConceptPaymentID))
                    {
                        var capturedData = parameters.ConceptsToApply.FirstOrDefault(p => p.Code == overdraftDetail.ConceptPayment.Code);
                        var overDetail = BuildEmptyOverdraftDetail(parameters.IdentityWorkID, overdraftDetail.ConceptPaymentID, parameters.InstanceID, settlementOvedraft.ID, parameters.user, capturedData.TotalDays);
                        overdraftDetailsToADD.Add(overDetail);
                    }
                    //Cuando no aplica no se agrega
                }
                else
                {
                    //Si no tiene que ver con la lista se manda como va pero en un objeto nuevo
                    overdraftDetail.ID = Guid.NewGuid();
                    overdraftDetail.OverdraftID = settlementOvedraft.ID;
                    overdraftDetail.ConceptPayment = null;
                    overdraftDetailsToADD.Add(overdraftDetail);
                }
            });


            settlementConcepts.ForEach(automaticSettlementConcept =>
            {
                //si esta en la lista de rubros
                if (conceptsFromSettlementScreen.Any(x => x.ID == automaticSettlementConcept.ID))
                {
                    //Si aplica
                    if (conceptsSelectedByUser.Any(y => y.ID == automaticSettlementConcept.ID))
                    {
                        var capturedData = parameters.ConceptsToApply.FirstOrDefault(p => p.Code == automaticSettlementConcept.Code);
                        var overDetail = BuildEmptyOverdraftDetail(parameters.IdentityWorkID, automaticSettlementConcept.ID, parameters.InstanceID, settlementOvedraft.ID, parameters.user, capturedData.TotalDays);
                        overdraftDetailsToADD.Add(overDetail);
                    }
                    //Cuando no aplica no se agrega
                }
                else
                {
                    int value = 0;
                    if (automaticSettlementConcept.Code == 29 && automaticSettlementConcept.ConceptType == ConceptType.SalaryPayment)
                    {
                        var conceptToOverride = parameters.ConceptsToApply.Where(x => x.Code == 29).FirstOrDefault();
                        if (conceptToOverride != null)
                        {
                            value = (int)conceptToOverride.TotalDays;
                        }
                    }
                    //Si no tiene que ver con la lista se genera el detail
                    var overDetail = BuildEmptyOverdraftDetail(parameters.IdentityWorkID, automaticSettlementConcept.ID, parameters.InstanceID, settlementOvedraft.ID, parameters.user, value);//cero para automatico??
                    overdraftDetailsToADD.Add(overDetail);
                }
            });


            settlementOvedraft.OverdraftDetails = overdraftDetailsToADD;
            await SaveOverdraft(settlementOvedraft, parameters.IdentityWorkID);

            return await Calculate(settlementOvedraft, parameters.IdentityWorkID,
                parameters.InstanceID, parameters.user);
        }


        /// <summary>
        /// Applies the settlement.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public async Task<ApplySettlementProcessResult> ApplySettlement(ApplySettlementProcessParams parameters)
        {
            var activeOverdraft = (await GetOverdrafts(parameters.EmployeeID, parameters.InstanceID,
               parameters.IdentityWorkID, parameters.PeriodDetailID,
               new List<OverdraftType>() { OverdraftType.Ordinary }));
            var mgr = new AuthorizationManager();
            var overdrafts = new List<Guid>();
            if (parameters.OrdinaryID != Guid.Empty)
            {
                overdrafts.Add(parameters.OrdinaryID);
            }

            if (parameters.OrdinaryID != Guid.Empty)
            {
                overdrafts.Add(parameters.IndemnizationOverID);
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (parameters.ChangeOverdrafts)
                {
                    await UpdateDetails(parameters.OrdinaryID, parameters.IndemnizationOverID,
                        parameters.OrdinaryOverDetailsIDs, parameters.IndemnizationOverDetailsIDs,
                        parameters.IdentityWorkID, parameters.InstanceID);
                }
                await mgr.AuthorizationByOverdraftAsync(new AuthorizationByOverdraftParams()
                {
                    IdentityWorkID = parameters.IdentityWorkID,
                    InstanceID = parameters.InstanceID,
                    OverdraftIDs = overdrafts,
                    user = parameters.user
                });

                await UnregisterEmployee(parameters);
                await DeleteOverdrafts(activeOverdraft, parameters.IdentityWorkID);
                scope.Complete();
            }

            return new ApplySettlementProcessResult();
        }


        private async Task UpdateEmployee(CalculateSettlementProcessParams parameters, Employee employee)
        {
            employee.SettlementSalaryBase = parameters.SettlementBaseSalary;
            var employeeManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
            await employeeManager.UpdateAsync(new List<Employee>() { employee }, parameters.IdentityWorkID);
        }

        private Overdraft BuildOverdraft(Guid identityWorkId, Guid instanceId, Guid userId, Guid employeId, Guid periodDetailId,
            OverdraftType overdraftType)
        {
            return new Overdraft()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceId,
                Description = "Sobrerecibo finiquito",
                CreationDate = DateTime.Now,
                Name = "Sobrerecibo finiquito",
                StatusID = 1,
                EmployeeID = employeId,
                user = userId,
                PeriodDetailID = periodDetailId,
                OverdraftType = overdraftType,
                OverdraftStatus = OverdraftStatus.None
            };
        }

        private OverdraftDetail BuildEmptyOverdraftDetail(Guid identityWorkId, Guid conceptId, Guid instanceId, Guid overdraftId, Guid userId, decimal value)
        {
            return new OverdraftDetail
            {
                Active = true,
                Amount = 0,
                Taxed = 0,
                Exempt = 0,
                IMSSTaxed = 0,
                IMSSExempt = 0,
                company = identityWorkId,
                ConceptPaymentID = conceptId,
                CreationDate = DateTime.UtcNow,
                Name = "",
                Description = "",
                ID = Guid.NewGuid(),
                InstanceID = instanceId,
                IsAmount1CapturedByUser = false,
                IsAmount2CapturedByUser = false,
                IsAmount3CapturedByUser = false,
                IsAmount4CapturedByUser = false,
                IsTotalAmountCapturedByUser = false,
                IsValueCapturedByUser = false,
                Label1 = "",
                Label2 = "",
                Label3 = "",
                Label4 = "",
                OverdraftID = overdraftId,
                StatusID = 1,
                Timestamp = DateTime.UtcNow,
                user = userId,
                Value = value
            };

        }


        private async Task UnregisterEmployee(ApplySettlementProcessParams parameters)
        {
            var statusManager = new StatusManager<Employee>(new EmployeeValidator());
            await statusManager.SetUnregistered(new List<Guid>() { parameters.EmployeeID }, parameters.IdentityWorkID,
                parameters);
        }

        private async Task<Overdraft> GenerateCompensationOverdraft(CalculateSettlementProcessParams parameters, List<ConceptPayment> allConcepts)
        {
            var satCodes = COMPENSATIONSATCODES;

            var compensationConcepts = allConcepts.Where(x => x.Code == ISRFINIQUITOCONCEPTCODE || satCodes.Contains(x.SATGroupCode));
            var conceptsFromSettlementScreen = compensationConcepts.Where(x => parameters.ConceptsToApply.Select(x => x.Code).Contains(x.Code));
            var selectedCodesByUser = parameters.ConceptsToApply.Where(x => x.Apply).Select(y => y.Code);

            var conceptsSelectedByUser = conceptsFromSettlementScreen.Where(x => selectedCodesByUser.Contains(x.Code));


            var overdraft = BuildOverdraft(parameters.IdentityWorkID, parameters.InstanceID, parameters.user, parameters.EmployeeID, parameters.PeriodDetailID, OverdraftType.CompensationSettlement);
            overdraft.Name = "Sobrerecibo finiquito compensación";
            overdraft.Description = "Sobrerecibo finiquito compensación";
            var applyIndemnization = compensationConcepts.Any(x => selectedCodesByUser.Contains(x.Code));

            if (!applyIndemnization)
            { return null; }

            List<OverdraftDetail> overdraftDetailsToADD = new List<OverdraftDetail>();

            compensationConcepts.ForEach(concept =>
            {
                //si esta en la lista de rubros
                if (conceptsFromSettlementScreen.Any(x => x.ID == concept.ID))
                {
                    //Si aplica
                    if (conceptsSelectedByUser.Any(y => y.ID == concept.ID))
                    {
                        var capturedData = parameters.ConceptsToApply.FirstOrDefault(p => p.Code == concept.Code);
                        var overDetail = BuildEmptyOverdraftDetail(parameters.IdentityWorkID, concept.ID, parameters.InstanceID, overdraft.ID, parameters.user, capturedData.TotalDays);
                        overdraftDetailsToADD.Add(overDetail);

                    }
                    //Cuando no aplica no se agrega
                }
                else
                {
                    int value = 0;
                    //Si no tiene que ver con la lista se genera el detail
                    var overDetail = BuildEmptyOverdraftDetail(parameters.IdentityWorkID, concept.ID, parameters.InstanceID, concept.ID, parameters.user, value);//cero para automatico
                    overdraftDetailsToADD.Add(overDetail);
                }
            });

            overdraft.OverdraftDetails = overdraftDetailsToADD;

            var liabilitesDetails = GenerateLiabilities(parameters, allConcepts);
            overdraft.OverdraftDetails.AddRange(liabilitesDetails);

            await SaveOverdraft(overdraft, parameters.IdentityWorkID);

            return await Calculate(overdraft, parameters.IdentityWorkID, parameters.InstanceID, parameters.user);
        }


        private List<OverdraftDetail> GenerateLiabilities(CalculateSettlementProcessParams parameters, List<ConceptPayment> allConcepts)
        {
            var liabilitiConcepts = allConcepts.Where(x => x.ConceptType == ConceptType.LiabilityPayment && x.AutomaticDismissal);

            var overdraftDetails = new List<OverdraftDetail>();


            liabilitiConcepts.ForEach(concept =>
            {
                var overDetail = BuildEmptyOverdraftDetail(parameters.IdentityWorkID, concept.ID, parameters.InstanceID, concept.ID, parameters.user, 0);
                overdraftDetails.Add(overDetail);

            });

            return overdraftDetails;
        }


        /// <summary>
        /// Generates the settlement letter.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public async Task<string> GenerateSettlementLetter(GenerateSettlementLetterParams parameters)
        {
            return await letterHelper.GenerateSettlementLetter(parameters, new MSSpreadsheetWriterBlob());
        }

        /// <summary>
        /// Generates the settlement letter indicates writer.
        /// </summary>
        /// <param name="activeOverdraft">The active overdraft.</param>
        /// <param name="identityWorkID">The identity work identifier.</param>
        /// <param name="instanceID">The instance identifier.</param>
        /// <param name="writer">The writer.</param>
        /// <returns></returns>
        public async Task<string> GenerateSettlementLetter(List<Overdraft> activeOverdrafts, Guid identityWorkID, Guid instanceID, string token, IMSSpreadsheetWriter writer)
        {
            return await letterHelper.GenerateSettlementLetter(activeOverdrafts, identityWorkID, instanceID, token, writer);

        }

        private async Task SaveOverdraft(Overdraft settlementOverdraft, Guid identityWorkID)
        {
            var manager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
            await manager.CreateAsync(new List<Overdraft>() { settlementOverdraft }, identityWorkID);
        }



        private async Task<Overdraft> Calculate(Overdraft actualOverdraft, Guid identityWorkID, Guid instanceID, Guid user)
        {
            var mgr = new OverdraftCalculationManager();
            var result = await mgr.CalculateAsync(new CalculateOverdraftParams()
            {
                IdentityWorkID = identityWorkID,
                InstanceID = instanceID,
                DeleteAccumulates = false,
                OverdraftID = actualOverdraft.ID,
                ResetCalculation = false,
                UserID = user
            });
            return (result as CalculateOverdraftResult).OverdraftResult;
        }

        private async Task<List<ConceptPayment>> GetAllConceptsSettlement(IEnumerable<int> codes, IEnumerable<string> fiscalCodes, Guid identityWorkID, Guid instanceID)
        {
            var manager = new MiddlewareManager<ConceptPayment>(new BaseRecordManager<ConceptPayment>(), new ConceptPaymentValidator());
            return await manager.FindByExpressionAsync(x => instanceID == x.InstanceID &&
            (fiscalCodes.Contains(x.SATGroupCode) || codes.Contains(x.Code) || x.AutomaticDismissal), identityWorkID);
        }

        private async Task<List<Overdraft>> GetOverdrafts(Guid employeeID, Guid instanceID, Guid identityWorkID, Guid periodDetailID, List<OverdraftType> overdraftType)
        {
            var manager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
            List<Overdraft> overdrafts = null;
            if (periodDetailID != Guid.Empty)
            {
                overdrafts = await manager.FindByExpressionAsync(x => x.InstanceID == instanceID && x.EmployeeID == employeeID &&
                x.PeriodDetailID == periodDetailID && overdraftType.Contains(x.OverdraftType) &&
                x.OverdraftStatus == OverdraftStatus.None,
                identityWorkID, new string[] { "OverdraftDetails", "OverdraftDetails.ConceptPayment" });
            }
            else
            {
                throw new CotorraException(108, "108", "No se estableció el periodo correspondiente", null);
            }
            return overdrafts;
        }

        private async Task<List<Overdraft>> GetOverdrafts(Guid employeeID, Guid instanceID, Guid identityWorkID, List<OverdraftType> overdraftType)
        {
            var manager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
            List<Overdraft> overdrafts = null;

            overdrafts = await manager.FindByExpressionAsync(x =>
                x.InstanceID == instanceID &&
                x.EmployeeID == employeeID &&
                overdraftType.Contains(x.OverdraftType) &&
                x.OverdraftStatus == OverdraftStatus.None,
              identityWorkID, new string[] { "OverdraftDetails", "OverdraftDetails.ConceptPayment" });

            return overdrafts;
        }

        private async Task DeleteOverdrafts(List<Overdraft> overdrafts, Guid identityWorkID)
        {
            if (overdrafts != null && overdrafts.Any())
            {
                var detailManager = new MiddlewareManager<OverdraftDetail>(new BaseRecordManager<OverdraftDetail>(), new OverdraftDetailValidator());
                var overManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());

                var detailToDelete = overdrafts.SelectMany(x => x.OverdraftDetails).Select(x => x.ID).ToList();

                if (detailToDelete.Any())
                {
                    await detailManager.DeleteAsync(detailToDelete, identityWorkID);

                }

                await overManager.DeleteAsync(overdrafts.Select(x => x.ID).ToList(), identityWorkID);
            }

        }

        private async Task UpdateDetails(Guid OrdinaryID, Guid IndemnizationOverID, List<Guid> OrdinaryOverDetailsIDs,
            List<Guid> IndemnizationOverDetailsIDs, Guid identityWorkId, Guid instanceID)
        {
            var manager = new MiddlewareManager<OverdraftDetail>(new BaseRecordManager<OverdraftDetail>(), new OverdraftDetailValidator());
            var allGuids = new List<Guid>(OrdinaryOverDetailsIDs);
            allGuids.AddRange(IndemnizationOverDetailsIDs);

            var allDetailsDelete = await manager.FindByExpressionAsync(x => x.InstanceID == instanceID && (x.OverdraftID == IndemnizationOverID || x.OverdraftID == OrdinaryID), identityWorkId,
                new string[] { "ConceptPayment" });

            var allDetails = allDetailsDelete.Where(x => OrdinaryOverDetailsIDs.Contains(x.ID) || IndemnizationOverDetailsIDs.Contains(x.ID)).ToList();


            var oldOrdinaryDetailsCompensation = allDetailsDelete.Where(x => x.OverdraftID == OrdinaryID && (x.ConceptPayment.Code == ISRFINIQUITOCONCEPTCODE ||
             COMPENSATIONSATCODES.Contains(x.ConceptPayment.SATGroupCode))).ToList();

            allDetails.Where(x => OrdinaryOverDetailsIDs.Contains(x.ID)).AsParallel().ForAll(y => { y.OverdraftID = OrdinaryID; y.ConceptPayment = null; });
            //se elimina porke no hace nada
            //allDetails.Where(x => OrdinaryOverDetailsIDs.Contains(x.ID)).Where(p => p.Active);

            allDetails.Where(x => IndemnizationOverDetailsIDs.Contains(x.ID)).AsParallel().ForAll(y =>
            {
                y.OverdraftID = IndemnizationOverID; y.ConceptPayment = null;
            });

            await manager.UpdateAsync(allDetails, identityWorkId);

            if (oldOrdinaryDetailsCompensation.Any())
            {
                oldOrdinaryDetailsCompensation.AsParallel().ForAll(item => item.ConceptPayment = null);
                await manager.DeleteAsync(oldOrdinaryDetailsCompensation.Select(x => x.ID).ToList(), identityWorkId);
            }

        }


    }

}
