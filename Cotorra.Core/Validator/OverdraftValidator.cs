using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class OverdraftValidator : IValidator<Overdraft>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();
        public IMiddlewareManager<Overdraft> MiddlewareManager { get; set; }
        public OverdraftValidator()
        {
            //createRules.Add(new SimpleStringRule("Name", "Nombre", true, 1, 100, 110001));
        }

        public void AfterCreate(List<Overdraft> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<Overdraft> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<Overdraft> lstObjectsToValidate)
        {
            var validator = new RuleValidator<Overdraft>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            var middlewareManagerDetails = new MiddlewareManager<OverdraftDetail>(new BaseRecordManager<OverdraftDetail>(),
                new OverdraftDetailValidator());
            var overdraftDetails = middlewareManagerDetails.FindByExpression(p=> lstObjectsToValidate.Contains(p.OverdraftID) ,Guid.Empty);
            if (overdraftDetails.Any())
            {
                middlewareManagerDetails.Delete(overdraftDetails.Select(p => p.ID).ToList(), Guid.Empty);
            }
        }

        public void BeforeUpdate(List<Overdraft> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<Overdraft>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
