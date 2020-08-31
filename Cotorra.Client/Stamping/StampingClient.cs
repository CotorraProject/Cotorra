using Cotorra.Schema;
using System;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class StampingClient : IStampingClient
    {
        private readonly IStampingClient _client;

        public StampingClient(string authorizationHeader, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        {
            _client = ClientAdapterFactory2.GetInstance<IStampingClient>(this.GetType(), authorizationHeader, clientadapter: clientadapter) as IStampingClient;
        }

        public async Task<PayrollStampingResult> PayrollStampingAsync(PayrollStampingParams payrollStampingParams)
        {
            return await _client.PayrollStampingAsync(payrollStampingParams);
        }

        public async Task<PayrollStampingResult> PayrollIndividualStampingAsync(PayrollIndividualStampingParams payrollIndividualStampingParams)
        {
            return await _client.PayrollIndividualStampingAsync(payrollIndividualStampingParams);
        }
    }
}
