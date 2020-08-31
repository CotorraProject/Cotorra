using CotorraNode.Common.Config;
using CotorraNode.Common.Proxy;
using Cotorra.ClientProxy;
using Cotorra.Schema;
using System;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class CotorriaBotClientProxy : ICotorriaBotClient
    {
        private readonly string _authorizationHeader;
        private readonly string _cotorraUri;
        public CotorriaBotClientProxy(string authorizationHeader)
        {
            _cotorraUri = $"{ConfigManager.GetValue("CotorraService")}api/CotorriaBot";
            _authorizationHeader = authorizationHeader;
        }

         
        public async Task<string> GetIntent(GetIntentParams parameters)
        { 
            string result = null;

            //call service async
            await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.GET, _authorizationHeader,
                  new Uri($"{_cotorraUri}/GetIntent/{parameters.IdentityWorkID}/{parameters.InstanceID}/{parameters.Utterance}"),
                  new object[] { })
                .ContinueWith((i) =>
                {
                    if (i.Exception != null)
                    {
                        throw i.Exception;
                    }

                    result = i.Result;
                });

            return result;


        }
    }
}
