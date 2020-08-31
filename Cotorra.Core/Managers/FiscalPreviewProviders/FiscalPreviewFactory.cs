using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Managers.FiscalPreview
{
    public static class FiscalPreviewFactory
    {
        public static IFiscalPreview CreateInstance(FiscalStampingVersion fiscalStampingVersion)
        {
            if (fiscalStampingVersion == FiscalStampingVersion.CFDI33_Nom12)
            {
                return new FiscalPreviewCFDIv33Nom12();
            }

            throw new CotorraException(60001, "60001", "No es posible crear un proveedor de previsualizador para la versión especificada.", null);
        }
    }
}
