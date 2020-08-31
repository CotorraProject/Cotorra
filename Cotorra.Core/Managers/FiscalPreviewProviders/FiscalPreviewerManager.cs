using Cotorra.Core.Managers.FiscalPreview;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core
{
    public class FiscalPreviewerManager
    {
        public async Task<PreviewTransformResult> TransformAsync(PreviewTransformParams previewTransformParams)
        {
            IFiscalPreview fiscalPreview = FiscalPreviewFactory.CreateInstance(previewTransformParams.FiscalStampingVersion);
            return await fiscalPreview.TransformAsync(previewTransformParams);
        }
    }
}
