using Cotorra.Schema;
using Cotorra.Schema.nom035;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class NOMEvaluationPeriodValidator : IValidator<NOMEvaluationPeriod>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<NOMEvaluationPeriod> MiddlewareManager { get; set; }
        public NOMEvaluationPeriodValidator()
        {
            createRules.Add(new SimpleStringRule("Period", "Periodo", true, 1, 100, 4003));
        }

        public void AfterCreate(List<NOMEvaluationPeriod> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<NOMEvaluationPeriod> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<NOMEvaluationPeriod> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<NOMEvaluationPeriod>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<NOMEvaluationPeriod> lstObjectsToValidate)
        {
        }
    }
}