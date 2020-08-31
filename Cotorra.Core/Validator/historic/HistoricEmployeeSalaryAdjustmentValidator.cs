using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class HistoricEmployeeSalaryAdjustmentValidator : IValidator<HistoricEmployeeSalaryAdjustment>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<HistoricEmployeeSalaryAdjustment> MiddlewareManager { get; set; }
        public HistoricEmployeeSalaryAdjustmentValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 4002));
            createRules.Add(new GuidRule("EmployeeID", "ID Empleado", 4002));
        }

        public void AfterCreate(List<HistoricEmployeeSalaryAdjustment> lstObjectsToValidate)
        {
            
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<HistoricEmployeeSalaryAdjustment> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<HistoricEmployeeSalaryAdjustment> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<HistoricEmployeeSalaryAdjustment>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
           
        }

        public void BeforeUpdate(List<HistoricEmployeeSalaryAdjustment> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<HistoricEmployeeSalaryAdjustment>();
            validator.ValidateRules(lstObjectsToValidate, createRules);


        }
    }
}
