using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class IncidentTypeValidator : IValidator<IncidentType>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<IncidentType> MiddlewareManager { get; set; }
        public IncidentTypeValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 8001));
            createRules.Add(new SimpleStringRule("Code", "Código", true, 1, 4, 8002));
            createRules.Add(new SimpleStringRule("Name", "Nombre", true, 1, 100, 8003));
        }

        public void AfterCreate(List<IncidentType> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<IncidentType> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<IncidentType> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<IncidentType>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<IncidentType> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<IncidentType>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
