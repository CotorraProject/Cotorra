using Cotorra.Core;
using Cotorra.Schema;
using System;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class BankPaymentLayoutClientLocal : IBankPaymentLayoutClient
    {
        public BankPaymentLayoutClientLocal(string authorizationHeader)
        {
        }

        public async Task<string> GenerateBankLayoutPeriod(BankLayoutPaymentInformation bankLayoutPaymentInformation)
        {
            var urlLayout = await new BankPaymentLayoutManager().GenerateBankLayoutPeriod(bankLayoutPaymentInformation);

            return urlLayout;
        }
    }
}
