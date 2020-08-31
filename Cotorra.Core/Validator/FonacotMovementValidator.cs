using MoreLinq;
using Cotorra.Core.Managers.Calculation;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cotorra.Core.Validator
{
    public class FonacotMovementValidator : IValidator<FonacotMovement>
    {
        private readonly List<IValidationRule> createRules;

        public IMiddlewareManager<FonacotMovement> MiddlewareManager { get; set; }
        public FonacotMovementValidator()
        {
            createRules = new List<IValidationRule>();
            //Relaciones
            createRules.Add(new GuidRule("InstanceID", "Instancia", 15001));
            createRules.Add(new GuidRule("EmployeeID", "Empleado", 15002));

            //Número de crédito            
            createRules.Add(new SimpleStringRule("CreditNumber", "No. de crédito", true, 1, 20, 15004));
            createRules.Add(new DuplicateItemRule<FonacotMovement>(new string[] { "CreditNumber", "EmployeeID" },
                "Ya existe otra trajeta FONACOT activa con este número de crédito", this, 15003));

            //Descripción
            createRules.Add(new SimpleStringRule("Description", "Descripción", false, 0, 50, 15006));

            //Mes y Ejercicio
            createRules.Add(new IntRule("Month", "El mes es inválido", 1, 12, 15007));
            createRules.Add(new IntRule("Year", "El año es inválido", 1900, DateTime.Now.AddYears(60).Year, 15008));
        }

        public void CreateConcurrentConcepts(List<FonacotMovement> lstObjectsToValidate)
        {

            //verify if the concurrent concepts exists or modify
            var instanceID = lstObjectsToValidate.FirstOrDefault().InstanceID;
            var companyID = lstObjectsToValidate.FirstOrDefault().company;
            var user = lstObjectsToValidate.FirstOrDefault().user;
            new EmployeeConceptsRelationManager().CreateConcurrentConceptsAsync(instanceID, companyID, user).Wait();

            //recalculate employees
            var employeeIds = lstObjectsToValidate.Select(p => p.EmployeeID).ToList();
            new OverdraftCalculationManager().CalculationByEmployeesAsync(employeeIds, companyID,
                instanceID, user).Wait();
        }

        public void ValidateCreditRules(List<FonacotMovement> lstObjectsToValidate)
        {
            var employeeConceptsRelationValidator = new EmployeeConceptsRelationValidator();
            employeeConceptsRelationValidator.AfterUpdateCreateOrigin(lstObjectsToValidate.Select(p => p.EmployeeConceptsRelation));             
        }

        public void ValidateNotChangeData(List<FonacotMovement> lstObjectsToValidate)
        {
            //all good
            //validar si ya tiene pagos aplicados
            var fonacotIDs = lstObjectsToValidate.Select(p => p.ID);
            var middlewareManagerFonacot = new MiddlewareManager<FonacotMovement>(new BaseRecordManager<FonacotMovement>(),
               new FonacotMovementValidator());
            var fonacotDataExistent = middlewareManagerFonacot.FindByExpression(p => fonacotIDs.Contains(p.ID), Guid.Empty);

            var boolChanged = false;
            foreach (var exists in fonacotDataExistent)
            {
                var found = lstObjectsToValidate.FirstOrDefault(p => p.ID == exists.ID);
                if (exists.Month != found.Month || exists.Year != found.Year)
                {
                    boolChanged = true;
                    break;
                }
            }
            
            var employeeConceptsId = lstObjectsToValidate.Select(p => p.EmployeeConceptsRelationID);
            var middlewareManager = new MiddlewareManager<EmployeeConceptsRelationDetail>(new BaseRecordManager<EmployeeConceptsRelationDetail>(),
                new EmployeeConceptsRelationDetailValidator());
            var details = middlewareManager.FindByExpression(p =>
                employeeConceptsId.Contains(p.EmployeeConceptsRelationID), Guid.Empty);

            if (details.Any(p => p.ConceptsRelationPaymentStatus == ConceptsRelationPaymentStatus.Applied) && boolChanged)
            {
                throw new Exception("No se puede actualizar los datos proporcionados del crédito, debido a que ya tiene pagos aplicados al mismo. Si lo deseas puedes cambiar el estado del crédito a Inactivo, para que no se aplique.");
            }
        }

        public void AfterCreate(List<FonacotMovement> lstObjectsToValidate)
        {
            ValidateCreditRules(lstObjectsToValidate);
            CreateConcurrentConcepts(lstObjectsToValidate);
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<FonacotMovement> lstObjectsToValidate)
        {
            ValidateCreditRules(lstObjectsToValidate);
            CreateConcurrentConcepts(lstObjectsToValidate);            
        }

        public void BeforeCreate(List<FonacotMovement> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<FonacotMovement>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<FonacotMovement> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<FonacotMovement>();
            validator.ValidateRules(lstObjectsToValidate, createRules);

            ValidateNotChangeData(lstObjectsToValidate);
        }
    }
}
