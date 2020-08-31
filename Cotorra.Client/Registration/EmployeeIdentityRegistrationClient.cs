using System;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class EmployeeIdentityRegistrationClient : IEmployeeIdentityRegistrationClient
    {
        private readonly IEmployeeIdentityRegistrationClient _client;
        public EmployeeIdentityRegistrationClient(string authorizationHeader, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        {
            _client = ClientAdapterFactory2.GetInstance<IEmployeeIdentityRegistrationClient>(this.GetType(), authorizationHeader, clientadapter: clientadapter);
        }

        /// <summary>
        /// Get IdentityUser By Email
        /// </summary>
        /// <param name="authorizationHeader"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Guid?> GetIdentityUserAsync(string email)
        {
            return await _client.GetIdentityUserAsync(email);
        }
    }
}
