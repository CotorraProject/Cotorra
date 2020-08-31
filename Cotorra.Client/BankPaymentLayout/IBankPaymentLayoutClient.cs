using Cotorra.Schema;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public interface IBankPaymentLayoutClient
    {
        Task<string> GenerateBankLayoutPeriod(BankLayoutPaymentInformation bankLayoutPaymentInformation);

    }
}
