using CotorraNode.Common.Base.Schema;
using CotorraNode.Common.Config;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class Client<T> : IClient<T> where T : BaseEntity
    {
        #region "Attributes"
        public IValidator<T> Validator { get; set; }
        private readonly IClient<T> _client;
        private readonly IConfigProvider _configProvider;
        #endregion

        #region "Constructor"
        public Client(string authorizationHeader, IValidator<T> validator,
            ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        {
            _client = ClientAdapterFactory.GetInstance<T>(authorizationHeader, validator, clientadapter);
            _configProvider = null;
        }

        public Client(string authorizationHeader, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        {
            _client = ClientAdapterFactory.GetInstance<T>(authorizationHeader, clientadapter);
            _configProvider = null;
        }

        public Client(string authorizationHeader, IConfigProvider configProvider, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        {
            _configProvider = configProvider;
            _client = ClientAdapterFactory.GetInstance<T>(authorizationHeader, configProvider, clientadapter);
        }
        #endregion

        public async Task CreateAsync(List<T> lstObjects, Guid identityWorkId)
        {
            await _client.CreateAsync(lstObjects, identityWorkId);
        }

        public Task CreateAsync(List<T> lstObjects, LicenseParams licenseParams)
        {
            return _client.CreateAsync(lstObjects, licenseParams);
        }
       
        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, Guid identityWorkId, string[] objectsToInclude = null)
        {
            return await _client.FindAsync(predicate, identityWorkId, objectsToInclude);
        }      

        public async Task<List<T>> GetByIdsAsync(List<Guid> lstGuids, Guid identityWorkId, string[] objectsToInclude = null)
        {
            return await _client.GetByIdsAsync(lstGuids, identityWorkId, objectsToInclude);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, Guid identityWorkId, Guid instanceId)
        {
            return await _client.CountAsync(predicate, identityWorkId, instanceId);
        }

        public async Task<int> CountAllAsync(Guid identityWorkId, Guid instanceId)
        {
            return await _client.CountAllAsync(identityWorkId, instanceId);
        }

        public async Task UpdateAsync(List<T> lstObjects, Guid identityWorkId)
        {
            await _client.UpdateAsync(lstObjects, identityWorkId);
        }

        public async Task DeleteAllAsync(Guid identityWorkId, Guid instanceId)
        {
            await _client.DeleteAllAsync(identityWorkId, instanceId);
        }

        public async Task DeleteByExpresssionAsync(Expression<Func<T, bool>> predicate, Guid identityWorkId)
        {
            await _client.DeleteByExpresssionAsync(predicate, identityWorkId);
        }

        public Task<List<T>> GetAllAsync(string[] objectsToInclude = null)
        {
            return _client.GetAllAsync(objectsToInclude);
        }

        public async Task<List<T>> GetAllAsync(Guid identityWorkId, Guid instanceId, string[] objectsToInclude = null)
        {
            return await _client.GetAllAsync(identityWorkId, instanceId, objectsToInclude);
        }

        public async Task DeleteAsync(List<Guid> lstGuids, Guid identityWorkId)
        {
            await _client.DeleteAsync(lstGuids, identityWorkId);
        }

        public Task DeleteAsync(List<Guid> lstGuids, LicenseParams licenseParams)
        {
            return _client.DeleteAsync(lstGuids, licenseParams);
        }
    }
}
