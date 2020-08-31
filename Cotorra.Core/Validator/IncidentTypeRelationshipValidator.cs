using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class IncidentTypeRelationshipValidator : IValidator<IncidentTypeRelationship>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<IncidentTypeRelationship> MiddlewareManager { get; set; }
        public IncidentTypeRelationshipValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 8001));
            createRules.Add(new GuidRule("IncidentTypeID", "Tipo de Incidente", 8002));
            createRules.Add(new GuidRule("AccumulatedTypeID", "Tipo de Acumulado", 8003));
        }

        public void AfterCreate(List<IncidentTypeRelationship> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<IncidentTypeRelationship> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<IncidentTypeRelationship> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<IncidentTypeRelationship>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<IncidentTypeRelationship> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<IncidentTypeRelationship>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
