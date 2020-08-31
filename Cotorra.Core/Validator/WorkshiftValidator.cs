using CotorraNode.Common.Base.Schema;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class WorkshiftValidator : IValidator<Workshift>
    {
        static List<IValidationRule> createRules = new List<IValidationRule>();
        public IMiddlewareManager<Workshift> MiddlewareManager { get; set; }
        static WorkshiftValidator()
        {
            createRules.Add(new SimpleStringRule("Name", "Nombre", true, 1, 25, 8001)); 
            createRules.Add(new DoubleRule("Hours", "Horas", 0, 99, 8002));
        }

        public void AfterCreate(List<Workshift> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<Workshift> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<Workshift> lstObjectsToValidate)
        {
            var validator = new RuleValidator<Workshift>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<Workshift> lstObjectsToValidate)
        {
            var validator = new RuleValidator<Workshift>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }


    }
}
