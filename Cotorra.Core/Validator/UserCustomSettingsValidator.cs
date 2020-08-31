using Cotorra.Schema;
using System;
using System.Collections.Generic;

namespace Cotorra.Core.Validator
{
    public class UserCustomSettingsValidator : IValidator<UserCustomSettings>
    {
        private readonly List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<UserCustomSettings> MiddlewareManager { get; set; }
        public UserCustomSettingsValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 2003));
            createRules.Add(new DuplicateItemRule<UserCustomSettings>(new string[] { "Key" }, "La llave proporcionada ya existe", this, 2002));
        }

        public void AfterCreate(List<UserCustomSettings> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<UserCustomSettings> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<UserCustomSettings> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<UserCustomSettings>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<UserCustomSettings> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<UserCustomSettings>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
