using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class WorkCenterValidator : IValidator<WorkCenter>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<WorkCenter> MiddlewareManager { get; set; }
        public WorkCenterValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 6901));
            //createRules.Add(new DuplicateItemRule<WorkCenter>(new string[] { "Key" }, "Clave", this, 6902));

        }

        public void AfterCreate(List<WorkCenter> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<WorkCenter> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<WorkCenter> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<WorkCenter>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<WorkCenter> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<WorkCenter>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
