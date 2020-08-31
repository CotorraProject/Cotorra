using CotorraNode.Common.Config;
using CotorraNode.Common.Proxy;
using Cotorra.Client.nom035;
using Cotorra.Schema.nom035;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.ClientProxy.nom035
{
    public class NOMSurveyReplyClientProxy : INOMSurveyReplyClient
    {
        private string _authorizationHeader;
        private static string _cotorraUri;

        static NOMSurveyReplyClientProxy()
        {
            _cotorraUri = $"{ConfigManager.GetValue("CotorraService")}api/NOMSurveyReply";
        }


        public NOMSurveyReplyClientProxy(string authorizationHeader)
        {
            _authorizationHeader = authorizationHeader;
        }

        public async Task<NOMSurveyReplyResult> CreateAsync(NOMSurveyReplyParams nOMSurveyReplyParams)
        {
            string serializedResult = await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, _authorizationHeader,
                    new Uri($"{_cotorraUri}"), new object[] { nOMSurveyReplyParams });
            if (!String.IsNullOrEmpty(serializedResult))
            {
                return JsonConvert.DeserializeObject<NOMSurveyReplyResult>(serializedResult);
            }
            return default;
        }
    }
}
