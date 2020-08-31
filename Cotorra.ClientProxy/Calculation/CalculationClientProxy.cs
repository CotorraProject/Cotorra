using CotorraNode.Common.Config;
using CotorraNode.Common.Proxy;
using Newtonsoft.Json;
using Cotorra.Client;
using Cotorra.Schema;
using Cotorra.Schema.Calculation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.ClientProxy
{
    public class CalculationClientProxy : ICalculationClient
    {
        private readonly string _authorizationHeader;
        private readonly string _cotorraUri;

        public CalculationClientProxy(string authorizationHeader)
        {
            _cotorraUri = $"{ConfigManager.GetValue("CotorraService")}api/Overdraft";
            _authorizationHeader = authorizationHeader;
        }

        public async Task<CalculateGenericResult> CalculateFormulatAsync(CalculateGenericParams calculateGenericParams)
        {
            //call service async
            var result = await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, _authorizationHeader,
                   new Uri($"{_cotorraUri}/CalculateFormula"), new object[] { calculateGenericParams }).ContinueWith((i) =>
                   {
                       if (i.Exception != null)
                       {
                           throw i.Exception;
                       }

                       return i.Result;
                   });

            var calculateGenericResult = JsonConvert.DeserializeObject<CalculateGenericResult>(result);
            return calculateGenericResult;
        }

        public async Task<CalculateOverdraftResult> CalculateOverdraftAsync(CalculateOverdraftParams calculateOverdraftParams)
        {
            //call service async
            var result = await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, _authorizationHeader,
                   new Uri($"{_cotorraUri}/CalculateOverdraft"), new object[] { calculateOverdraftParams }).ContinueWith((i) =>
                   {
                       if (i.Exception != null)
                       {
                           throw i.Exception;
                       }

                       return i.Result;
                   });

            var calculateGenericResult = JsonConvert.DeserializeObject<CalculateOverdraftResult>(result);
            return calculateGenericResult;
        }

        public async Task CalculationFireAndForgetByEmployeesAsync(CalculationFireAndForgetByEmployeeParams calculationFireAndForgetByEmployeeParams)
        {
            //call service async
            await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, _authorizationHeader,
                   new Uri($"{_cotorraUri}/CalculationFireAndForgetByEmployee"), new object[] { calculationFireAndForgetByEmployeeParams }).ContinueWith((i) =>
                   {
                       if (i.Exception != null)
                       {
                           throw i.Exception;
                       }
                   });
        }

        public async Task CalculationByEmployeesAsync(CalculationFireAndForgetByEmployeeParams calculationFireAndForgetByEmployeeParams)
        {
            //call service async
            await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, _authorizationHeader,
                   new Uri($"{_cotorraUri}/CalculationByEmployee"), new object[] { calculationFireAndForgetByEmployeeParams }).ContinueWith((i) =>
                   {
                       if (i.Exception != null)
                       {
                           throw i.Exception;
                       }
                   });
        }

        public async Task CalculationFireAndForgetByPeriodIdsAsync(CalculationFireAndForgetByPeriodParams calculationFireAndForgetByPeriodParams)
        {
            //call service async
            await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, _authorizationHeader,
                   new Uri($"{_cotorraUri}/CalculationFireAndForgetByPeriodIds"), new object[] { calculationFireAndForgetByPeriodParams }).ContinueWith((i) =>
                   {
                       if (i.Exception != null)
                       {
                           throw i.Exception;
                       }
                   });
        }
    }
}
