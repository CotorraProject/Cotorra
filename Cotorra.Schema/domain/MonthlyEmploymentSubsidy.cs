using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class MonthlyEmploymentSubsidy : IdentityCatalogEntityExt
    {
        [DataMember]
        public DateTime ValidityDate { get; set; }

        [DataMember]
        public Decimal LowerLimit { get; set; }

        [DataMember]
        public Decimal MonthlySubsidy { get; set; }


    }
}
