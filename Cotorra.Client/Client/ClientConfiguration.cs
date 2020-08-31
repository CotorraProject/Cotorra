using CotorraNode.Common.Config;
using CotorraNode.Security.ACS.Client;
using CotorraNode.Security.ACS.Client.Schema.Entities;
using System;
using System.Collections.Concurrent;
using System.Runtime.Serialization;

namespace Cotorra.Client
{
    //Class that contains common configuration and constants
    public class ClientConfiguration
    {
        [DataContract]        
        private class ClientConfigHelper
        {
            [DataMember]
            public string User { get; set; }
            [DataMember]
            public string Password { get; set; }
        }

        [DataContract]
        private class ClientConfigHelperValues
        {
            [DataMember]
            public string Token { get; set; }
            [DataMember]
            public DateTime ExpiresOn { get; set; }
        }

        private static ClientAdapter? _clientProvider = null;

        private static ConcurrentDictionary<ClientConfigHelper, ClientConfigHelperValues> ClientConfigHelpers = new ConcurrentDictionary<ClientConfigHelper, ClientConfigHelperValues>();

        public enum ClientAdapter
        {
            Proxy,
            Local,
            Internal
        };

        private static bool isValidToken(ClientConfigHelperValues clientConfigHelper)
        {
            return DateTime.UtcNow < clientConfigHelper.ExpiresOn;
        }

        private static ClientConfigHelperValues GetExistsToken(string user, string pass)
        {
            ClientConfigHelperValues valuesToReturn;
            var clientConfigHelper = new ClientConfigHelper() { User = user, Password = pass };
            ClientConfigHelpers.TryGetValue(clientConfigHelper, out valuesToReturn);

            if (null != valuesToReturn)
            {
                if (!isValidToken(valuesToReturn))
                {
                    valuesToReturn = null;
                }
            }

            return valuesToReturn;
        }

        private static void CreateCacheToken(string user, string pass, DateTime expiresOn, string token)
        {
            var key = new ClientConfigHelper() { User = user, Password = pass };
            var value = new ClientConfigHelperValues() { ExpiresOn = expiresOn, Token = token };
            ClientConfigHelpers.TryAdd(key, value);
        }

        //GEt authorization header from username and password in configuration apps settings serviceUsername and servicePassword
        public static string GetAuthorizationHeader()
        {
            return String.Empty;
        }

        //GEt authorization header from username and password in configuration apps settings serviceUsername and servicePassword
        public static string GetAuthorizationHeader(IConfigProvider configProvider)
        {
            return String.Empty;
        }

        public static string GetAuthorizationHeader(string user, string pass)
        {
            using (ACSClient client = new ACSClient(ConfigManager.GetValue("ACSService")))
            {
                var resultTokenCache = GetExistsToken(user, pass);
                if (null == resultTokenCache)
                {
                    var loginResult = client.Login(new ServiceCredential() { Name = user, Password = pass });
                    var token = loginResult.AccessToken.AuthorizationHeader;
                    CreateCacheToken(user, pass, loginResult.AccessToken.ExpiresOn, token);
                    return token;
                }
                else
                {
                    return resultTokenCache.Token;
                }
            }
        }


        public static string GetAuthorizationHeaderUser()
        {
            return String.Empty;
        }

        public static string GetAuthorizationHeaderUser(string user, string pass)
        {
            UserCredential userCred = new UserCredential()
            {
                Name = user,
                Password = pass
            };
            return GetAuthorizationHeaderUser(userCred);
        }

        public static string GetAuthorizationHeaderUser(UserCredential cred)
        {
            using (ACSClient client = new ACSClient(ConfigManager.GetValue("ACSService")))
            {
                var resultTokenCache = GetExistsToken(cred.Name, cred.Password);
                if (null == resultTokenCache)
                {
                    var loginResult = client.Login(cred);
                    var token = loginResult.AccessToken.AuthorizationHeader;
                    CreateCacheToken(cred.Name, cred.Password, loginResult.AccessToken.ExpiresOn, token);
                    return token;
                }
                else
                {
                    return resultTokenCache.Token;
                }
            }
        }

        //Obtiene adapter (tipo de cliente) de archivo de configuración
        public static ClientAdapter GetAdapterFromConfig()
        {
            try
            {
                if (null == _clientProvider)
                {
                    string configAdapterValue = ConfigManager.GetValue("clientCotorraAdapter");
                    ClientAdapter adapter;
                    Enum.TryParse(configAdapterValue, out adapter);
                    _clientProvider = adapter;
                }

                return (ClientAdapter)_clientProvider;
            }
            catch
            {
                //Retorna adapter default si falla lectura de archivo de configuración
                return ClientConfiguration.ClientAdapter.Local;
            }
        }


        public static ClientAdapter GetAdapterFromConfig(IConfigProvider iConfigProvider)
        {
            try
            {
                if (null == _clientProvider)
                {
                    string configAdapterValue = iConfigProvider.GetValue("clientCotorraAdapter");
                    ClientConfiguration.ClientAdapter adapter;
                    Enum.TryParse(configAdapterValue, out adapter);
                    _clientProvider = adapter;
                }

                return (ClientAdapter)_clientProvider;
            }
            catch
            {
                //Retorna adapter default si falla lectura de archivo de configuración
                return ClientConfiguration.ClientAdapter.Proxy;
            }
        }
    }
}
