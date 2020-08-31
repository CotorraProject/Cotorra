using CotorraNode.Common.Config;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class StatusClient<T> : IStatusClient<T> where T : StatusIdentityCatalogEntityExt
    {
        private readonly IStatusClient<T> _client;
        private readonly IConfigProvider _configProvider;

        public StatusClient(string authorizationHeader, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        {
            _configProvider = null;
            _client = StatusClientAdapterFactory.GetInstance<T>(authorizationHeader, clientadapter);           
        }

        public StatusClient(string authorizationHeader, IConfigProvider configProvider, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        {
            _configProvider = configProvider;
            _client = StatusClientAdapterFactory.GetInstance<T>(authorizationHeader, configProvider, clientadapter);
        }
       
        public async Task SetActive(IEnumerable<Guid> idsToUpdate, Guid identityWorkId)
        {
             await _client.SetActive(idsToUpdate, identityWorkId);
        }

        public async Task SetUnregistered(IEnumerable<Guid> idsToUpdate, Guid identityWorkId, params object[] parameters)
        {
             await _client.SetUnregistered(idsToUpdate, identityWorkId, parameters);
        }

        public async Task SetInactive(IEnumerable<Guid> idsToUpdate, Guid identityWorkId)
        {
             await _client.SetInactive(idsToUpdate, identityWorkId);
        }

        public async Task SetStatus(IEnumerable<Guid> idsToUpdate, Guid identityWorkId, CotorriaStatus status)
        {
             await _client.SetStatus(idsToUpdate, identityWorkId, status);
        }

        public async Task UpdateAsync(IEnumerable<Guid> idsToUpdate, CotorriaStatus status, Guid identityWorkID, params object[] parameters)
        {
             await _client.UpdateAsync(idsToUpdate, status, identityWorkID, parameters);
        }
    }
}

