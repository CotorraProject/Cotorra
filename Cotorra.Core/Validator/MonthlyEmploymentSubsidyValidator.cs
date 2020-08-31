using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class MonthlyEmploymentSubsidyValidator : IValidator<MonthlyEmploymentSubsidy>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<MonthlyEmploymentSubsidy> MiddlewareManager { get; set; }
        public MonthlyEmploymentSubsidyValidator()
        {
            //createRules.Add(new DecimalRule("Value", "Valor UMA", 0, 999999.99M, 9003));
            //createRules.Add(new DuplicateItemRule<UMA>(new string[] {"InitialDate" },
            //   "No se puede duplicar la UMA para esa fecha proporcionada", this, 9006));
        }

        public void AfterCreate(List<MonthlyEmploymentSubsidy> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<MonthlyEmploymentSubsidy> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<MonthlyEmploymentSubsidy> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<MonthlyEmploymentSubsidy>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<MonthlyEmploymentSubsidy> lstObjectsToValidate)
        {
            var validator = new RuleValidator<MonthlyEmploymentSubsidy>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
