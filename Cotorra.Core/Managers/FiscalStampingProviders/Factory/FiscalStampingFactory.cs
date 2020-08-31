using Cotorra.Schema;

namespace Cotorra.Core.Managers.FiscalStamping
{  
    /// <summary>
    /// Factory to create instance of stamping providers
    /// </summary>
    public static class FiscalStampingFactory
    {
        /// <summary>
        /// Create instance of Stamping provider
        /// </summary>
        /// <param name="fiscalStampingVersion"></param>
        /// <returns></returns>
        public static IFiscalStamping CreateInstance(FiscalStampingVersion fiscalStampingVersion)
        {
            if (fiscalStampingVersion == FiscalStampingVersion.CFDI33_Nom12)
            {
                return new FiscalStampingCFDIv33Nomv12();
            }

            throw new CotorraException(50001, "50001", "No es posible crear un proveedor de timbrado para la versión especificada.", null);
        }
    }
}
