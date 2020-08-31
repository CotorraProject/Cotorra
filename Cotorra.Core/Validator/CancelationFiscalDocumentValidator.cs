using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class CancelationFiscalDocumentValidator : IValidator<CancelationFiscalDocument>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();
        public IMiddlewareManager<CancelationFiscalDocument> MiddlewareManager { get; set; }
        public CancelationFiscalDocumentValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 2003));
        }

        public void AfterCreate(List<CancelationFiscalDocument> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<CancelationFiscalDocument> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<CancelationFiscalDocument> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<CancelationFiscalDocument>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<CancelationFiscalDocument> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<CancelationFiscalDocument>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
