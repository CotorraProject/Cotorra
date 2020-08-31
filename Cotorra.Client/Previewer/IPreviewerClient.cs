using Cotorra.Schema;
using System;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public interface IPreviewerClient
    {
        Task<GetPreviewResult> GetPreviewByOverdraft(GetPreviewParams getPreviewParams);

        Task<GetPreviewUrlResult> GetPreviewUrlByUUIDAsync(Guid instanceId, Guid UUID);

        Task<GetPreviewCancelationAckURLResult> GetPreviewCancelationAckURLAsync(Guid cancelationResponseXMLID, Guid instanceID);
    }
}
