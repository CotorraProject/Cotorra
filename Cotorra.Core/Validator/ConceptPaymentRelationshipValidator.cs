using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class ConceptPaymentRelationshipValidator : IValidator<ConceptPaymentRelationship>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<ConceptPaymentRelationship> MiddlewareManager { get; set; }

        public ConceptPaymentRelationshipValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 6001));
        }

        public void AfterCreate(List<ConceptPaymentRelationship> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<ConceptPaymentRelationship> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<ConceptPaymentRelationship> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<ConceptPaymentRelationship>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<ConceptPaymentRelationship> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<ConceptPaymentRelationship>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
