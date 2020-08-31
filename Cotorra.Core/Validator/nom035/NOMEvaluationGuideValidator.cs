using Cotorra.Schema;
using Cotorra.Schema.domain.nom035;
using Cotorra.Schema.nom035;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class NOMEvaluationGuideValidator : IValidator<NOMEvaluationGuide>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<NOMEvaluationGuide> MiddlewareManager { get; set; }
        public NOMEvaluationGuideValidator()
        {
        }

        public void AfterCreate(List<NOMEvaluationGuide> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<NOMEvaluationGuide> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<NOMEvaluationGuide> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<NOMEvaluationGuide>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<NOMEvaluationGuide> lstObjectsToValidate)
        {
        }
    }
}