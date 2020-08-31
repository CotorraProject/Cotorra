using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class Overdraft : IdentityCatalogEntityExt, ICloneable
    {
        public Overdraft()
        {
            this.OverdraftDetails = new List<OverdraftDetail>();
            this.OverdraftType = OverdraftType.Ordinary;
        }

        [DataMember]
        public Guid PeriodDetailID { get; set; }

        [DataMember]
        public Guid EmployeeID { get; set; }

        [DataMember]
        public OverdraftType OverdraftType { get; set; }

        [DataMember]
        public Guid UUID { get; set; }      

        [DataMember]
        public decimal WorkingDays { get; set; }

        [DataMember]
        public OverdraftStatus OverdraftStatus { get; set; }

        [DataMember]
        public Guid? HistoricEmployeeID { get; set; }

        [DataMember]
        public Guid? OverdraftPreviousCancelRelationshipID { get; set; }

        [DataMember]
        public virtual List<OverdraftDetail> OverdraftDetails { get; set; }

        [DataMember]
        public virtual Employee Employee { get; set; }

        [DataMember]
        public virtual PeriodDetail PeriodDetail { get; set; }

        [DataMember]
        public virtual HistoricEmployee HistoricEmployee { get; set; }

        [DataMember]
        public virtual Overdraft OverdraftPreviousCancelRelationship { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
