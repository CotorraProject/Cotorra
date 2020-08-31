using Cotorra.Core.Managers.Calculation;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class InfonavitMovementValidator : IValidator<InfonavitMovement>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<InfonavitMovement> MiddlewareManager { get; set; }
        public InfonavitMovementValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 19001));
            createRules.Add(new GuidRule("EmployeeID", "Empleado", 19002));
            createRules.Add(new DecimalRule("MonthlyFactor", "Factor mensual", 1, 999999999, 19003));
            createRules.Add(new DecimalRule("AccumulatedAmount", "Monto acumulado", 0, 9999999, 19004));
            createRules.Add(new DecimalRule("AppliedTimes", "Veces aplicado", 0, 9999, 19005));

            createRules.Add(new DuplicateItemRule<InfonavitMovement>(new string[] { "CreditNumber" }, "Número de crédito", this, 19050));
        }

        public void AfterCreate(List<InfonavitMovement> lstObjectsToValidate)
        {
            ValidateCreditRules(lstObjectsToValidate);
            CreateConcurrentConcepts(lstObjectsToValidate);

        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        /// <summary>
        /// Borra los conceptos de infonavit que no deberían de estar ya, por actualización
        /// </summary>
        /// <param name="lstObjectsToValidate"></param>
        private void DeleteOverdraftConcepts(List<InfonavitMovement> lstObjectsToValidate)
        {
            var middlewareManagerOverdraft = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(),
                new OverdraftValidator());
            var middlewareManagerOverdraftDetails = new MiddlewareManager<OverdraftDetail>(new BaseRecordManager<OverdraftDetail>(),
                new OverdraftDetailValidator());

            var employeeIds = lstObjectsToValidate.Select(p => p.EmployeeID);
            var companyID = lstObjectsToValidate.FirstOrDefault().company;
            var overdraftActive = middlewareManagerOverdraft.FindByExpression(p =>
                    p.PeriodDetail.PeriodStatus == PeriodStatus.Calculating &&
                    employeeIds.Contains(p.EmployeeID), companyID
                    , new string[] { "OverdraftDetails", "OverdraftDetails.ConceptPayment" });

            if (overdraftActive.Any())
            {
                var overdraft = overdraftActive.FirstOrDefault();
                lstObjectsToValidate.ForEach(infonavitMovement =>
                {
                    if (infonavitMovement.InfonavitCreditType == InfonavitCreditType.DiscountFactor_D15)
                    {
                        var detailsIds = overdraft.OverdraftDetails
                        .Where(p =>
                            (p.ConceptPayment.Code == 59 || p.ConceptPayment.Code == 16) &&
                             p.ConceptPayment.ConceptType == ConceptType.DeductionPayment)
                        .Select(p => p.ID)
                        .ToList();

                        if (detailsIds.Any())
                        {
                            middlewareManagerOverdraftDetails.Delete(detailsIds, companyID);
                        }
                    }
                    else if (infonavitMovement.InfonavitCreditType == InfonavitCreditType.FixQuota_D16)
                    {
                        var detailsIds = overdraft.OverdraftDetails
                          .Where(p =>
                              (p.ConceptPayment.Code == 59 || p.ConceptPayment.Code == 15) &&
                               p.ConceptPayment.ConceptType == ConceptType.DeductionPayment)
                          .Select(p => p.ID)
                          .ToList();

                        if (detailsIds.Any())
                        {
                            middlewareManagerOverdraftDetails.Delete(detailsIds, companyID);
                        }
                    }
                    else if (infonavitMovement.InfonavitCreditType == InfonavitCreditType.Percentage_D59)
                    {
                        var detailsIds = overdraft.OverdraftDetails
                          .Where(p =>
                              (p.ConceptPayment.Code == 16 || p.ConceptPayment.Code == 15) &&
                               p.ConceptPayment.ConceptType == ConceptType.DeductionPayment)
                          .Select(p => p.ID)
                          .ToList();

                        if (detailsIds.Any())
                        {
                            middlewareManagerOverdraftDetails.Delete(detailsIds, companyID);
                        }
                    }

                    //Seguro de vivienda infonavit
                    if (!infonavitMovement.IncludeInsurancePayment_D14)
                    {
                        var detailsIds = overdraft.OverdraftDetails
                        .Where(p =>
                            (p.ConceptPayment.Code == 14) &&
                             p.ConceptPayment.ConceptType == ConceptType.DeductionPayment)
                        .Select(p => p.ID)
                        .ToList();

                        var conceptIds = overdraft.OverdraftDetails
                        .Where(p =>
                            (p.ConceptPayment.Code == 14) &&
                             p.ConceptPayment.ConceptType == ConceptType.DeductionPayment)
                        .Select(p => p.ConceptPaymentID)
                        .ToList();

                        if (detailsIds.Any())
                        {
                            middlewareManagerOverdraftDetails.Delete(detailsIds, companyID);
                        }

                        //Delete employeeConceptRelated
                        var employeeConceptRelatedMiddleware = new MiddlewareManager<EmployeeConceptsRelation>(
                            new BaseRecordManager<EmployeeConceptsRelation>(), new EmployeeConceptsRelationValidator());
                        var employessRelatedToDelete = employeeConceptRelatedMiddleware.FindByExpression(p => 
                            p.EmployeeID == overdraft.EmployeeID &&
                            conceptIds.Contains(p.ConceptPaymentID), companyID);
                        var employessRelatedToDeleteIds = employessRelatedToDelete.Select(p => p.ID).ToList();
                        employeeConceptRelatedMiddleware.Delete(employessRelatedToDeleteIds, companyID);
                    }
                });
            }
        }

        public void AfterUpdate(List<InfonavitMovement> lstObjectsToValidate)
        {
            ValidateCreditRules(lstObjectsToValidate);
            CreateConcurrentConcepts(lstObjectsToValidate);
            DeleteOverdraftConcepts(lstObjectsToValidate);
        }

        public void BeforeCreate(List<InfonavitMovement> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<InfonavitMovement>();
            validator.ValidateRules(lstObjectsToValidate, createRules);

            var employeesIDs = lstObjectsToValidate.Select(x => x.EmployeeID).ToList();

            var middlewareManager = new MiddlewareManager<InfonavitMovement>(new BaseRecordManager<InfonavitMovement>(), this);

            var entities = middlewareManager.FindByExpression(p => employeesIDs.Contains(p.EmployeeID) && p.Active, Guid.Empty, new string[] { });

            if (entities.Any())
            {
                throw new CotorraException(801, "801", "Ya se ha registrado un crédito infonavit activo para el empleado", null);
            }

        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public object BeforeDelete(List<Guid> lstObjectsToValidate, Guid identityWorkID)
        {
            var toDelete = MiddlewareManager.FindByExpressionAsync(x => lstObjectsToValidate.Contains(x.ID) && x.Active, Guid.Empty, new string[] { "EmployeeConceptsRelation" }).Result;
            List<EmployeeConceptsRelation> toDeleteAfter = new List<EmployeeConceptsRelation>();
            toDeleteAfter.AddRange(toDelete.Select(x => x.EmployeeConceptsRelation));
            return toDeleteAfter;
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate, object parameters)
        {
            List<EmployeeConceptsRelation> listAfterDelete = (List<EmployeeConceptsRelation>)parameters;
            var emplRelManager = new MiddlewareManager<EmployeeConceptsRelation>(new BaseRecordManager<EmployeeConceptsRelation>(),
                  new EmployeeConceptsRelationValidator());
            emplRelManager.DeleteAsync(listAfterDelete.Select(x => x.ID).ToList(), listAfterDelete.FirstOrDefault().company).Wait();
        }

        public void BeforeUpdate(List<InfonavitMovement> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<InfonavitMovement>();
            validator.ValidateRules(lstObjectsToValidate, createRules);

            lstObjectsToValidate.ForEach(p => 
            {
                if (!p.IncludeInsurancePayment_D14)
                {
                    p.EmployeeConceptsRelationInsuranceID = null;
                }
            });
        }

        public void ValidateCreditRules(IEnumerable<InfonavitMovement> lstObjectsToValidate)
        {
            var employeeConceptsRelationValidator = new EmployeeConceptsRelationValidator();
            employeeConceptsRelationValidator.AfterUpdateCreateOrigin(lstObjectsToValidate.Select(p => p.EmployeeConceptsRelation), false);
        }

        public void CreateConcurrentConcepts(List<InfonavitMovement> lstObjectsToValidate)
        {
            //verify if the concurrent concepts exists or modify
            var instanceID = lstObjectsToValidate.FirstOrDefault().InstanceID;
            var companyID = lstObjectsToValidate.FirstOrDefault().company;
            var user = lstObjectsToValidate.FirstOrDefault().user;
            new EmployeeConceptsRelationManager().CreateConcurrentConceptsAsync(instanceID, companyID, user).Wait();
        }

    }
}
