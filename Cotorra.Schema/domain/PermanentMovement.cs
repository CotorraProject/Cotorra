using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class PermanentMovement : IdentityCatalogEntityExt
    {
        [DataMember]
        public Guid ConceptPaymentID { get; set; }

        [DataMember]
        public Guid EmployeeID { get; set; }

        [DataMember]
        public DateTime InitialApplicationDate { get; set; }

        [DataMember]
        public PermanentMovementType PermanentMovementType { get; set; }

        [DataMember]
        public PermanentMovementStatus PermanentMovementStatus { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public int TimesToApply { get; set; }

        [DataMember]
        public int TimesApplied { get; set; }

        [DataMember]
        public decimal LimitAmount { get; set; }

        [DataMember]
        public decimal AccumulatedAmount { get; set; }

        [DataMember]
        public DateTime RegistryDate { get; set; }

        [DataMember]
        public int ControlNumber { get; set; }

        [DataMember]
        public virtual ConceptPayment ConceptPayment { get; set; }

        [DataMember]
        [NotMapped]
        public virtual Employee Employee { get; set; }
    }
}