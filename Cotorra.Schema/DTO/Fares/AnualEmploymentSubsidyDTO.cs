using System;
using System.Runtime.Serialization;

namespace Cotorra.Schema.DTO
{
     
    [DataContract]
    public class AnualEmploymentSubsidyDTO
    {
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public DateTime ValidityDate { get; set; }

        [DataMember]
        public Decimal LowerLimit { get; set; }

        [DataMember]
        public Decimal AnualSubsidy { get; set; }


    }
}
