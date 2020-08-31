using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class EmployeeConceptsRelationDetailValidator : IValidator<EmployeeConceptsRelationDetail>
    {
        private readonly List<IValidationRule> createRules;

        public IMiddlewareManager<EmployeeConceptsRelationDetail> MiddlewareManager { get; set; }

        public EmployeeConceptsRelationDetailValidator()
        {
            createRules = new List<IValidationRule>();
            //Relaciones
            createRules.Add(new GuidRule("InstanceID", "Instancia", 15001));
            createRules.Add(new GuidRule("EmployeeConceptsRelationID", "Concepto", 15002));
            createRules.Add(new GuidRule("OverdraftID", "Recibo de nómina", 15002));

            //Valor aplicado en el recibo de nómina 
            createRules.Add(new DecimalRule("ValueApplied", "Valor aplicado en el recibo de nómina", 0, 9999999m, 15009));

            //Monto aplicado en el recibo de nómina
            createRules.Add(new DecimalRule("AmountApplied", "Monto aplicado en el recibo de nómina", 0, 9999999m, 15009));

            //No se puede duplicar el detalle del crédito FONACOT
            createRules.Add(new DuplicateItemRule<EmployeeConceptsRelationDetail>(new string[] { "EmployeeConceptsRelationID", "OverdraftID" }, "Pago de crédito", this, 15010));
        }

        public void AfterCreate(List<EmployeeConceptsRelationDetail> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<EmployeeConceptsRelationDetail> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<EmployeeConceptsRelationDetail> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<EmployeeConceptsRelationDetail>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<EmployeeConceptsRelationDetail> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<EmployeeConceptsRelationDetail>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
