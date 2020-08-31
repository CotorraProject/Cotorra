using Cotorra.Schema;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public interface IAuthorizeClient
    {
        Task<List<Overdraft>> AuthorizationAsync(AuthorizationParams authorizationParams);
    }
}
