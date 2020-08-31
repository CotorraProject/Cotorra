using Cotorra.Core;
using Cotorra.Schema;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class StampingClientLocal : IStampingClient
    {
        public StampingClientLocal(string authorizationHeader)
        {

        }

        public async Task<PayrollStampingResult> PayrollStampingAsync(PayrollStampingParams payrollStampingParams)
        {
            var manager = new PayrollStampingManager();
            return await manager.PayrollStampingAsync(payrollStampingParams);
        }

        public async Task<PayrollStampingResult> PayrollIndividualStampingAsync(PayrollIndividualStampingParams payrollIndividualStampingParams)
        {
            var manager = new PayrollStampingManager();
            return await manager.PayrollIndividualStampingAsync(payrollIndividualStampingParams);
        }

    }
}
