using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class VacationDaysOffValidator : IValidator<VacationDaysOff>
    {
        static List<IValidationRule> createRules = new List<IValidationRule>();
        public IMiddlewareManager<VacationDaysOff> MiddlewareManager { get; set; }
        static VacationDaysOffValidator()
        {            
         
        }

        public void AfterCreate(List<VacationDaysOff> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<VacationDaysOff> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<VacationDaysOff> lstObjectsToValidate)
        {
           
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<VacationDaysOff> lstObjectsToValidate)
        {
            
        }


    }
}
