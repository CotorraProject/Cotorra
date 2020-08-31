using Cotorra.Schema;
using System;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class CancelStampingClient : ICancelStampingClient
    {
        private readonly ICancelStampingClient _client;
        public CancelStampingClient(string authorizationHeader, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        {
            _client = ClientAdapterFactory2.GetInstance<ICancelStampingClient>(this.GetType(), authorizationHeader, clientadapter: clientadapter);
        }

        public async Task<CancelPayrollStampingResult> CancelPayrollStampingAsync(CancelPayrollStampingParams cancelPayrollStampingParams)
        {
            return await _client.CancelPayrollStampingAsync(cancelPayrollStampingParams);
        }
    }
}
