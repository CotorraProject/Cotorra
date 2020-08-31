using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class UMAValidator : IValidator<UMA>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<UMA> MiddlewareManager { get; set; }
        public UMAValidator()
        {
            createRules.Add(new DecimalRule("Value", "Valor UMA", 0, 999999.99M, 9003));
            //createRules.Add(new DuplicateItemRule<UMA>(new string[] {"InitialDate" },
            //   "No se puede duplicar la UMA para esa fecha proporcionada", this, 9006));
        }

        public void AfterCreate(List<UMA> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<UMA> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<UMA> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<UMA>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<UMA> lstObjectsToValidate)
        {
            var validator = new RuleValidator<UMA>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
