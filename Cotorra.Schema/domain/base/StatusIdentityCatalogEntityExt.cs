using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    public abstract class StatusIdentityCatalogEntityExt : IdentityCatalogEntityExt, IStatusFull
    {
        [DataMember]
        public CotorriaStatus LocalStatus { get; set; }

    
        [DataMember]
        public DateTime LastStatusChange { get; set; }
    }
}
