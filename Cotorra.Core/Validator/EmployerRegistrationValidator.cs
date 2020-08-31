using Microsoft.EntityFrameworkCore.Internal;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class EmployerRegistrationValidator : IValidator<EmployerRegistration>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<EmployerRegistration> MiddlewareManager { get; set; }
        public EmployerRegistrationValidator()
        {
            createRules.Add(new SimpleStringRule("Code", "Código", true, 1, 100, 3002));
            createRules.Add(new SimpleStringRule("RiskClass", "Riesgo", true, 1, 100, 3003));
            createRules.Add(new DoubleRule("RiskClassFraction", "Fracción de riesgo", 0, 1, 3004));         
            
            createRules.Add(new GuidRule("InstanceID", "Instancia", 3020));
            createRules.Add(new DuplicateItemRule<EmployerRegistration>(new string[] { "Code" }, "Existe otro registro patronal con el mismo código", this, 3021));
        }
        
        public void AfterCreate(List<EmployerRegistration> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<EmployerRegistration> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<EmployerRegistration> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<EmployerRegistration>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            var middlewareManagerEmployee = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
           if(middlewareManagerEmployee.FindByExpression(p =>
                p.Active == true &&
                lstObjectsToValidate.Contains(p.EmployerRegistrationID.Value), Guid.Empty).Any())
            {
                throw new CotorraException(4005, "4005",
                          $"El registro patronal no se puede eliminar por que tiene colaboradores asignados.", null);
            }


        }

        public void BeforeUpdate(List<EmployerRegistration> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<EmployerRegistration>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
