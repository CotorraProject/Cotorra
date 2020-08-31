using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class DepartmentValidator : IValidator<Department>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();
        public IMiddlewareManager<Department> MiddlewareManager { get; set; }
        public DepartmentValidator()
        {
            createRules.Add(new SimpleStringRule("Name", "Nombre", true, 1, 50, 2001));
            createRules.Add(new GuidRule("InstanceID", "Instancia", 2003));
            createRules.Add(new DuplicateItemRule<Department>(new string[] { "Name" }, "Nombre", this, 2002));
        }

        public void AfterCreate(List<Department> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<Department> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<Department> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<Department>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<Department> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<Department>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
