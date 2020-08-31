
using System.Runtime.Serialization;

namespace Cotorra.Schema.DTO.Catalogs
{
    [DataContract]
    public class EmployeeBenefitsDTO
    { 
        [DataMember]

        public string VacationsPerSeniority { get; set; }
        [DataMember]

        public string VacationalBonusPerSeniority { get; set; }
        [DataMember]

        public string YearlyBonusPerSeniority { get; set; }
        [DataMember]

        public string PendingVacationDays { get; set; }
         
    }
}
