using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class MonthlyIncomeTaxValidator : IValidator<MonthlyIncomeTax>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<MonthlyIncomeTax> MiddlewareManager { get; set; }
        public MonthlyIncomeTaxValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 2003));
            createRules.Add(new DecimalRule("LowerLimit", "Limite inferior", 0, 999999999.99m, 4001));
            createRules.Add(new DecimalRule("FixedFee", "Cuota fija", 0, 999999999.99m, 4002));
            //createRules.Add(new DecimalRule("Rate", "Tasa", 0, 1, 4003));

        }

        public void AfterCreate(List<MonthlyIncomeTax> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<MonthlyIncomeTax> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<MonthlyIncomeTax> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<MonthlyIncomeTax>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<MonthlyIncomeTax> lstObjectsToValidate)
        {
            var validator = new RuleValidator<MonthlyIncomeTax>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
