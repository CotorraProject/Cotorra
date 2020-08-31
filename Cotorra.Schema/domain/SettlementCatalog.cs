using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class SettlementCatalog : IdentityCatalogEntityExt
    {
        [DataMember]
        public DateTime ValidityDate { get; set; }
        
        [DataMember]
        public int Number { get; set; }

        [DataMember]
        public int Code { get; set; }

        [DataMember]
        public decimal CASUSMO { get; set; }

        [DataMember]
        public decimal CASISR86 { get; set; }

        [DataMember]
        public decimal CalDirecPerc { get; set; }

        [DataMember]
        public decimal Indem90 { get; set; }

        [DataMember]
        public decimal Indem20 { get; set; }

        [DataMember]
        public decimal PrimaAntig { get; set; }
    }
}
