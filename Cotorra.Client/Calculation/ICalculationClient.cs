using Cotorra.Schema;
using Cotorra.Schema.Calculation;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public interface ICalculationClient
    {
        Task<CalculateOverdraftResult> CalculateOverdraftAsync(CalculateOverdraftParams calculateOverdraftParams);

        Task<CalculateGenericResult> CalculateFormulatAsync(CalculateGenericParams calculateGenericParams);

        Task CalculationFireAndForgetByEmployeesAsync(CalculationFireAndForgetByEmployeeParams calculationFireAndForgetByEmployeeParams);

        Task CalculationByEmployeesAsync(CalculationFireAndForgetByEmployeeParams calculationFireAndForgetByEmployeeParams);


        Task CalculationFireAndForgetByPeriodIdsAsync(CalculationFireAndForgetByPeriodParams calculationFireAndForgetByPeriodParams);
    }
}
