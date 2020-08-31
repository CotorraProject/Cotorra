using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class UMIValidator : IValidator<UMI>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<UMI> MiddlewareManager { get; set; }
        public UMIValidator()
        {
        }

        public void AfterCreate(List<UMI> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<UMI> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<UMI> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<UMI>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<UMI> lstObjectsToValidate)
        {
            var validator = new RuleValidator<UMI>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
