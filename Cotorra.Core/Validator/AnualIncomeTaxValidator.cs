using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class AnualIncomeTaxValidator : IValidator<AnualIncomeTax>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<AnualIncomeTax> MiddlewareManager { get; set; }
        public AnualIncomeTaxValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 5001));
            createRules.Add(new DecimalRule("LowerLimit", "Límite inferior", 0, 999999999.99m, 5002));
            createRules.Add(new DecimalRule("FixedFee", "Cuota fija", 0, 999999999.99m, 5003));
            //createRules.Add(new DecimalRule("Rate", "Tasa", 0, 1, 5004));
        }

        public void AfterCreate(List<AnualIncomeTax> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<AnualIncomeTax> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<AnualIncomeTax> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<AnualIncomeTax>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<AnualIncomeTax> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<AnualIncomeTax>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
