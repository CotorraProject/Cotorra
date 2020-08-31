using Cotorra.Schema;
using System;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class CotorriaBotClient : ICotorriaBotClient
    {
        private readonly ICotorriaBotClient _client;

        public CotorriaBotClient(string authorizationHeader, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        {
            _client = ClientAdapterFactory2.GetInstance<ICotorriaBotClient>(this.GetType(), authorizationHeader, clientadapter: clientadapter);
        }

        public Task<string> GetIntent(GetIntentParams parameters)
        {
            return _client.GetIntent(parameters);
        }
    }
}

