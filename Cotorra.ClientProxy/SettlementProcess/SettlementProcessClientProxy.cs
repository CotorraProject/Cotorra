using CotorraNode.Common.Config;
using CotorraNode.Common.Proxy;
using Newtonsoft.Json;
using Cotorra.ClientProxy;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class SettlementProcessClientProxy : ISettlementProcessClient
    {
        private readonly string _authorizationHeader;
        private readonly string _cotorraUri;
        public SettlementProcessClientProxy(string authorizationHeader)
        {
            _cotorraUri = $"{ConfigManager.GetValue("CotorraService")}api/SettlementProcess";
            _authorizationHeader = authorizationHeader;
        }

        public async Task<ApplySettlementProcessResult> ApplySettlement(ApplySettlementProcessParams parameters)
        {
             var result = await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, _authorizationHeader,
             new Uri($"{_cotorraUri}/ApplySettlement"), new object[] { parameters }).ContinueWith((i) =>
             {
                 if (i.Exception != null)
                 {
                     throw i.Exception;
                 }
                 return i.Result;
             });
            return JsonConvert.DeserializeObject<ApplySettlementProcessResult>(result);
        }

        public async Task<List<Overdraft>> Calculate(CalculateSettlementProcessParams parameters)
        {
            var result = await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, _authorizationHeader,
                new Uri($"{_cotorraUri}/Calculate"), new object[] { parameters }).ContinueWith((i) =>
                   {
                       if (i.Exception != null)
                       {
                           throw i.Exception;
                       }
                       return i.Result;
                   });
            return JsonConvert.DeserializeObject<List<Overdraft>>(result);
        }

        public async Task<string> GenerateSettlementLetter(GenerateSettlementLetterParams parameters)
        {
            var result = await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, _authorizationHeader,
             new Uri($"{_cotorraUri}/GenerateSettlementLetter"), new object[] { parameters }).ContinueWith((i) =>
             {
                 if (i.Exception != null)
                 {
                     throw i.Exception;
                 }
                 return i.Result;
             });
            return JsonConvert.DeserializeObject<string>(result);
        }
    }
}
