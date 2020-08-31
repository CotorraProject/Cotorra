using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class IMSSWorkRisk : IdentityCatalogEntityExt
    {
        [DataMember]
        public DateTime ValidityDate { get; set; }

        [DataMember]
        public decimal Value { get; set; }
    }
}
