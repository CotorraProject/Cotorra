using Cotorra.Schema;
using Cotorra.Schema.Calculation;
using System;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class CalculationClient : ICalculationClient
    {
        private readonly ICalculationClient _client;
        public CalculationClient(string authorizationHeader, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        {
            _client = ClientAdapterFactory2.GetInstance<ICalculationClient>(this.GetType(), authorizationHeader, clientadapter: clientadapter);
        }

        public async Task<CalculateOverdraftResult> CalculateOverdraftAsync(CalculateOverdraftParams calculateOverdraftParams)
        {
            return await _client.CalculateOverdraftAsync(calculateOverdraftParams);
        }

        public async Task<CalculateGenericResult> CalculateFormulatAsync(CalculateGenericParams calculateGenericParams)
        {
            return await _client.CalculateFormulatAsync(calculateGenericParams);
        }

        public async Task CalculationFireAndForgetByEmployeesAsync(CalculationFireAndForgetByEmployeeParams calculationFireAndForgetByEmployeeParams)
        {
            await _client.CalculationFireAndForgetByEmployeesAsync(calculationFireAndForgetByEmployeeParams);
        }

        public async Task CalculationByEmployeesAsync(CalculationFireAndForgetByEmployeeParams calculationFireAndForgetByEmployeeParams)
        {
            await _client.CalculationByEmployeesAsync(calculationFireAndForgetByEmployeeParams);
        }

        public async Task CalculationFireAndForgetByPeriodIdsAsync(CalculationFireAndForgetByPeriodParams calculationFireAndForgetByPeriodParams)
        {
            await _client.CalculationFireAndForgetByPeriodIdsAsync(calculationFireAndForgetByPeriodParams);
        }
    }
}
