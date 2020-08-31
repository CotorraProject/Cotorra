using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class BenefitTypeValidator : IValidator<BenefitType>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<BenefitType> MiddlewareManager { get; set; }
        public BenefitTypeValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 4002));
            createRules.Add(new IntRule("Antiquity", "Antiguedad", 0, 60, 4003));
            createRules.Add(new DecimalRule("Holidays", "Días vacaciones", 0, 999, 4004));
            createRules.Add(new DecimalRule("HolidayPremiumPortion", "Porcion prima vacacional", 0, 999, 4006));
            createRules.Add(new DecimalRule("DaysOfChristmasBonus", "Días de aguinaldo", 0, 999, 4007));
            createRules.Add(new DecimalRule("IntegrationFactor", "Factor de integración", 0, 999, 4008));
            createRules.Add(new DecimalRule("DaysOfAntiquity", "Días Antigüedad", 0, 999, 4009));
        }

        public void AfterCreate(List<BenefitType> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<BenefitType> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<BenefitType> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<BenefitType>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<BenefitType> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<BenefitType>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
