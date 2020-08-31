using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core.Managers.FiscalPreview
{
    public interface IFiscalPreview
    {
        Task<PreviewTransformResult> TransformAsync(PreviewTransformParams previewTransformParams);

        Task<GetPreviewUrlResult> GetPreviewUrlByUUIDAsync(Guid instanceId, Guid UUID);

        Task<GetPreviewCancelationAckURLResult> GetPreviewCancelationAckURLAsync(Guid cancelationResponseXMLID, Guid instanceID);
    }
}
