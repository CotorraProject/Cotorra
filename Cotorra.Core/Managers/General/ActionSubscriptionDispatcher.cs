using CotorraNode.Common.Base.Schema;
using CotorraNode.Common.Library.Public;
using CotorraNode.Common.Proxy;
using CotorraNube.CommonApp.RestClient;
using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.General.Schema;
using Cotorra.Schema;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Action = Cotorra.General.Schema.Action;

namespace Cotorra.General.Core
{
    public class ActionSubscriptionDispatcher
    {

        public async Task DispatchAsync(Guid actionID, Guid idRegister)
        {
            var middlewareManager = new MiddlewareManager<Action>(new BaseRecordGeneralManager<Action>(), new ActionValidator());


            var action = (await middlewareManager
                .FindByExpressionAsync(p => p.ID == actionID && p.Active == true, Guid.Empty, new string[]
                { "ActionSubscriptions"})).FirstOrDefault();

            IDataResolver dataResolver = DataResolverFactory.GetResolver(action);

            var data = await dataResolver.ResolveDataAsync(action, idRegister);

            DisperseAsync(action.ActionSubscriptions, data);
        }

        public void DisperseAsync(List<ActionSubscription> actionSubscriptions, object data)
        {

            actionSubscriptions.ForEach(actionSubscription =>
       {
           ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, "",
                 new Uri(actionSubscription.PostBack), new object[] { data });
                 //new Uri(actionSubscription.PostBack), new object[] { data }).ContinueWith((i) =>
                 //{
                 //    if (i.Exception != null)
                 //    {
                 //        throw i.Exception;
                 //    }

                 //    return i.Result;
                 //});
       });
 

        }
    }

    public class DataResolverFactory
    {
        public static IDataResolver GetResolver(Action action)
        {
            if (action.Catalog == "Employee")
            {
                return new DataResolver<Employee>();

            }
            return new DataResolver<Employee>();

        }

    }

    public interface IDataResolver
    {
        Task<Object> ResolveDataAsync(Action action, Guid registerID);
    }

    public class DataResolver<T> : IDataResolver where T : BaseEntity
    {
        public async Task<Object> ResolveDataAsync(Action action, Guid registerID)
        {
            string res = "";
            var middlewareManager = new MiddlewareManager<T>(new BaseRecordManager<T>(), new EmptyValidator<T>());

            if (action.WebHookOperationType == WebHookOperationType.Delete)
            {
                return  registerID ;
            }

            else
            {
                var employee = (await middlewareManager.FindByExpressionAsync(p => p.ID == registerID,
                    Guid.Empty, null)).FirstOrDefault();
                if (employee != null)
                {
                    return employee;
                }
            }
            return res;
        }
    }

    public class EmptyValidator<T> : IValidator<T> where T : BaseEntity
    {


        public IMiddlewareManager<T> MiddlewareManager { get; set; }


        public void AfterCreate(List<T> lstObjectsToValidate)
        {

        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {

        }

        public void AfterUpdate(List<T> lstObjectsToValidate)
        {

        }

        public void BeforeCreate(List<T> lstObjectsToValidate)
        {
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {

        }

        public void BeforeUpdate(List<T> lstObjectsToValidate)
        {

        }
    }
}
