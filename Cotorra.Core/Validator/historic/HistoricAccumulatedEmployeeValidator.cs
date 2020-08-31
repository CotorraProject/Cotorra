using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class HistoricAccumulatedEmployeeValidator : IValidator<HistoricAccumulatedEmployee>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<HistoricAccumulatedEmployee> MiddlewareManager { get; set; }

        public HistoricAccumulatedEmployeeValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 2003));
        }

        public void AfterCreate(List<HistoricAccumulatedEmployee> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<HistoricAccumulatedEmployee> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<HistoricAccumulatedEmployee> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<HistoricAccumulatedEmployee>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<HistoricAccumulatedEmployee> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<HistoricAccumulatedEmployee>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}