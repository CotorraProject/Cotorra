using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class AuthorizeClient : IAuthorizeClient
    {
        private readonly IAuthorizeClient _client;
        public AuthorizeClient(string authorizationHeader, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        {
            _client = ClientAdapterFactory2.GetInstance<IAuthorizeClient>(this.GetType(), authorizationHeader, clientadapter: clientadapter);
        }

        public async Task<List<Overdraft>> AuthorizationAsync(AuthorizationParams authorizationParams)
        {
            return await _client.AuthorizationAsync(authorizationParams);
        }
    }
}
