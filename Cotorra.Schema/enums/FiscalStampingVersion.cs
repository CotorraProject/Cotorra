using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    /// <summary>
    /// Version de timbrado a utilizar
    /// </summary>
    [DataContract]
    public enum FiscalStampingVersion
    {
        CFDI33_Nom12 = 1
    }
}
