using Cotorra.Schema;
using System;
using System.Collections.Generic;

namespace Cotorra.Core.Validator
{
    public class HistoricEmployeeValidator : IValidator<HistoricEmployee>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<HistoricEmployee> MiddlewareManager { get; set; }

        public HistoricEmployeeValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 2003));
        }

        public void AfterCreate(List<HistoricEmployee> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<HistoricEmployee> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<HistoricEmployee> lstObjectsToValidate)
        {
            //all good
            //var validator = new RuleValidator<HistoricEmployee>();
            //validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<HistoricEmployee> lstObjectsToValidate)
        {
            //all good
            //var validator = new RuleValidator<HistoricEmployee>();
            //validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}