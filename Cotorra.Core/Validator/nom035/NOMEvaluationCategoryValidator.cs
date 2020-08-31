using Cotorra.Schema;
using Cotorra.Schema.nom035;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator.nom035
{
    public class NOMEvaluationCategoryValidator : IValidator<NOMEvaluationCategory>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<NOMEvaluationCategory> MiddlewareManager { get; set; }
        public NOMEvaluationCategoryValidator()
        {
            createRules.Add(new SimpleStringRule("Description", "Descripcion", true, 1, 100, 4103));
            createRules.Add(new SimpleStringRule("Name", "Nomrbe", true, 1, 100, 4104));
        }

        public void AfterCreate(List<NOMEvaluationCategory> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<NOMEvaluationCategory> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<NOMEvaluationCategory> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<NOMEvaluationCategory>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<NOMEvaluationCategory> lstObjectsToValidate)
        {
        }
    }
}