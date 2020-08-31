using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CotorraNode.Common.Config;
using CotorraNode.Common.Proxy;
using Newtonsoft.Json;
using Cotorra.Client;
using Cotorra.Schema.DTO.Catalogs;
using Serialize.Linq.Serializers;

namespace Cotorra.ClientProxy
{
    public class OverdraftDTOClientProxy : IOverdraftDTOClient
    {
        private string _authorizationHeader;
        private string _cotorraUri;
        private readonly IConfigProvider _configProvider;

        public OverdraftDTOClientProxy(string authorizationHeader)
        {
            _cotorraUri = $"{ConfigManager.GetValue("CotorraService")}api/Overdraft";
            _authorizationHeader = authorizationHeader;
        }

        public OverdraftDTOClientProxy(string authorizationHeader, IConfigProvider configProvider)
        {
            _configProvider = configProvider;
            _cotorraUri = $"{_configProvider.GetValue("CotorraService")}api/Overdraft";
            _authorizationHeader = authorizationHeader;
        }


        public async Task<List<OverdraftDTO>> GetByEmployeeId(Guid companyId, Guid instanceId, Guid employeeId)
        {
            var serializer = new ExpressionSerializer(new Serialize.Linq.Serializers.JsonSerializer());

            //call service async                        
            string result = null;

            //call service async
            await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.GET, _authorizationHeader,
                  new Uri($"{_cotorraUri}/GetByEmployeeId/{companyId}/{instanceId}/{employeeId}"),
                  new object[] { })
                .ContinueWith((i) =>
                {
                    if (i.Exception != null)
                    {
                        throw i.Exception;
                    }

                    result = i.Result;
                });

            return JsonConvert.DeserializeObject<List<OverdraftDTO>>(result);
        }

      
    }
}
