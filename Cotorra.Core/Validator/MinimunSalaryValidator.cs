using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class MinimunSalaryValidator : IValidator<MinimunSalary>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<MinimunSalary> MiddlewareManager { get; set; }
        public MinimunSalaryValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 9002));
            createRules.Add(new DecimalRule("ZoneA", "Zona A", 0m, 999999.99m, 9003));
            createRules.Add(new DecimalRule("ZoneB", "Zona B", 0m, 999999.99m, 9004));
            createRules.Add(new DecimalRule("ZoneC", "Zona C", 0m, 999999.99m, 9005));

            //createRules.Add(new DuplicateItemRule<MinimunSalary>(new string[] { "InstanceID", "ExpirationDate" }, 
            //    "No se puede duplicar el salario mínimo para esa fecha proporcionada", this, 9006));
        }

        public void AfterCreate(List<MinimunSalary> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<MinimunSalary> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<MinimunSalary> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<MinimunSalary>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<MinimunSalary> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<MinimunSalary>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
