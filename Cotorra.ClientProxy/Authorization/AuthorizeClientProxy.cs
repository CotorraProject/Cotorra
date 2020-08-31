using CotorraNode.Common.Config;
using CotorraNode.Common.Proxy;
using Cotorra.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cotorra.ClientProxy;

namespace Cotorra.Client
{
    public class AuthorizeClientProxy : IAuthorizeClient
    {
        private readonly string _authorizationHeader;
        private readonly string _cotorraUri;

        public AuthorizeClientProxy(string authorizationHeader)
        {
            _cotorraUri = $"{ConfigManager.GetValue("CotorraService")}api/Overdraft";
            _authorizationHeader = authorizationHeader;
        }

        public async Task<List<Overdraft>> AuthorizationAsync(AuthorizationParams authorizationParams)
        {
            //call service async
            var result = await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, _authorizationHeader,
                   new Uri($"{_cotorraUri}/Authorize"), new object[] { authorizationParams }).ContinueWith((i) =>
                   {
                       if (i.Exception != null)
                       {
                           throw i.Exception;
                       }

                       return i.Result;
                   });

            var overdraft = JsonConvert.DeserializeObject<List<Overdraft>>(result);
            return overdraft;
        }
    }
}
