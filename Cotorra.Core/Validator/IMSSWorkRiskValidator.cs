using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class IMSSWorkRiskValidator : IValidator<IMSSWorkRisk>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();
        public IMiddlewareManager<IMSSWorkRisk> MiddlewareManager { get; set; }
        public IMSSWorkRiskValidator()
        {

        }

        public void AfterCreate(List<IMSSWorkRisk> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<IMSSWorkRisk> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<IMSSWorkRisk> lstObjectsToValidate)
        {
            var validator = new RuleValidator<IMSSWorkRisk>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<IMSSWorkRisk> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<IMSSWorkRisk>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
