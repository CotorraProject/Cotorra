using CotorraNode.Common.Config;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQL; 
using System.Threading.Tasks;
using Cotorra.Client;

namespace Cotorra.Client
{
    public class SearchClientProxy : ISearchClient
    {
        private string _authorizationHeader;
        private string _cotorraUri;
        private readonly IConfigProvider _configProvider;
        private string _grapqhlEndpoint;


        public SearchClientProxy(string authorizationHeader)
        {
            _cotorraUri = ConfigManager.GetValue("CotorraService");
            _grapqhlEndpoint = ConfigManager.GetValue("CotorraService") + ConfigManager.GetValue("GraphqlEndpoint");
            _authorizationHeader = authorizationHeader;
        }
         
        public SearchClientProxy(string authorizationHeader, IConfigProvider configProvider)
        {
            _configProvider = configProvider;
            _cotorraUri = _configProvider.GetValue("CotorraService");
            //var graph = _configProvider.GetValue("GraphqlEndpoint");
            _grapqhlEndpoint = _cotorraUri + "graphql";
            _authorizationHeader = authorizationHeader;
        }

        public async Task<T> QueryAsync<T>(string query) 

        {
            var graphQLClient = new GraphQLHttpClient(_grapqhlEndpoint, new NewtonsoftJsonSerializer());
            var request = new GraphQLRequest
            {
                Query = query
            };
            var graphQLResponse = await graphQLClient.SendQueryAsync<T>(request);
            return graphQLResponse.Data;
        }

    }
}
