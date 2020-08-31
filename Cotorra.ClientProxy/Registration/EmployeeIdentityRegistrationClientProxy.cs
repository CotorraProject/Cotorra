using CotorraNode.Common.Config;
using Cotorra.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.ClientProxy
{
    public class EmployeeIdentityRegistrationClientProxy : IEmployeeIdentityRegistrationClient
    {
        private readonly string _authorizationHeader;
        private readonly string _cotorraUri;

        public EmployeeIdentityRegistrationClientProxy(string authorizationHeader)
        {
            _cotorraUri = $"{ConfigManager.GetValue("CotorraService")}api/EmployeeIdentityRegistration";
            _authorizationHeader = authorizationHeader;
        }

        public Task<Guid?> GetIdentityUserAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}
