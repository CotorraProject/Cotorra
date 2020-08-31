using CotorraNode.Common.Config;
using System;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class SearchClient : ISearchClient
    {
        private readonly ISearchClient _client;

        public SearchClient(string authorizationHeader, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        {
            _client = ClientAdapterFactory2.GetInstance<ISearchClient>(this.GetType(), authorizationHeader, clientadapter: clientadapter);
        }

        public SearchClient(string authorizationHeader, IConfigProvider configProvider)
        { 
            _client = ClientAdapterFactory2.GetInstance<ISearchClient>(this.GetType(), authorizationHeader, configProvider);
        }

        public Task<T> QueryAsync<T>(string query)
        {
            return _client.QueryAsync<T>(query);
        }
    }
}

