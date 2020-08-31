using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class PermanentMovementValidator : IValidator<PermanentMovement>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<PermanentMovement> MiddlewareManager { get; set; }
        public PermanentMovementValidator()
        {
            //createRules.Add(new SimpleStringRule("Name", "Nombre", true, 1, 100, 110001));
        }

        public void AfterCreate(List<PermanentMovement> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<PermanentMovement> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<PermanentMovement> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<PermanentMovement>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<PermanentMovement> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<PermanentMovement>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
