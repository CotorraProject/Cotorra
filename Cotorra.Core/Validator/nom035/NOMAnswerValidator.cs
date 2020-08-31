using Cotorra.Schema;
using Cotorra.Schema.domain.nom035;
using Cotorra.Schema.nom035;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class NOMAnswerValidator : IValidator<NOMAnswer>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<NOMAnswer> MiddlewareManager { get; set; }
        public NOMAnswerValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 4002));
        }

        public void AfterCreate(List<NOMAnswer> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<NOMAnswer> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<NOMAnswer> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<NOMAnswer>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<NOMAnswer> lstObjectsToValidate)
        {
        }
    }
}