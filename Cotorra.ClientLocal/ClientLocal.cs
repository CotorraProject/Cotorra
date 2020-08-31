using CotorraNode.Common.Base.Schema;
using Cotorra.Client;
using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cotorra.ClientLocal
{
    public class ClientLocal<T> : IClient<T> where T : BaseEntity
    {
        private readonly string _authorizationHeader;
        public IValidator<T> Validator { get; set; }

        public ClientLocal(string authorizationHeader)
        {
            Validator = new ValidatorFactory().CreateInstance<T>();
            _authorizationHeader = authorizationHeader;
        }

        public ClientLocal(string authorizationHeader, IValidator<T> validator)
        {
            Validator = validator;
            _authorizationHeader = authorizationHeader;
        }

        public async Task CreateAsync(List<T> lstObjects, Guid identityWorkId)
        {
            var middlewareManager = new MiddlewareManager<T>(new BaseRecordManager<T>(), Validator);
            await middlewareManager.CreateAsync(lstObjects, identityWorkId);
        }

        public async Task CreateAsync(List<T> lstObjects, LicenseParams licenseParams)
        {
            var middlewareManager = new MiddlewareManager<T>(new BaseRecordManager<T>(), Validator);
            await middlewareManager.CreateAsync(lstObjects, licenseParams);
        }

        public async Task DeleteAsync(List<Guid> lstGuids, Guid identityWorkId)
        {
            var middlewareManager = new MiddlewareManager<T>(new BaseRecordManager<T>(), Validator);
            await middlewareManager.DeleteAsync(lstGuids, identityWorkId);
        }
         
        public async Task DeleteAsync(List<Guid> lstGuids, LicenseParams licenseParams)
        {
            var middlewareManager = new MiddlewareManager<T>(new BaseRecordManager<T>(), Validator);
            await middlewareManager.DeleteAsync(lstGuids, licenseParams);
        }

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, Guid identityWorkId, string[] objectsToInclude = null)
        {
            var middlewareManager = new MiddlewareManager<T>(new BaseRecordManager<T>(), Validator);
            var lstReturn = await middlewareManager.FindByExpressionAsync(predicate, identityWorkId, objectsToInclude);

            return lstReturn;
        }     

        public async Task<List<T>> GetByIdsAsync(List<Guid> lstGuids, Guid identityWorkId, string[] objectsToInclude = null)
        {
            var middlewareManager = new MiddlewareManager<T>(new BaseRecordManager<T>(), Validator);
            var lstReturn = await middlewareManager.GetByIdsAsync(lstGuids, identityWorkId, objectsToInclude);

            return lstReturn;
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, Guid identityWorkId, Guid instanceId)
        {
            var middlewareManager = new MiddlewareManager<T>(new BaseRecordManager<T>(), Validator);
            int lstReturn = await middlewareManager.CountAsync(predicate, identityWorkId);

            return lstReturn;
        }

        public async Task<int> CountAllAsync(Guid identityWorkId, Guid instanceId)
        {
            var middlewareManager = new MiddlewareManager<T>(new BaseRecordManager<T>(), Validator);
            int lstReturn = await middlewareManager.CountAllAsync(identityWorkId, instanceId);

            return lstReturn;
        }

        public async Task UpdateAsync(List<T> lstObjects, Guid identityWorkId)
        {
            var middlewareManager = new MiddlewareManager<T>(new BaseRecordManager<T>(), Validator);
            await middlewareManager.UpdateAsync(lstObjects, identityWorkId);
        }

        public async Task DeleteAllAsync(Guid identityWorkId, Guid instanceId)
        {
            var middlewareManager = new MiddlewareManager<T>(new BaseRecordManager<T>(), Validator);
            await middlewareManager.DeleteAllAsync(identityWorkId, instanceId);
        }

        public async Task DeleteByExpresssionAsync(Expression<Func<T, bool>> predicate, Guid identityWorkId)
        {
            var middlewareManager = new MiddlewareManager<T>(new BaseRecordManager<T>(), Validator);
            await middlewareManager.DeleteByExpresssionAsync(predicate, identityWorkId, null);
        }

        public async Task<List<T>> GetAllAsync(string[] objectsToInclude = null)
        { 
            var middlewareManager = new MiddlewareManager<T>(new BaseRecordManager<T>(), Validator);
            return await middlewareManager.GetAllAsync( objectsToInclude);             
        }

        public async Task<List<T>> GetAllAsync(Guid identityWorkId, Guid instanceId, string[] objectsToInclude = null)
        {
            var middlewareManager = new MiddlewareManager<T>(new BaseRecordManager<T>(), Validator);
            var lstReturn = await middlewareManager.GetAllAsync(identityWorkId, instanceId, objectsToInclude);

            return lstReturn;
        }

    }
}
