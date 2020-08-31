using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class InfonavitInsuranceValidator : IValidator<InfonavitInsurance>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<InfonavitInsurance> MiddlewareManager { get; set; }
        public InfonavitInsuranceValidator()
        {
        }

        public void AfterCreate(List<InfonavitInsurance> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<InfonavitInsurance> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<InfonavitInsurance> lstObjectsToValidate)
        {
            throw new System.NotSupportedException("No se permite agregar elementos a la colección");
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            throw new System.NotSupportedException("No se permite eliminar elementos a la colección");
        }

        public void BeforeUpdate(List<InfonavitInsurance> lstObjectsToValidate)
        {
            throw new System.NotSupportedException("No se permite editar elementos a la colección");
        }
    }
}
