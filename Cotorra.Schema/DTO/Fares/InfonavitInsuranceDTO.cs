using System;
using System.Runtime.Serialization;

namespace Cotorra.Schema.DTO
{

    [DataContract]
    public class InfonavitInsuranceDTO
    {
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public DateTime ValidityDate { get; set; }

        [DataMember]
        public Decimal Value { get; set; }


    }
}
