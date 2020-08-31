using Cotorra.Schema;
using Cotorra.Schema.nom035;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class NOMEvaluationPhaseValidator : IValidator<NOMEvaluationPhase>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<NOMEvaluationPhase> MiddlewareManager { get; set; }
        public NOMEvaluationPhaseValidator()
        {
            createRules.Add(new SimpleStringRule("Description", "Descripcion", true, 1, 100, 4103));
            createRules.Add(new SimpleStringRule("Name", "Nomrbe", true, 1, 100, 4104));
        }

        public void AfterCreate(List<NOMEvaluationPhase> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<NOMEvaluationPhase> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<NOMEvaluationPhase> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<NOMEvaluationPhase>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<NOMEvaluationPhase> lstObjectsToValidate)
        {
        }
    }
}