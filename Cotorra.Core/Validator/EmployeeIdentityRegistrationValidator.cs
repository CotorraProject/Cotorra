using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class EmployeeIdentityRegistrationValidator : IValidator<EmployeeIdentityRegistration>
    {
        private readonly List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<EmployeeIdentityRegistration> MiddlewareManager { get; set; }
        public EmployeeIdentityRegistrationValidator()
        {
        }

        public void AfterCreate(List<EmployeeIdentityRegistration> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<EmployeeIdentityRegistration> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<EmployeeIdentityRegistration> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<EmployeeIdentityRegistration>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<EmployeeIdentityRegistration> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<EmployeeIdentityRegistration>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}