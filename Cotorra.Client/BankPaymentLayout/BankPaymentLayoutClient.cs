using Cotorra.Schema;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class BankPaymentLayoutClient : IBankPaymentLayoutClient
    {
        private readonly IBankPaymentLayoutClient _client;
        public BankPaymentLayoutClient(string authorizationHeader, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        {
            _client = ClientAdapterFactory2.GetInstance<IBankPaymentLayoutClient>(this.GetType(), authorizationHeader, clientadapter: clientadapter);
        }

        public Task<string> GenerateBankLayoutPeriod(BankLayoutPaymentInformation bankLayoutPaymentInformation)
        {
            return _client.GenerateBankLayoutPeriod(bankLayoutPaymentInformation);
        }
    }
}

