using Cotorra.Schema;
using Cotorra.Schema.nom035;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator.nom035
{
    public class NOMEvaluationDomainValidator : IValidator<NOMEvaluationDomain>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<NOMEvaluationDomain> MiddlewareManager { get; set; }
        public NOMEvaluationDomainValidator()
        {
            createRules.Add(new SimpleStringRule("Description", "Descripcion", true, 1, 100, 4103));
            createRules.Add(new SimpleStringRule("Name", "Nomrbe", true, 1, 100, 4104));
        }

        public void AfterCreate(List<NOMEvaluationDomain> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<NOMEvaluationDomain> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<NOMEvaluationDomain> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<NOMEvaluationDomain>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<NOMEvaluationDomain> lstObjectsToValidate)
        {
        }
    }
}