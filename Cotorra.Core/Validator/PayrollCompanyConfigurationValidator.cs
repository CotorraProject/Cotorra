using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class PayrollCompanyConfigurationValidator : IValidator<PayrollCompanyConfiguration>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();
        public IMiddlewareManager<PayrollCompanyConfiguration> MiddlewareManager { get; set; }
        public PayrollCompanyConfigurationValidator()
        {
            createRules.Add(new GuidRule("company", "ID de compañia", 1301));
            createRules.Add(new GuidRule("InstanceID", "ID de instancia", 1302));
            createRules.Add(new GuidRule("CurrencyID", "ID de moneda", 1303));
            createRules.Add(new IntRule("CurrentExerciseYear", "Ejercicio actual ", 2000, int.MaxValue, 1304));
            createRules.Add(new DoubleRule("NonDeducibleFactor", "Factor deducible", 0, 1, 1305));
            createRules.Add(new IntRule("CurrentPeriod", "Periodo inicial", 1, 100, 1306));
        }

        public void AfterCreate(List<PayrollCompanyConfiguration> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<PayrollCompanyConfiguration> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<PayrollCompanyConfiguration> lstObjectsToValidate)
        {           
            var validator = new RuleValidator<PayrollCompanyConfiguration>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<PayrollCompanyConfiguration> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<PayrollCompanyConfiguration>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
