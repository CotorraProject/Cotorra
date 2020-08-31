using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Managers.FiscalValidator
{
    public static class FiscalValidatorFactory
    {
        /// <summary>
        /// Create instance of Validator provider
        /// </summary>
        /// <param name="fiscalStampingVersion">Versión de Timbrado</param>
        /// <returns></returns>
        public static IFiscalValidator CreateInstance(FiscalStampingVersion fiscalStampingVersion)
        {
            if (fiscalStampingVersion == FiscalStampingVersion.CFDI33_Nom12)
            {
                return new FiscalValidatorCFDIv33Nomv12();
            }

            throw new CotorraException(50002, "50002", "No es posible crear un proveedor de validación de xml para la versión especificada.", null);
        }
    }
}
