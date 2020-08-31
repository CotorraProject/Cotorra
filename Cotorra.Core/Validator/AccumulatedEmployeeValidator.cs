using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class AccumulatedEmployeeValidator : IValidator<AccumulatedEmployee>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<AccumulatedEmployee> MiddlewareManager { get; set; }

        public AccumulatedEmployeeValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 2003));
        }

        public void AfterCreate(List<AccumulatedEmployee> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<AccumulatedEmployee> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<AccumulatedEmployee> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<AccumulatedEmployee>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<AccumulatedEmployee> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<AccumulatedEmployee>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}