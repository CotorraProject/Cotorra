using Cotorra.Client;
using Cotorra.Core;
using System;
using System.Threading.Tasks;

namespace Cotorra.ClientLocal
{
    public class EmployeeIdentityRegistrationClientLocal : IEmployeeIdentityRegistrationClient
    {
        private readonly string _authorizationHeader;

        public EmployeeIdentityRegistrationClientLocal(string authorizationHeader)
        {
            _authorizationHeader = authorizationHeader;
        }

        public async Task<Guid?> GetIdentityUserAsync(string email)
        {
            var manager = new EmployeeIdentityRegistrationManager();
            return await manager.GetIdentityUserAsync(_authorizationHeader, email);
        }
    }
}
