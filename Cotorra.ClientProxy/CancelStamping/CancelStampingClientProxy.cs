using CotorraNode.Common.Config;
using Cotorra.Schema;
using System;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class CancelStampingClientProxy : ICancelStampingClient
    {
        private readonly string _authorizationHeader;
        private readonly string _cotorraUri;

        public CancelStampingClientProxy(string authorizationHeader)
        {
            _authorizationHeader = authorizationHeader;
            _cotorraUri = $"{ConfigManager.GetValue("CotorraService")}api/Stamping";
        }

        public Task<CancelPayrollStampingResult> CancelPayrollStampingAsync(CancelPayrollStampingParams cancelPayrollStampingParams)
        {
            throw new NotImplementedException();
        }
    }
}
