using CotorraNode.Common.Base.Schema;
using Cotorra.Schema.domain.nom035;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.nom035
{
    [DataContract]
    [Serializable]
    public class NOMAnswer : IdentityCatalogEntityExt
    {
        [DataMember]
        public Guid NOMSurveyReplyID { get; set; }

        [DataMember]
        public Guid NOMEvaluationQuestionID { get; set; } 

        [DataMember]
        public int Value { get; set; }

        [DataMember]
        public virtual NOMSurveyReply NOMSurveyReply { get; set; }

        [DataMember]
        public virtual NOMEvaluationQuestion NOMEvaluationQuestion { get; set; }

    }
}
