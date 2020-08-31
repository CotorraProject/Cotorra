using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class IMSSFareValidator : IValidator<IMSSFare>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<IMSSFare> MiddlewareManager { get; set; }
        public IMSSFareValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 4002));
            createRules.Add(new DecimalRule("EmployerShare", "Patrón", 0, 1, 4005));
            createRules.Add(new DecimalRule("EmployeeShare", "Obrero", 0, 1, 4006));
            createRules.Add(new IntRule("MaxSMDF", "Tope SMDF", 0, 365, 4007));
            createRules.Add(new SimpleStringRule("IMMSBranch", "Rama", true, 1, 50, 4008));            
        }

        public void AfterCreate(List<IMSSFare> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<IMSSFare> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<IMSSFare> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<IMSSFare>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<IMSSFare> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<IMSSFare>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
