using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.DTO.Catalogs
{
    [DataContract]
    public class PerceptionDTO
    {
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public decimal Amount { get; set; }
    }
}
