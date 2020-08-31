using CotorraNode.Common.Config;
using CotorraNode.Common.Proxy;
using Newtonsoft.Json;
using Cotorra.ClientProxy;
using Cotorra.Schema;
using System;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class ActionClientProxy : IActionClient
    {
        private readonly string _authorizationHeader;
        private readonly string _cotorraUri;
        public ActionClientProxy(string authorizationHeader)
        {
            _cotorraUri = $"{ConfigManager.GetValue("CotorraService")}api/Action";
            _authorizationHeader = authorizationHeader;
        }

        public async Task DispatchAsync(Guid actionID, Guid registerID)
        {
            DispatchAsyncParams parameters = new DispatchAsyncParams() { ActionID = actionID, RegisterID = registerID };
            var result = await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.GET, _authorizationHeader,
            new Uri($"{_cotorraUri}/GetIntent"), new object[] { parameters }).ContinueWith((i) =>
            {
                if (i.Exception != null)
                {
                    throw i.Exception;
                }
                return i.Result;
            });
           
        }

    }
}
