using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Managers.FiscalStamping
{
    public static class CancelStampingFactory
    {
         public static ICancelStamping CreateInstance(FiscalStampingVersion fiscalStampingVersion)
    {
        if (fiscalStampingVersion == FiscalStampingVersion.CFDI33_Nom12)
        {
            return new CancelStampingCFDIv33Nomv12();
        }

        throw new CotorraException(50001, "50001", "No es posible crear un proveedor de cancelación de timbrado para la versión especificada.", null);
    }
}
}
