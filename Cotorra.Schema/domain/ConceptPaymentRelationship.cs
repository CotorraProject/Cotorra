using System;
using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class ConceptPaymentRelationship : IdentityCatalogEntityExt
    {
        [DataMember]
        public Guid ConceptPaymentID { get; set; }

        [DataMember]
        public Guid AccumulatedTypeID { get; set; }

        [DataMember]
        public ConceptPaymentType ConceptPaymentType { get; set; }

        [DataMember]
        public ConceptPaymentRelationshipType ConceptPaymentRelationshipType { get; set; }

        [DataMember]
        public virtual ConceptPayment ConceptPayment { get; set; }

        [DataMember]
        public virtual AccumulatedType AccumulatedType { get; set; }
    }
}
