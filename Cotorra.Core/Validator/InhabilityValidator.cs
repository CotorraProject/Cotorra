using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;

namespace Cotorra.Core.Validator
{
    public class InhabilityValidator : IValidator<Inhability>
    {
        private readonly List<IValidationRule> createRules = new List<IValidationRule>();

        /// <summary>
        /// Constructor
        /// </summary>
        public InhabilityValidator()
        {
            createRules.Add(new SimpleStringRule("Description", "Descripción", true, 1, 250, 18001));
            createRules.Add(new SimpleStringRule("Folio", "Folio", true, 1, 9, 18002));
            createRules.Add(new IntRule("AuthorizedDays", "Días autorizados", 1, 90, 18003));
            createRules.Add(new DecimalRule("Percentage", "Porcentaje de incapacidad", 0, 100, 18004));

            //Duplicados
            createRules.Add(new DuplicateItemRule<Inhability>(new string[] { "Folio" }, "Folio", this, 18050));
        }

        public IMiddlewareManager<Inhability> MiddlewareManager { get; set; }

        /// <summary>
        /// Incidents Validation
        /// </summary>
        /// <param name="identityWorkID"></param>
        /// <param name="instanceID"></param>
        /// <param name="employeeID"></param>
        /// <param name="initialDate"></param>
        /// <returns></returns>
        private async Task ValidateIncidents(Guid identityWorkID, Guid instanceID, Guid employeeID, List<Inhability> inhabilities)
        {
            var incidentValidator = new IncidentValidator();
            var middlewareManager = new MiddlewareManager<Incident>(new BaseRecordManager<Incident>(), incidentValidator);
            incidentValidator.MiddlewareManager = middlewareManager;
            var initialDate = inhabilities.Select(x => x.InitialDate);
            var finalDate = inhabilities.Select(x => x.FinalDate);
            var orderList = initialDate.OrderBy(p => p);
            var orderListFinal = finalDate.OrderBy(p => p);


            var result = await middlewareManager.FindByExpressionAsync(p => p.InstanceID == instanceID
                && p.EmployeeID == employeeID
                && p.Date.Date >= orderList.FirstOrDefault().Date
                && p.Date.Date <= orderListFinal.LastOrDefault().Date, identityWorkID);

            inhabilities.ForEach(toSave =>
            {
                var badEntities = result.Where(x => (toSave.InitialDate <= x.Date && toSave.FinalDate >= x.Date));
                if (badEntities.Any())
                {
                    throw new CotorraException(1, "1", $"Ya capturaste incidencias para la fecha de la incapacidad {badEntities.FirstOrDefault().Date.ToShortDateString()}", null);
                }
            });             
        }


        private async Task ValidateInhabilitiesOnDate(Guid identityWorkID, Guid instanceID, Guid employeeID, List<Inhability> inhabilities)
        {
     
            var middlewareManager = new MiddlewareManager<Inhability>(new BaseRecordManager<Inhability>(), new InhabilityValidator()); 

            var result = await middlewareManager.FindByExpressionAsync(p => p.InstanceID == instanceID
                && p.EmployeeID == employeeID
               , identityWorkID);

            inhabilities.ForEach(toSave =>
            {
                var badEntities = result.Where(x => x.ID != toSave.ID && ((toSave.InitialDate >= x.InitialDate && toSave.FinalDate <= x.FinalDate) ||
                            (toSave.InitialDate >= x.InitialDate && toSave.InitialDate <= x.FinalDate) ||
                            (toSave.FinalDate <= x.FinalDate && toSave.FinalDate >= x.InitialDate)));

                if (badEntities.Any())
                {
                    throw new CotorraException(601, "601", "Ya se han registrado incapacidades en esas fechas para el empleado", null);
                }
            });

            result.ForEach(toSave =>
            {
                var badEntities = inhabilities.Where(x =>  x.ID != toSave.ID && ((toSave.InitialDate >= x.InitialDate && toSave.FinalDate <= x.FinalDate) ||
                            (toSave.InitialDate >= x.InitialDate && toSave.InitialDate <= x.FinalDate) ||
                            (toSave.FinalDate <= x.FinalDate && toSave.FinalDate >= x.InitialDate)));

                if (badEntities.Any())
                {
                    throw new CotorraException(601, "601", "Ya se han registrado incapacidades en esas fechas para el empleado", null);
                }
            }); 
        }

