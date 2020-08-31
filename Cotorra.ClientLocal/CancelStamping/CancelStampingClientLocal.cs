using Cotorra.Core;
using Cotorra.Schema;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class CancelStampingClientLocal : ICancelStampingClient
    {
        public CancelStampingClientLocal(string authorizationHeader)
        {

        }

        public async Task<CancelPayrollStampingResult> CancelPayrollStampingAsync(CancelPayrollStampingParams cancelPayrollStampingParams)
        {
            var cancelStampingManager = new CancelStampingManager();
            return await cancelStampingManager.CancelPayrollStampingAsync(cancelPayrollStampingParams);
        }
    }
}
