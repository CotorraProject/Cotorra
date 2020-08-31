using CotorraNode.Common.Base.Schema;
using CotorraNode.Common.Config;
using CotorraNode.Common.Proxy;
using Cotorra.Client;
using Cotorra.Schema;
using Newtonsoft.Json;
using Serialize.Linq.Serializers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Cotorra.ClientProxy
{
    public class ClientProxy<T> : IClient<T> where T : BaseEntity
    {
        private IValidator<T> _validator;
        private string _authorizationHeader;
        private string _cotorraUri;
        private string _cotorraExtensionsUri;
        private readonly IConfigProvider _configProvider;

       
        public IValidator<T> Validator
        {
            get => _validator; set => _validator = value;
        }

        public ClientProxy(string authorizationHeader, IValidator<T> validator)
        {
            _cotorraUri = $"{ConfigManager.GetValue("CotorraService")}api/client";
            _cotorraExtensionsUri = $"{ConfigManager.GetValue("CotorraService")}api/clientextension";
            _validator = validator;
            _authorizationHeader = authorizationHeader;
        }

        public ClientProxy(string authorizationHeader)
        {
            _cotorraUri = $"{ConfigManager.GetValue("CotorraService")}api/client";
            _cotorraExtensionsUri = $"{ConfigManager.GetValue("CotorraService")}api/clientextension";
            _authorizationHeader = authorizationHeader;
        }

        public ClientProxy(string authorizationHeader, IConfigProvider configProvider)
        {
            _configProvider = configProvider;
            _cotorraUri = $"{_configProvider.GetValue("CotorraService")}api/client";
            _cotorraExtensionsUri = $"{_configProvider.GetValue("CotorraService")}api/clientextension";
            _authorizationHeader = authorizationHeader;
        }

        public async Task CreateAsync(List<T> lstObjects, Guid identityWorkId)
        {
            var createParmas = new ClientParams();
            createParmas.FullNameType = typeof(T).FullName;
            createParmas.IdentityWorkID = identityWorkId;
            createParmas.JsonListObjects = JsonConvert.SerializeObject(lstObjects);

            //call service async
            await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, _authorizationHeader,
                   new Uri($"{_cotorraUri}"), new object[] { createParmas }).ContinueWith((i) =>
                   {
                       if (i.Exception != null)
                       {
                           throw i.Exception;
                       }
                   });
        }

        public async Task CreateAsync(List<T> lstObjects, LicenseParams parameters)
        {
            var createParmas = new ClientParams();
            createParmas.FullNameType = typeof(T).FullName;
            createParmas.IdentityWorkID = parameters.IdentityWorkID;
            createParmas.JsonListObjects = JsonConvert.SerializeObject(lstObjects);
            createParmas.LicenseServiceID = parameters.LicenseServiceID;
            createParmas.LicenseID = parameters.LicenseID;
            createParmas.AuthTkn = parameters.AuthTkn;

            //call service async
            await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, _authorizationHeader,
                   new Uri($"{_cotorraUri}"), new object[] { createParmas }).ContinueWith((i) =>
                   {
                       if (i.Exception != null)
                       {
                           throw i.Exception;
                       }
                   });
        }

        public async Task DeleteAsync(List<Guid> lstGuids, Guid identityWorkId)
        {
            var ids = JsonConvert.SerializeObject(lstGuids);

            //call service async
            await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.DELETE, _authorizationHeader,
                   new Uri($"{_cotorraUri}/{identityWorkId}/{typeof(T).FullName}/{ids}"), new object[] { null }).ContinueWith((i) =>
                   {
                       if (i.Exception != null)
                       {
                           throw i.Exception;
                       }
                   });

        }

        public async Task DeleteAsync(List<Guid> lstGuids, LicenseParams parameters)
        {
            var ids = JsonConvert.SerializeObject(lstGuids);

            //call service async
            await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.DELETE, _authorizationHeader,
                   new Uri($"{_cotorraUri}/{parameters.IdentityWorkID}/{typeof(T).FullName}/{ids}"), new object[] { null }).ContinueWith((i) =>
                   {
                       if (i.Exception != null)
                       {
                           throw i.Exception;
                       }
                   });

        }

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, Guid identityWorkId, string[] objectsToInclude = null)
        {
            var serializer = new ExpressionSerializer(new Serialize.Linq.Serializers.JsonSerializer());

            //call service async                        
            string result = null;

            var findComplexParams = new FindComplexParams();
            findComplexParams.TypeFullName = typeof(T).FullName;
            findComplexParams.IdentityWorkId = identityWorkId;
            findComplexParams.ExpressionNode = serializer.SerializeText(predicate);
            findComplexParams.ObjectsToInclude = objectsToInclude;
            findComplexParams.OperationType = OperationType.Select;

            //call service async
            await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, _authorizationHeader,
                  new Uri($"{_cotorraExtensionsUri}"), new object[] { findComplexParams }).ContinueWith((i) =>
                  {
                      if (i.Exception != null)
                      {
                          throw i.Exception;
                      }

                      result = i.Result;
                  });

            return JsonConvert.DeserializeObject<List<T>>(result);
        }

        public async Task<List<T>> GetAllAsync(Guid identityWorkId, Guid instanceId, string[] objectsToInclude = null)
        {
            //call service async
            string lstObjectsToInclude = null;
            if (objectsToInclude != null)
            {
                for (int i = 0; i < objectsToInclude.Count(); i++)
                {
                    lstObjectsToInclude = lstObjectsToInclude + $"objectsToInclude={objectsToInclude[i]}";

                    if (i <= objectsToInclude.Count() - 1)
                    {
                        lstObjectsToInclude = lstObjectsToInclude + "&";
                    }
                }
            }

            string result = null;
            if (!String.IsNullOrEmpty(lstObjectsToInclude))
            {
                await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.GET, _authorizationHeader,
                    new Uri($"{_cotorraUri}/{identityWorkId}/{instanceId}/{typeof(T).FullName}?{lstObjectsToInclude}"), new object[] { null }).ContinueWith((i) =>
                    {
                        if (i.Exception != null)
                        {
                            throw i.Exception;
                        }

                        result = i.Result;
                    });
            }
            else
            {
                await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.GET, _authorizationHeader,
                       new Uri($"{_cotorraUri}/{identityWorkId}/{instanceId}/{typeof(T).FullName}"), new object[] { null }).ContinueWith((i) =>
                       {
                           if (i.Exception != null)
                           {
                               throw i.Exception;
                           }

                           result = i.Result;
                       });
            }
            return JsonConvert.DeserializeObject<List<T>>(result);

        }

        public async Task<List<T>> GetByIdsAsync(List<Guid> lstGuids, Guid identityWorkId, string[] objectsToInclude = null)
        {
            //call service async                        
            string result = null;

            string lstObjectsToInclude = null;
            if (objectsToInclude != null)
            {
                for (int i = 0; i < objectsToInclude.Count(); i++)
                {
                    lstObjectsToInclude = lstObjectsToInclude + $"objectsToInclude={objectsToInclude[i]}";

                    if (i <= objectsToInclude.Count() - 1)
                    {
                        lstObjectsToInclude = lstObjectsToInclude + "&";
                    }
                }
            }

            string lst = null;
            if (lstGuids != null)
            {
                for (int i = 0; i < lstGuids.Count(); i++)
                {
                    lst = lst + $"ids={lstGuids[i]}";

                    if (i <= lstGuids.Count() - 1)
                    {
                        lst = lst + "&";
                    }
                }
            }

            if (!String.IsNullOrEmpty(lstObjectsToInclude))
            {
                await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.GET, _authorizationHeader,
               new Uri($"{_cotorraUri}/{identityWorkId}/{typeof(T).FullName}?{lst}&{lstObjectsToInclude}"), new object[] { null }).ContinueWith((i) =>
               {
                   if (i.Exception != null)
                   {
                       throw i.Exception;
                   }

                   result = i.Result;
               });
            }
            else
            {
                await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.GET, _authorizationHeader,
                  new Uri($"{_cotorraUri}/{identityWorkId}/{typeof(T).FullName}?{lst}"), new object[] { null }).ContinueWith((i) =>
                  {
                      if (i.Exception != null)
                      {
                          throw i.Exception;
                      }

                      result = i.Result;
                  });
            }
            return JsonConvert.DeserializeObject<List<T>>(result);
        }

        public async Task UpdateAsync(List<T> lstObjects, Guid identityWorkId)
        {
            var createParmas = new ClientParams();
            createParmas.FullNameType = typeof(T).FullName;
            createParmas.IdentityWorkID = identityWorkId;
            createParmas.JsonListObjects = JsonConvert.SerializeObject(lstObjects);

            var id = lstObjects.FirstOrDefault().ID;

            //call service async
            await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.PUT, _authorizationHeader,
                  new Uri($"{_cotorraUri}/{id}"), new object[] { createParmas }).ContinueWith((i) =>
                  {
                      if (i.Exception != null)
                      {
                          throw i.Exception;
                      }
                  });
        }

        internal Type GetType(string typeFullName)
        {
            return Assembly.GetAssembly(typeof(Employee)).GetType(typeFullName);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, Guid identityWorkId, Guid instanceId)
        {
            var serializer = new ExpressionSerializer(new Serialize.Linq.Serializers.JsonSerializer());

            //call service async                        
            string result = null;

            var findComplexParams = new FindComplexParams();
            findComplexParams.TypeFullName = typeof(T).FullName;
            findComplexParams.IdentityWorkId = identityWorkId;
            findComplexParams.InstanceId = instanceId;
            findComplexParams.ExpressionNode = serializer.SerializeText(predicate);
            findComplexParams.OperationType = OperationType.Count;

            //call service async
            await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, _authorizationHeader,
                  new Uri($"{_cotorraExtensionsUri}"), new object[] { findComplexParams }).ContinueWith((i) =>
                  {
                      if (i.Exception != null)
                      {
                          throw i.Exception;
                      }

                      result = i.Result;
                  });

            return JsonConvert.DeserializeObject<int>(result);
        }

        public async Task<int> CountAllAsync(Guid identityWorkId, Guid instanceId)
        {
            //call service async                        
            string result = null;

            var findComplexParams = new FindComplexParams();
            findComplexParams.TypeFullName = typeof(T).FullName;
            findComplexParams.IdentityWorkId = identityWorkId;
            findComplexParams.InstanceId = instanceId;
            findComplexParams.ExpressionNode = null;
            findComplexParams.OperationType = OperationType.Count;

            //call service async
            await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, _authorizationHeader,
                  new Uri($"{_cotorraExtensionsUri}"), new object[] { findComplexParams }).ContinueWith((i) =>
                  {
                      if (i.Exception != null)
                      {
                          throw i.Exception;
                      }

                      result = i.Result;
                  });

            return JsonConvert.DeserializeObject<int>(result);
        }

        public async Task DeleteAllAsync(Guid identityWorkId, Guid instanceId)
        {
            var findComplexParams = new FindComplexParams
            {
                TypeFullName = typeof(T).FullName,
                IdentityWorkId = identityWorkId,
                InstanceId = instanceId
            };

            //call service async
            await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, _authorizationHeader,
                      new Uri($"{_cotorraExtensionsUri}/DeleteAllAsync"), new object[] { findComplexParams }).ContinueWith((i) =>
                      {
                          if (i.Exception != null)
                          {
                              throw i.Exception;
                          }
                      });

        }

        public async Task DeleteByExpresssionAsync(Expression<Func<T, bool>> predicate, Guid identityWorkId)
        {
            var serializer = new ExpressionSerializer(new Serialize.Linq.Serializers.JsonSerializer());

            //call service async                        
            

            var findComplexParams = new FindComplexParams();
            findComplexParams.TypeFullName = typeof(T).FullName;
            findComplexParams.IdentityWorkId = identityWorkId;
            findComplexParams.ExpressionNode = serializer.SerializeText(predicate);
            findComplexParams.ObjectsToInclude = new string[] { };
            findComplexParams.OperationType = OperationType.Select;

            //call service async
            await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, _authorizationHeader,
                      new Uri($"{_cotorraExtensionsUri}/DeleteByExpresssionAsync"), new object[] { findComplexParams })
                        .ContinueWith((i) =>
                          {
                              if (i.Exception != null)
                              {
                                  throw i.Exception;
                              }
                          });
        }

        public async Task<List<T>> GetAllAsync(string[] objectsToInclude = null)
        {
            //call service async
            string lstObjectsToInclude = null;
            if (objectsToInclude != null)
            {
                for (int i = 0; i < objectsToInclude.Count(); i++)
                {
                    lstObjectsToInclude = lstObjectsToInclude + $"objectsToInclude={objectsToInclude[i]}";

                    if (i <= objectsToInclude.Count() - 1)
                    {
                        lstObjectsToInclude = lstObjectsToInclude + "&";
                    }
                }
            }

            string result = null;
            if (!String.IsNullOrEmpty(lstObjectsToInclude))
            {
                await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.GET, _authorizationHeader,
                    new Uri($"{_cotorraUri}/{typeof(T).FullName}?{lstObjectsToInclude}"), new object[] { null }).ContinueWith((i) =>
                    {
                        if (i.Exception != null)
                        {
                            throw i.Exception;
                        }

                        result = i.Result;
                    });
            }
            else
            {
                await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.GET, _authorizationHeader,
                       new Uri($"{_cotorraUri}/{typeof(T).FullName}"), new object[] { null }).ContinueWith((i) =>
                       {
                           if (i.Exception != null)
                           {
                               throw i.Exception;
                           }

                           result = i.Result;
                       });
            }
            return JsonConvert.DeserializeObject<List<T>>(result);
        }
    }
}
