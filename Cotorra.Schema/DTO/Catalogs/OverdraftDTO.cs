using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.DTO.Catalogs
{
    [DataContract]
    public class OverdraftDTO
    {
        public OverdraftDTO()
        {
            Perceptions = new List<PerceptionDTO>();
            Deductions = new List<DeductionDTO>();
        }

        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public Guid InstanceID { get; set; }

        [DataMember]
        public Guid PeriodDetailID { get; set; }

        [DataMember]
        public string PeriodTypeName { get; set; }

        [DataMember]
        public DateTime InitialDate { get; set; }

        [DataMember]
        public DateTime FinalDate { get; set; }

        [DataMember]
        public decimal TotalPerceptions { get; set; }

        [DataMember]
        public decimal TotalDeductions { get; set; }

        [DataMember]
        public decimal Total { get; set; }

        [DataMember]
        public Guid UUID { get; set; }

        [DataMember]
        public string XML { get; set; }

        [DataMember]
        public List<PerceptionDTO> Perceptions { get; set; }

        [DataMember]
        public List<DeductionDTO> Deductions { get; set; }
    }
}
