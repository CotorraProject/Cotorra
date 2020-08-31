using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.DTO
{
     
    [DataContract]
    public class AreaDTO
    {
        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public string Name { get; set; } 


    }
}
