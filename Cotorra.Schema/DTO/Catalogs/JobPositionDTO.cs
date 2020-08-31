using System; 
using System.Runtime.Serialization; 

namespace Cotorra.Schema.DTO
{
     
    [DataContract]
    public class JobPositionDTO
    {
        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public string Name { get; set; } 


    }
}
