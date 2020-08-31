using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{

    [DataContract]
    [Serializable]
    public class Vacation : IdentityCatalogEntityExt
    {
        public Vacation()
        {
            VacationDaysOff = new List<VacationDaysOff>();
        }

        [DataMember]
        public Guid EmployeeID { get; set; }

        [DataMember]
        public VacationsCaptureType VacationsCaptureType { get; set; }

        [DataMember]
        public DateTime InitialDate { get; set; }

        [DataMember]
        public DateTime FinalDate { get; set; }

        [DataMember]
        public DateTime PaymentDate { get; set; }

        [DataMember]
        public decimal Break_Seventh_Days { get; set; }

        [DataMember]
        public decimal VacationsDays { get; set; }

        [DataMember]
        public decimal VacationsBonusDays { get; set; }

        [DataMember]
        public decimal VacationsBonusPercentage { get; set; }

        [DataMember]
        [NotMapped]
        public virtual Employee Employee { get; set; }

        [NotMapped]
        public string DaysOff { get; set; }

        [DataMember]
        public virtual List<VacationDaysOff> VacationDaysOff { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

    }
}