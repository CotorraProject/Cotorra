using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.DTO
{
    [DataContract]
    public class MinimumSalaryDTO
    {
        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public DateTime ExpirationDate { get; set; }

        [DataMember]
        public decimal ZoneA { get; set; }

        [DataMember]
        public decimal ZoneB { get; set; }

        [DataMember]
        public decimal ZoneC { get; set; }

    }

   
}
