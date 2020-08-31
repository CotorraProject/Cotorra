using CotorraNode.Common.Base.Schema.Parameters.Base;
using CotorraNube.CommonApp.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class UpdateAmountConceptDetailParams : IInstanceIDParams
    {
        [DataMember]
        public Guid OverdraftDetailID { get; set; }
        [DataMember]
        public decimal Amount { get; set; }
        [DataMember]
        public List<ConceptRelationUpdateAmountDTO> ConceptRelationsAmountApplied { get; set; }       

    }

   
}
