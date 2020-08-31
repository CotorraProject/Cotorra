using Cotorra.Schema;
using System;
using System.Collections.Generic;

namespace Cotorra.Core.Validator
{
    public class AreaValidator : IValidator<Area>
    {
        private readonly List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<Area> MiddlewareManager { get; set; }
        public AreaValidator()
        {
            createRules.Add(new SimpleStringRule("Name", "Nombre", true, 1, 50, 2001));
            createRules.Add(new GuidRule("InstanceID", "Instancia", 2003));
            createRules.Add(new DuplicateItemRule<Area>(new string[] { "Name" }, "Nombre", this, 2002));
        }

        public void AfterCreate(List<Area> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<Area> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<Area> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<Area>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<Area> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<Area>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
