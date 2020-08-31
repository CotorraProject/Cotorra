using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class SettlementCatalogValidator : IValidator<SettlementCatalog>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<SettlementCatalog> MiddlewareManager { get; set; }
       
        public SettlementCatalogValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 1003));
            createRules.Add(new DuplicateItemRule<SettlementCatalog>(new string[] { "ValidityDate" }, "Fecha de validez", this, 1002));
        }

        public void AfterCreate(List<SettlementCatalog> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<SettlementCatalog> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<SettlementCatalog> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<SettlementCatalog>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<SettlementCatalog> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<SettlementCatalog>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
