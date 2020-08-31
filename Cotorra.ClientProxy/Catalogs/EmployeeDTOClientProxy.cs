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
    public class EmployeeDTOClientProxy : IEmployeeDTOClient
    {
        private string _authorizationHeader;
        private string _cotorraUri;
        private readonly IConfigProvider _configProvider;

        public EmployeeDTOClientProxy(string authorizationHeader)
        {
            _cotorraUri = $"{ConfigManager.GetValue("CotorraService")}api/payroll/Employee";
            _authorizationHeader = authorizationHeader;
        }

        public EmployeeDTOClientProxy(string authorizationHeader, IConfigProvider configProvider)
        {
            _configProvider = configProvider;
            _cotorraUri = $"{_configProvider.GetValue("CotorraService")}api/payroll/Employee";
            _authorizationHeader = authorizationHeader;
        }


        public async Task<EmployeeDTO> GetById(Guid companyId, Guid instanceId, Guid employeeId)
        {
            var serializer = new ExpressionSerializer(new Serialize.Linq.Serializers.JsonSerializer());

            //call service async                        
            string result = null;

            //call service async
            await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.GET, _authorizationHeader,
                  new Uri($"{_cotorraUri}/GetById/{companyId}/{instanceId}/{employeeId}"),
                  new object[] { })
                .ContinueWith((i) =>
                  {
                      if (i.Exception != null)
                      {
                          throw i.Exception;
                      }

                      result = i.Result;
                  });

            return JsonConvert.DeserializeObject<EmployeeDTO>(result);
        }

        public async Task<EmployeeDTO> GetByIdentityId(Guid identityUserID)
        {
            var serializer = new ExpressionSerializer(new Serialize.Linq.Serializers.JsonSerializer());

            //call service async                        
            string result = null;

            //call service async
            await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.GET, _authorizationHeader,
                  new Uri($"{_cotorraUri}/GetByIdentityId/{identityUserID}"),
                  new object[] { })
                .ContinueWith((i) =>
                {
                    if (i.Exception != null)
                    {
                        throw i.Exception;
                    }

                    result = i.Result;
                });

            return JsonConvert.DeserializeObject<EmployeeDTO>(result);
        }
    }
}
