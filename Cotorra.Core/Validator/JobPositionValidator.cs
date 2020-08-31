using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class JobPositionValidator : IValidator<JobPosition>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<JobPosition> MiddlewareManager { get; set; }
        static JobPositionValidator()
        {

        }

        public JobPositionValidator()
        {
            createRules.Add(new SimpleStringRule("Name", "Nombre", true, 1, 50, 1001));
            createRules.Add(new GuidRule("InstanceID", "Instancia", 1003));
            createRules.Add(new DuplicateItemRule<JobPosition>(new string[] { "Name" }, "Nombre", this, 1002));
        }

        public void AfterCreate(List<JobPosition> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<JobPosition> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<JobPosition> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<JobPosition>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<JobPosition> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<JobPosition>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
