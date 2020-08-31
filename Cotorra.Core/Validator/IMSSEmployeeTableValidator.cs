using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class IMSSEmployeeTableValidator : IValidator<IMSSEmployeeTable>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();
        public IMiddlewareManager<IMSSEmployeeTable> MiddlewareManager { get; set; }
        public IMSSEmployeeTableValidator()
        {

        }

        public void AfterCreate(List<IMSSEmployeeTable> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<IMSSEmployeeTable> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<IMSSEmployeeTable> lstObjectsToValidate)
        {
            var validator = new RuleValidator<IMSSEmployeeTable>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<IMSSEmployeeTable> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<IMSSEmployeeTable>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
