using Cotorra.Schema;
using Cotorra.Schema.domain.nom035;
using Cotorra.Schema.nom035;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class NOMEvaluationSurveyValidator : IValidator<NOMEvaluationSurvey>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<NOMEvaluationSurvey> MiddlewareManager { get; set; }
        public NOMEvaluationSurveyValidator()
        {
        }

        public void AfterCreate(List<NOMEvaluationSurvey> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<NOMEvaluationSurvey> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<NOMEvaluationSurvey> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<NOMEvaluationSurvey>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<NOMEvaluationSurvey> lstObjectsToValidate)
        {
        }
    }
}