using Cotorra.Schema;
using System;
using System.Collections.Generic;

namespace Cotorra.Core.Validator
{
    public class catCFDI_CodigoPostalValidator : IValidator<catCFDI_CodigoPostal>
    {
        public IMiddlewareManager<catCFDI_CodigoPostal> MiddlewareManager { get; set; }
        public catCFDI_CodigoPostalValidator()
        {
          
        }

        public void AfterCreate(List<catCFDI_CodigoPostal> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<catCFDI_CodigoPostal> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeCreate(List<catCFDI_CodigoPostal> lstObjectsToValidate)
        {
            throw new Exception("Este catálogo es de solo lectura");
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            throw new Exception("Este catálogo es de solo lectura");
        }

        public void BeforeUpdate(List<catCFDI_CodigoPostal> lstObjectsToValidate)
        {
            throw new Exception("Este catálogo es de solo lectura");
        }
    }
}
