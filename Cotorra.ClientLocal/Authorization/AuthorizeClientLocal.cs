using Cotorra.Core.Managers;
using Cotorra.Schema;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class AuthorizeClientLocal : IAuthorizeClient
    {
        public AuthorizeClientLocal(string authorizationHeader)
        {

        }

        public async Task<List<Overdraft>> AuthorizationAsync(AuthorizationParams authorizationParams)
        {
            var authorizationManager = new AuthorizationManager();
            return await authorizationManager.AuthorizationAsync(authorizationParams);
        }
    }
}
