using Cotorra.Schema;
using Cotorra.Schema.nom035;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class NOMEvaluationQuestionValidator : IValidator<NOMEvaluationQuestion>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<NOMEvaluationQuestion> MiddlewareManager { get; set; }
        public NOMEvaluationQuestionValidator()
        { 
            createRules.Add(new SimpleStringRule("Description", "Descripcion", true, 1, 100, 4103));
            createRules.Add(new SimpleStringRule("Name", "Nomrbe", true, 1, 100, 410));
        }

        public void AfterCreate(List<NOMEvaluationQuestion> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<NOMEvaluationQuestion> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<NOMEvaluationQuestion> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<NOMEvaluationQuestion>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<NOMEvaluationQuestion> lstObjectsToValidate)
        {
        }
    }
}