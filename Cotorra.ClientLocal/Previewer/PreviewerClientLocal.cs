using Cotorra.Core;
using Cotorra.Core.Managers.FiscalPreview;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class PreviewerClientLocal : IPreviewerClient
    {
        public PreviewerClientLocal(string authorizationHeader)
        {

        }

        public async Task<GetPreviewResult> GetPreviewByOverdraft(GetPreviewParams getPreviewParams)
        {
            GetPreviewResult getPreviewResult = new GetPreviewResult();

            var fiscalPreviewManager = new FiscalPreviewerManager();
            var previewTransformParams = new PreviewTransformParams();
            previewTransformParams.FiscalStampingVersion = getPreviewParams.FiscalStampingVersion;
            previewTransformParams.InstanceID = getPreviewParams.InstanceID;
            previewTransformParams.IdentityWorkID = getPreviewParams.IdentityWorkID;

            previewTransformParams.PreviewTransformParamsDetails.Add(new PreviewTransformParamsDetail()
            {
                OverdraftID = getPreviewParams.OverdraftID
            });

            //Transforma el XML a HTML y PDF
            getPreviewResult.PreviewTransformResult = await fiscalPreviewManager.TransformAsync(previewTransformParams);
            return getPreviewResult;
        }

        public async Task<GetPreviewUrlResult> GetPreviewUrlByUUIDAsync(Guid instanceId, Guid UUID)
        {
            var fiscalManager = FiscalPreviewFactory.CreateInstance(FiscalStampingVersion.CFDI33_Nom12);
            var result = await fiscalManager.GetPreviewUrlByUUIDAsync(instanceId, UUID);
            return result;
        }

        public async Task<GetPreviewCancelationAckURLResult> GetPreviewCancelationAckURLAsync(Guid cancelationResponseXMLID, Guid instanceID)
        {
            var fiscalManager = FiscalPreviewFactory.CreateInstance(FiscalStampingVersion.CFDI33_Nom12);
            var result = await fiscalManager.GetPreviewCancelationAckURLAsync(cancelationResponseXMLID, instanceID);
            return result;
        }
    }
}
