
using CotorraNode.Common.Config;
using Cotorra.Schema;
using System;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class BankPaymentLayoutClientProxy : IBankPaymentLayoutClient
    {
        private readonly string _authorizationHeader;
        private readonly string _cotorraUri;

        public BankPaymentLayoutClientProxy(string authorizationHeader)
        {
            _cotorraUri = $"{ConfigManager.GetValue("CotorraService")}api/BankPaymentLayout";
            _authorizationHeader = authorizationHeader;
        }

        public Task<string> GenerateBankLayoutPeriod(BankLayoutPaymentInformation bankLayoutPaymentInformation)
        {
            throw new NotSupportedException("Try local");
        }
    }
}
