using Cotorra.Schema;
using Cotorra.Schema.domain.nom035;
using Cotorra.Schema.nom035;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class NOMSurveyReplyValidator : IValidator<NOMSurveyReply>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<NOMSurveyReply> MiddlewareManager { get; set; }
        public NOMSurveyReplyValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 4002));
        }

        public void AfterCreate(List<NOMSurveyReply> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<NOMSurveyReply> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<NOMSurveyReply> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<NOMSurveyReply>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<NOMSurveyReply> lstObjectsToValidate)
        {
        }
    }
}