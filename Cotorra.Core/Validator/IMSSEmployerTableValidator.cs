using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class IMSSEmployerTableValidator : IValidator<IMSSEmployerTable>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();
        public IMiddlewareManager<IMSSEmployerTable> MiddlewareManager { get; set; }
        public IMSSEmployerTableValidator()
        {

        }

        public void AfterCreate(List<IMSSEmployerTable> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<IMSSEmployerTable> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<IMSSEmployerTable> lstObjectsToValidate)
        {
            var validator = new RuleValidator<IMSSEmployerTable>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<IMSSEmployerTable> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<IMSSEmployerTable>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
