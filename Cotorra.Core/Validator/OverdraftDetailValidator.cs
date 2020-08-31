using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class OverdraftDetailValidator : IValidator<OverdraftDetail>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();
        public IMiddlewareManager<OverdraftDetail> MiddlewareManager { get; set; }
        public OverdraftDetailValidator()
        {
            createRules.Add(new DuplicateItemRule<OverdraftDetail>(new string[] { "OverdraftID","ConceptPaymentID" }, "No es posible duplicar conceptos en el sobrerecibo", this, 205));
        }

        public void AfterCreate(List<OverdraftDetail> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<OverdraftDetail> lstObjectsToValidate)
        {

        }

        public void BeforeCreate(List<OverdraftDetail> lstObjectsToValidate)
        {
            
            //all good
            var validator = new RuleValidator<OverdraftDetail>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<OverdraftDetail> lstObjectsToValidate)
        {
            //all good
            //var validator = new RuleValidator<OverdraftDetail>();
            //validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