        /// <summary>
        /// Incident Type Validations
        /// </summary>
        /// <param name="identityWorkID"></param>
        /// <param name="instanceID"></param>
        /// <param name="employeeID"></param>
        /// <param name="incidentTypeIds"></param>
        /// <returns></returns>
        private async Task ValidateIncidentType(Guid identityWorkID, Guid instanceID, Guid employeeID, List<Guid> incidentTypeIds)
        {
            var incidentTypeValidator = new IncidentTypeValidator();
            var middlewareManager = new MiddlewareManager<IncidentType>(new BaseRecordManager<IncidentType>(), incidentTypeValidator);
            incidentTypeValidator.MiddlewareManager = middlewareManager;

            var result = await middlewareManager.FindByExpressionAsync(p => p.InstanceID == instanceID 
            && incidentTypeIds.Contains(p.ID) 
            && p.ItConsiders != ItConsiders.Inhability, identityWorkID);

            if (result.Any())
            {
                throw new CotorraException(2, "2", $"El tipo de incidente no puede ser diferente el campo Considera al valor Incapacidad, verifique {result.FirstOrDefault().ID}", null);
            }
        }

        /// <summary>
        /// Initial Date Validations
        /// </summary>
        /// <param name="identityWorkID"></param>
        /// <param name="instanceID"></param>
        /// <param name="employeeID"></param>
        /// <param name="initialDate"></param>
        /// <returns></returns>
        private async Task ValidateInitialDate(Guid identityWorkID, Guid instanceID, Guid employeeID, List<DateTime> initialDate)
        {
            var periodValidator = new PeriodDetailValidator();
            var middlewareManager = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), periodValidator);
            periodValidator.MiddlewareManager = middlewareManager;
            var orderList = initialDate.OrderBy(p => p);

            var result = await middlewareManager.FindByExpressionAsync(p => p.InstanceID == instanceID 
            && p.PeriodStatus == PeriodStatus.Calculating, identityWorkID);

            //pasado malo, futuro bueno
            if (orderList.FirstOrDefault() < result.FirstOrDefault().InitialDate)
            {
                throw new CotorraException(3, "3", $"La fecha inicial de la tarjeta no puede ser menor a la fecha de inicio de periodo", null);
            }
        }

        private void Validations(List<Inhability> lstObjectsToValidate)
        {
            if (lstObjectsToValidate.Any())
            {
                //all good
                var validator = new RuleValidator<Inhability>();
                validator.ValidateRules(lstObjectsToValidate, createRules);

                Guid identityWork = lstObjectsToValidate.FirstOrDefault().CompanyID;
                Guid instanceID = lstObjectsToValidate.FirstOrDefault().InstanceID;
                Guid employeeID = lstObjectsToValidate.FirstOrDefault().EmployeeID;
                var lstDates = lstObjectsToValidate.Select(p => p.InitialDate).ToList();
                var incidentTypesIds = lstObjectsToValidate.Select(p => p.IncidentTypeID).ToList();

                //Incidents
                ValidateIncidents(identityWork, instanceID, employeeID, lstObjectsToValidate).Wait();

                //Incident Types
                ValidateIncidentType(identityWork, instanceID, employeeID, incidentTypesIds).Wait();

                //Incidents
                ValidateInitialDate(identityWork, instanceID, employeeID, lstDates).Wait();

                ValidateInhabilitiesOnDate(identityWork, instanceID, employeeID, lstObjectsToValidate).Wait();

                //Vacations
                ValidateInDate(lstObjectsToValidate);
            }
        }

        private void ValidateInDate(List<Inhability> lstObjectsToValidate)
        {
            VacationInhabilityIncidentHelperValidator helperValidator = new VacationInhabilityIncidentHelperValidator();
            var employeesIDs = lstObjectsToValidate.Select(x => x.EmployeeID);

            var vacationManager = new MiddlewareManager<Vacation>(new BaseRecordManager<Vacation>(), new VacationValidator());
            var incidentManager = new MiddlewareManager<Incident>(new BaseRecordManager<Incident>(), new IncidentValidator());

            var orderListInitialDate = lstObjectsToValidate.Select(x => x.InitialDate).OrderBy(p => p).FirstOrDefault();
            var vacations = vacationManager.FindByExpression(x => x.InitialDate >= orderListInitialDate && employeesIDs.Contains(x.EmployeeID), Guid.Empty, new string[] { });
            var incidents = incidentManager.FindByExpression(x => x.Date >= orderListInitialDate && employeesIDs.Contains(x.EmployeeID), Guid.Empty, new string[] { });

            helperValidator.ValidateInDate(vacations, lstObjectsToValidate, incidents);
        }




        public void AfterCreate(List<Inhability> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<Inhability> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<Inhability> lstObjectsToValidate)
        {
            Validations (lstObjectsToValidate);

        }
        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<Inhability> lstObjectsToValidate)
        {
            Validations(lstObjectsToValidate);
        }
    }
}
