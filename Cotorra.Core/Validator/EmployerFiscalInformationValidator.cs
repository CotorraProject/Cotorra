using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class EmployerFiscalInformationValidator : IValidator<EmployerFiscalInformation>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<EmployerFiscalInformation> MiddlewareManager { get; set; }
        public EmployerFiscalInformationValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 3020));
            createRules.Add(new SimpleStringRule("RFC", "RFC", true, 12, 13, 3021));
            createRules.Add(new DuplicateItemRule<EmployerFiscalInformation>(new string[] { "CertificateNumber" }, "Él certificado que intentas agregar ya existe.", this, 3021));
        }

        public void AfterCreate(List<EmployerFiscalInformation> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<EmployerFiscalInformation> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<EmployerFiscalInformation> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<EmployerFiscalInformation>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<EmployerFiscalInformation> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<EmployerFiscalInformation>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
