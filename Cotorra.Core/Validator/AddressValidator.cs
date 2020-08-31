using Cotorra.Schema;
using System;
using System.Collections.Generic;

namespace Cotorra.Core.Validator
{
    public class AddressValidator : IValidator<Address>
    {
        private readonly List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<Address> MiddlewareManager { get; set; }
        public AddressValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 2003));
            createRules.Add(new SimpleStringRule("ZipCode", "Código Postal", true, 5, 5, 3005));
            createRules.Add(new SimpleStringRule("FederalEntity", "Entidad", true, 1, 100, 3006));
        }

        public void AfterCreate(List<Address> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<Address> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<Address> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<Address>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<Address> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<Address>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
