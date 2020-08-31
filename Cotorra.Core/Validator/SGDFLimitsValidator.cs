using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class SGDFLimitsValidator : IValidator<SGDFLimits>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();
        public IMiddlewareManager<SGDFLimits> MiddlewareManager { get; set; }
        public SGDFLimitsValidator()
        {

        }

        public void AfterCreate(List<SGDFLimits> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<SGDFLimits> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<SGDFLimits> lstObjectsToValidate)
        {
            var validator = new RuleValidator<SGDFLimits>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<SGDFLimits> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<SGDFLimits>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
