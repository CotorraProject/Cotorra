using System; 
using System.Runtime.Serialization; 

namespace Cotorra.Schema.DTO
{
    [DataContract]
    public class DepartmentDTO
    {
        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        
        [DataMember]
        public AreaDTO AreaDTO { get; set; }


    }

   
}
