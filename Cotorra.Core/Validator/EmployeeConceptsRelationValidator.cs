using DinkToPdf;
using Microsoft.EntityFrameworkCore.Internal;
using MoreLinq;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class EmployeeConceptsRelationValidator : IValidator<EmployeeConceptsRelation>
    {
        private readonly List<IValidationRule> createRules;

        public IMiddlewareManager<EmployeeConceptsRelation> MiddlewareManager { get; set; }
        public EmployeeConceptsRelationValidator()
        {
            createRules = new List<IValidationRule>();
            //Relaciones
            createRules.Add(new GuidRule("InstanceID", "Instancia", 15001));
            createRules.Add(new GuidRule("EmployeeID", "Empleado", 15002));
            createRules.Add(new GuidRule("ConceptPaymentID", "Concepto", 15002));

            //Monto del Crédito
            createRules.Add(new DecimalRule("CreditAmount", "El importe del crédito debe de ser mayor a 0.000001 y menor de a 999999999",
                0.000001m, 999999999m, 15005));

            //Incremento/Descuento mensual 
            createRules.Add(new DecimalRule("OverdraftDetailValue", "Valor Incremento/Descuento", 0.00000m, 9999999m, 15009));

            //Incremento/Descuento mensual
            createRules.Add(new DecimalRule("OverdraftDetailAmount", "Monto Incremento/Descuento", 0.00000m, 9999999m, 15009));

            //Pagos hechos por fuera
            createRules.Add(new DecimalRule("PaymentsMadeByOtherMethod",
                "El pago hecho por fuera debe ser mayor o igual a 0 y menor de a 9,999,999", -0.99999999m, 9999999m, 15010));
           
        }

        public void AfterCreate(List<EmployeeConceptsRelation> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<EmployeeConceptsRelation> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<EmployeeConceptsRelation> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<EmployeeConceptsRelation>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
            //validar si ya tiene pagos aplicados
            var middlewareManager = new MiddlewareManager<EmployeeConceptsRelationDetail>(new BaseRecordManager<EmployeeConceptsRelationDetail>(),
                new EmployeeConceptsRelationDetailValidator());
            var details = middlewareManager.FindByExpression(p => lstObjectsToValidate.Contains(p.EmployeeConceptsRelationID), Guid.Empty);

            if (details.Any(p=> p.ConceptsRelationPaymentStatus == ConceptsRelationPaymentStatus.Applied))
            {
                throw new Exception("No se puede eliminar el crédito, debido a que ya tiene pagos aplicados al mismo. Si lo deseas puedes cambiar el estado del crédito a Inactivo, para que no se aplique.");
            }

            if (details.Any())
            {
                middlewareManager.Delete(details.Select(p => p.ID).ToList(), Guid.Empty);
            }
        }

        public void BeforeUpdate(List<EmployeeConceptsRelation> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<EmployeeConceptsRelation>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

       

        public void AfterUpdateCreateOrigin(IEnumerable<EmployeeConceptsRelation> lstObjectsToValidate, bool validateTotalAmount = true)
        {
            if (lstObjectsToValidate.Any())
            {
                lstObjectsToValidate.ForEach(p =>
                {
                    var balance = p.CreditAmount - p.AccumulatedAmountWithHeldCalculated - p.PaymentsMadeByOtherMethod;

                    if (p.CreditAmount <= 0 && validateTotalAmount)
                    {
                        throw new CotorraException(103, "103", "El monto del crédito debe de ser mayor a 0.", null);
                    }
                    else if (p.PaymentsMadeByOtherMethod < 0 || p.PaymentsMadeByOtherMethod > p.CreditAmount)
                    {
                        throw new CotorraException(103, "103", "Los pagos hechos por fuera no pueden ser menores a 0 ni tampoco puede ser mayor al monto del crédito", null);
                    }
                    else if (p.OverdraftDetailAmount > balance)
                    {
                        throw new CotorraException(103, "103", "El monto de descuento no puede ser mayor al saldo pendiente.", null);
                    }
                    else if (balance < 0)
                    {
                        throw new CotorraException(103, "103", "El saldo pendiente no puede ser menor a 0.", null);
                    }
                });
            }
        }
    }
}
