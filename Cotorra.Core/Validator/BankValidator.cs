using Cotorra.Schema;
using System;
using System.Collections.Generic;

namespace Cotorra.Core.Validator
{
    public class BankValidaror : IValidator<Bank>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<Bank> MiddlewareManager { get; set; }
        public BankValidaror()
        {
            createRules.Add(new SimpleStringRule("Name", "Nombre", true, 1, 50, 2001));
            createRules.Add(new IntRule("Code", "Código", 1, 9999, 2004));
        }

        public void AfterCreate(List<Bank> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<Bank> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<Bank> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<Bank>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<Bank> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<Bank>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
