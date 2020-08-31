using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class SettlementValidator : IValidator<Settlement>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();
        public IMiddlewareManager<Settlement> MiddlewareManager { get; set; }
        public SettlementValidator()
        {
            createRules.Add(new SimpleStringRule("Name", "Nombre", true, 1, 50, 2001));
            createRules.Add(new GuidRule("InstanceID", "Instancia", 2003));
            createRules.Add(new DuplicateItemRule<Settlement>(new string[] { "Name" }, "Nombre", this, 2002));
        }

        public void AfterCreate(List<Settlement> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<Settlement> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<Settlement> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<Settlement>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<Settlement> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<Settlement>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
