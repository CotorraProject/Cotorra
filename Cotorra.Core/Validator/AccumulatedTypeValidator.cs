using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class AccumulatedTypeValidator : IValidator<AccumulatedType>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<AccumulatedType> MiddlewareManager { get; set; }
        public AccumulatedTypeValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 8001));
            createRules.Add(new SimpleStringRule("Name", "Nombre", true, 1, 40, 8002));
        }

        public void AfterCreate(List<AccumulatedType> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<AccumulatedType> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<AccumulatedType> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<AccumulatedType>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<AccumulatedType> lstObjectsToValidate)
        {
            var validator = new RuleValidator<AccumulatedType>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
