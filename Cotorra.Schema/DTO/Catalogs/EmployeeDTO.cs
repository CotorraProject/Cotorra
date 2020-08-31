using System;
using System.Runtime.Serialization;

namespace Cotorra.Schema.DTO.Catalogs
{
    [DataContract]
    public class EmployeeDTO
    {
        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public Guid company { get; set; }

        [DataMember]
        public Guid InstanceID { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string NSS { get; set; }

        [DataMember]
        public string RFC { get; set; }

        [DataMember]
        public string CURP { get; set; }

        [DataMember]
        public DateTime EntryDate { get; set; }

        [DataMember]
        public string FullName { get; set; }

        [DataMember]
        public DepartmentDTO DepartmentDTO { get; set; }

        [DataMember]
        public JobPositionDTO JobPositionDTO { get; set; }


        [DataMember]
        public string Antiquity { get; set; }
    }
}
