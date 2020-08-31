using Cotorra.Schema.nom035;
using System;
using System.Threading.Tasks;

namespace Cotorra.Client.nom035
{
    public class NOMSurveyReplyClient : INOMSurveyReplyClient
    {
        private readonly INOMSurveyReplyClient _client;

        public NOMSurveyReplyClient(string authorizationHeader, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        {
            _client = ClientAdapterFactory2.GetInstance<INOMSurveyReplyClient>(this.GetType(), authorizationHeader, clientadapter: clientadapter);
        }

        public async Task<NOMSurveyReplyResult> CreateAsync(NOMSurveyReplyParams nOMSurveyReplyParams)
        {
            return await _client.CreateAsync(nOMSurveyReplyParams);
        }
    }
}
