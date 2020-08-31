using Cotorra.Core.Managers.Calculation;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class IncidentValidator : IValidator<Incident>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<Incident> MiddlewareManager { get; set; }
        public IncidentValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 18001));
            createRules.Add(new GuidRule("PeriodDetailID", "Detalle Periodo", 18002));
            createRules.Add(new GuidRule("EmployeeID", "Empleado", 18003));
            createRules.Add(new GuidRule("IncidentTypeID", "Incidencia", 18004));
        }

        public void AfterCreate(List<Incident> lstObjectsToValidate)
        {
            //Calculation async fire and forget
            IEnumerable<Guid> employeeIds = lstObjectsToValidate.Select(p => p.EmployeeID);
            var company = lstObjectsToValidate.FirstOrDefault().company;
            var instanceID = lstObjectsToValidate.FirstOrDefault().InstanceID;
            var userId = lstObjectsToValidate.FirstOrDefault().IdentityID;
            new OverdraftCalculationManager().CalculationFireAndForgetByEmployeesAsync(employeeIds, company, instanceID, userId);
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<Incident> lstObjectsToValidate)
        {
            //Calculation async fire and forget
            IEnumerable<Guid> employeeIds = lstObjectsToValidate.Select(p => p.EmployeeID);
            var company = lstObjectsToValidate.FirstOrDefault().company;
            var instanceID = lstObjectsToValidate.FirstOrDefault().InstanceID;
            var userId = lstObjectsToValidate.FirstOrDefault().IdentityID;
            new OverdraftCalculationManager().CalculationFireAndForgetByEmployeesAsync(employeeIds, company, instanceID, userId);
        }

        private void BeforeCreateUpdate(List<Incident> lstObjectsToValidate)
        {
            var mgr = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
            var employees = mgr.FindByExpression(x => lstObjectsToValidate.Select(p => p.EmployeeID).Contains(x.ID), lstObjectsToValidate.FirstOrDefault().company);

            employees.ForEach(employee =>
            {
                var incidentsEmployee = lstObjectsToValidate.Where(x => x.EmployeeID == employee.ID);
                if (incidentsEmployee.Any(x => x.Date < employee.EntryDate))
                {
                    throw new CotorraException(18005, "18005", "No es posible registrar incidencias antes de la fecha de ingreso del colaborador.", null);
                }
            });

            ValidateInDate(lstObjectsToValidate);
        }

        public void BeforeCreate(List<Incident> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<Incident>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
            BeforeCreateUpdate(lstObjectsToValidate);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<Incident> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<Incident>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
            BeforeCreateUpdate(lstObjectsToValidate);

        }

        private void ValidateInDate(List<Incident> lstObjectsToValidate)
        {
            VacationInhabilityIncidentHelperValidator helperValidator = new VacationInhabilityIncidentHelperValidator();
            var employeesIDs = lstObjectsToValidate.Select(x => x.EmployeeID);

            var inhabilityManager = new MiddlewareManager<Inhability>(new BaseRecordManager<Inhability>(), new InhabilityValidator());
            var vacationManager = new MiddlewareManager<Vacation>(new BaseRecordManager<Vacation>(), new VacationValidator());

            var orderListInitialDate = lstObjectsToValidate.Select(x => x.Date).OrderBy(p => p).FirstOrDefault();
            var inhabilities = inhabilityManager.FindByExpression(x => (x.InitialDate >= orderListInitialDate || x.InitialDate.AddDays(x.AuthorizedDays - 1) >= orderListInitialDate) && employeesIDs.Contains(x.EmployeeID), Guid.Empty, new string[] { });
            var vacations = vacationManager.FindByExpression(x => x.InitialDate >= orderListInitialDate && employeesIDs.Contains(x.EmployeeID), Guid.Empty, new string[] { });

            helperValidator.ValidateInDate(vacations, inhabilities, lstObjectsToValidate);
        }

    }
}
