using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.nom035
{
    [DataContract]
    [Serializable]
    public class NOMEvaluationQuestion : CatalogEntity
    {
        [DataMember]
        public Guid NOMEvaluationPhaseID { get; set; }
        [DataMember]
        public Guid? NOMEvaluationCategoryID { get; set; }
        [DataMember]
        public Guid? NOMEvaluationDomainID { get; set; }
        [DataMember]
        public int Number { get; set; }

        [DataMember]
        public virtual NOMEvaluationPhase NOMEvaluationPhase { get; set;}

        [DataMember]
        public virtual NOMEvaluationCategory NOMEvaluationCategory { get; set; }

        [DataMember]
        public virtual NOMEvaluationDomain NOMEvaluationDomain { get; set; }
    }
}