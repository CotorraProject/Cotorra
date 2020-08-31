using Cotorra.Schema;
using System;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class InitializationClient : IInitializationClient
    {
        private readonly IInitializationClient _client;

        public InitializationClient(string authorizationHeader, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        {
            _client = ClientAdapterFactory2.GetInstance<IInitializationClient>(this.GetType(), authorizationHeader, clientadapter: clientadapter);
        }       

        public async Task<InitializationResult> InitializeAsync(string authTkn, Guid licenseServiceID, string socialReason, string RFC,
            PayrollCompanyConfiguration payrollCompanyConfiguration, EmployerRegistration employerRegistration)
        {
            return await _client.InitializeAsync(authTkn, licenseServiceID, socialReason, RFC, payrollCompanyConfiguration, employerRegistration);
        }
    }
}
