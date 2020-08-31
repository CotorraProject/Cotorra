using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class CancelationFiscalDocumentDetailValidator : IValidator<CancelationFiscalDocumentDetail>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();
        public IMiddlewareManager<CancelationFiscalDocumentDetail> MiddlewareManager { get; set; }
        public CancelationFiscalDocumentDetailValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 2003));
        }

        public void AfterCreate(List<CancelationFiscalDocumentDetail> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<CancelationFiscalDocumentDetail> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<CancelationFiscalDocumentDetail> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<CancelationFiscalDocumentDetail>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<CancelationFiscalDocumentDetail> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<CancelationFiscalDocumentDetail>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
