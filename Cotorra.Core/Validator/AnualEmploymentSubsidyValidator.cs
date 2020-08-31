using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class AnualEmploymentSubsidyValidator : IValidator<AnualEmploymentSubsidy>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<AnualEmploymentSubsidy> MiddlewareManager { get; set; }
        public AnualEmploymentSubsidyValidator()
        {
            //createRules.Add(new DecimalRule("Value", "Valor UMA", 0, 999999.99M, 9003));
            //createRules.Add(new DuplicateItemRule<UMA>(new string[] {"InitialDate" },
            //   "No se puede duplicar la UMA para esa fecha proporcionada", this, 9006));
        }

        public void AfterCreate(List<AnualEmploymentSubsidy> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<AnualEmploymentSubsidy> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<AnualEmploymentSubsidy> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<AnualEmploymentSubsidy>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<AnualEmploymentSubsidy> lstObjectsToValidate)
        {
            var validator = new RuleValidator<AnualEmploymentSubsidy>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
