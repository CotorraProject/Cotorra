using Cotorra.Schema;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public interface IStampingClient
    {
        Task<PayrollStampingResult> PayrollStampingAsync(PayrollStampingParams payrollStampingParams);

        Task<PayrollStampingResult> PayrollIndividualStampingAsync(PayrollIndividualStampingParams payrollIndividualStampingParams);
    }
}
