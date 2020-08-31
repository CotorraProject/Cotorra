using CotorraNode.Common.Base.Schema;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Schema
{
    public interface IValidator<T> where T : BaseEntity
    {
        IMiddlewareManager<T> MiddlewareManager { get; set; }

        void BeforeCreate(List<T> lstObjectsToValidate);

        void AfterCreate(List<T> lstObjectsToValidate);    
        

        public async Task AfterCreate (List<T> lstObjectsToValidate, IParams parameters)
        {
            this.AfterCreate(lstObjectsToValidate);
        }


        void BeforeUpdate(List<T> lstObjectsToValidate);

        void AfterUpdate(List<T> lstObjectsToValidate);

        void BeforeDelete(List<Guid> lstObjectsToValidate);

        void AfterDelete(List<Guid> lstObjectsToValidate);

        public async Task  AfterDeleteAsync(List<Guid> lstObjectsToValidate, IParams parameters)
        {
            this.AfterDelete(lstObjectsToValidate);
        }

        public object BeforeDelete(List<Guid> lstObjectsToValidate, Guid identityWorkID)
        {
            this.BeforeDelete(lstObjectsToValidate);
            return null;
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate, object parameters)
        {
            this.AfterDelete(lstObjectsToValidate);
        }
    }
}
