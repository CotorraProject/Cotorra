using System;
using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    public class AnualIncomeTax : IdentityCatalogEntityExt
    {
        [DataMember]
        public DateTime ValidityDate { get; set; }

        [DataMember]
        public Decimal LowerLimit { get; set; }

        [DataMember]
        public Decimal FixedFee { get; set; }

        [DataMember]
        public Decimal Rate { get; set; }

    }
}
