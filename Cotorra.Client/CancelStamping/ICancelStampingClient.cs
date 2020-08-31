using Cotorra.Schema;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public interface ICancelStampingClient
    {
        Task<CancelPayrollStampingResult> CancelPayrollStampingAsync(CancelPayrollStampingParams cancelPayrollStampingParams);
    }
}
