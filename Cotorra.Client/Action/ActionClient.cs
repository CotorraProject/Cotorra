using System;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class ActionClient : IActionClient
    {
        private readonly IActionClient _client;
        public ActionClient(string authorizationHeader, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        {
            _client = ClientAdapterFactory2.GetInstance<IActionClient>(this.GetType(), authorizationHeader, clientadapter: clientadapter);
        }

        public Task DispatchAsync(Guid actionID, Guid registerID)
        {
            return _client.DispatchAsync(actionID, registerID);
        }
    }
}

