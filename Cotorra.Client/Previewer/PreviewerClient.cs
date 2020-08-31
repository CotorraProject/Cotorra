using Cotorra.Schema;
using System;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class PreviewerClient : IPreviewerClient
    {
        private readonly IPreviewerClient _client;

        public PreviewerClient(string authorizationHeader, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        {
            _client = ClientAdapterFactory2.GetInstance<IPreviewerClient>(this.GetType(), authorizationHeader, clientadapter: clientadapter);
        }

        public async Task<GetPreviewResult> GetPreviewByOverdraft(GetPreviewParams getPreviewParams)
        {
            return await _client.GetPreviewByOverdraft(getPreviewParams);
        }

        public async Task<GetPreviewCancelationAckURLResult> GetPreviewCancelationAckURLAsync(Guid cancelationResponseXMLID, Guid instanceID)
        {
            return await _client.GetPreviewCancelationAckURLAsync(cancelationResponseXMLID, instanceID);
        }

        public async Task<GetPreviewUrlResult> GetPreviewUrlByUUIDAsync(Guid instanceId, Guid UUID)
        {
            return await _client.GetPreviewUrlByUUIDAsync(instanceId, UUID);
        }
    }
}
