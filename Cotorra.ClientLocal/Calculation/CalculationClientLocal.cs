using Cotorra.Core.Managers.Calculation;
using Cotorra.Schema;
using Cotorra.Schema.Calculation;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class CalculationClientLocal : ICalculationClient
    {
        public CalculationClientLocal(string authorizationHeader)
        {
        }

        public async Task<CalculateOverdraftResult> CalculateOverdraftAsync(CalculateOverdraftParams calculateOverdraftParams)
        {
            ICalculationManager calculationManager = new OverdraftCalculationManager();
            var calculationResult = await calculationManager.CalculateAsync(calculateOverdraftParams) as CalculateOverdraftResult;
            return calculationResult;
        }

        public async Task<CalculateGenericResult> CalculateFormulatAsync(CalculateGenericParams calculateGenericParams)
        {
            ICalculationManager calculationManager = new CalculationGenericManager();
            var calculationResult = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
            return calculationResult;
        }

        public async Task CalculationFireAndForgetByEmployeesAsync(CalculationFireAndForgetByEmployeeParams calculationFireAndForgetByEmployeeParams)
        {
            var calculationManager = new OverdraftCalculationManager();
            await calculationManager.CalculationFireAndForgetByEmployeesAsync(
                calculationFireAndForgetByEmployeeParams.EmployeeIds, 
                calculationFireAndForgetByEmployeeParams.IdentityWorkID,
                calculationFireAndForgetByEmployeeParams.InstanceID,
                calculationFireAndForgetByEmployeeParams.UserID);
        }

        public async Task CalculationByEmployeesAsync(CalculationFireAndForgetByEmployeeParams calculationFireAndForgetByEmployeeParams)
        {
            var calculationManager = new OverdraftCalculationManager();
            await calculationManager.CalculationByEmployeesAsync(
                calculationFireAndForgetByEmployeeParams.EmployeeIds,
                calculationFireAndForgetByEmployeeParams.IdentityWorkID,
                calculationFireAndForgetByEmployeeParams.InstanceID,
                calculationFireAndForgetByEmployeeParams.UserID);
        }

        public async Task CalculationFireAndForgetByPeriodIdsAsync(CalculationFireAndForgetByPeriodParams calculationFireAndForgetByPeriodParams)
        {
            var calculationManager = new OverdraftCalculationManager();
            await calculationManager.CalculationFireAndForgetByPeriodIdsAsync(
                calculationFireAndForgetByPeriodParams.PeriodIds,
                calculationFireAndForgetByPeriodParams.IdentityWorkID,
                calculationFireAndForgetByPeriodParams.InstanceID,
                calculationFireAndForgetByPeriodParams.UserID);
        }
    }
}
