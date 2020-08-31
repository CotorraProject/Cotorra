using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CotorraNode.Common.Config;
using CotorraNode.Common.Proxy;
using Newtonsoft.Json;
using Cotorra.Client;
using Cotorra.ClientProxy;
using Cotorra.Schema;
using Cotorra.Schema.DTO.Catalogs;
using Serialize.Linq.Serializers;

namespace Cotorra.Client
{
    public class PreviewerClientProxy : IPreviewerClient
    {
        private string _authorizationHeader;
        private string _cotorraUri;
        private readonly IConfigProvider _configProvider;

        public PreviewerClientProxy(string authorizationHeader)
        {
            _cotorraUri = $"{ConfigManager.GetValue("CotorraService")}api/Preview";
            _authorizationHeader = authorizationHeader;
        }

        public PreviewerClientProxy(string authorizationHeader, IConfigProvider configProvider)
        {
            _configProvider = configProvider;
            _cotorraUri = $"{_configProvider.GetValue("CotorraService")}api/Preview";
            _authorizationHeader = authorizationHeader;
        }

        public async Task<GetPreviewResult> GetPreviewByOverdraft(GetPreviewParams getPreviewParams)
        {
            var serializer = new ExpressionSerializer(new Serialize.Linq.Serializers.JsonSerializer());

            //call service async                        
            string result = null;

            //call service async
            await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.GET, _authorizationHeader,
                  new Uri($"{_cotorraUri}/GetPreviewByOverdraft/{getPreviewParams.IdentityWorkID}/{getPreviewParams.InstanceID}/{getPreviewParams.OverdraftID}"),
                  new object[] { })
                .ContinueWith((i) =>
                {
                    if (i.Exception != null)
                    {
                        throw i.Exception;
                    }

                    result = i.Result;
                });

            return JsonConvert.DeserializeObject<GetPreviewResult>(result);
        }

        public async Task<GetPreviewUrlResult> GetPreviewUrlByUUIDAsync(Guid instanceId, Guid UUID)
        {
            var serializer = new ExpressionSerializer(new Serialize.Linq.Serializers.JsonSerializer());

            //call service async                        
            string result = null;

            //call service async
            await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.GET, _authorizationHeader,
                  new Uri($"{_cotorraUri}/GetPreviewUrlByUUIDAsync/{instanceId}/{UUID}"),
                  new object[] { })
                .ContinueWith((i) =>
                {
                    if (i.Exception != null)
                    {
                        throw i.Exception;
                    }

                    result = i.Result;
                });

            return JsonConvert.DeserializeObject<GetPreviewUrlResult>(result);
        }

        public async Task<GetPreviewCancelationAckURLResult> GetPreviewCancelationAckURLAsync(Guid cancelationResponseXMLID, Guid instanceID)
        {

            //call service async                        
            string result = null;

            //call service async
            await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.GET, _authorizationHeader,
                  new Uri($"{_cotorraUri}/GetPreviewCancelationAckURLAsync/{cancelationResponseXMLID}/{instanceID}"),
                  new object[] { })
                .ContinueWith((i) =>
                {
                    if (i.Exception != null)
                    {
                        throw i.Exception;
                    }

                    result = i.Result;
                });

            return JsonConvert.DeserializeObject<GetPreviewCancelationAckURLResult>(result);
        }
    }
}