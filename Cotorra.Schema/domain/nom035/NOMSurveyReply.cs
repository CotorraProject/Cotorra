using CotorraNode.Common.Base.Schema;
using Cotorra.Schema.domain.nom035;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.nom035
{
    public enum NOMSurveyReplyResultType
    { 
       Numeric = 0,
       Percentage = 1
    }

    public enum EvaluationStateType
    { 
        New = 0,
        Sent = 1,
        Answered = 2
    }

    [DataContract]
    [Serializable]
    public class NOMSurveyReply : IdentityCatalogEntityExt
    {
        [DataMember]
        public Guid NOMEvaluationSurveyID { get; set; }

        [DataMember]
        public Guid NOMEvaluationPeriodID { get; set; }

        [DataMember]
        public Guid EmployeeID { get; set; }

        [DataMember]
        public NOMSurveyReplyResultType ResultType { get; set; }

        [DataMember]
        public int Result { get; set; }

        [DataMember] 
        public EvaluationStateType EvaluationState { get; set; } 

        [DataMember]
        public virtual List<NOMAnswer> NOMAnswer { get; set; }

        [DataMember]
        public virtual NOMEvaluationPeriod NOMEvaluationPeriod { get; set; }

        [DataMember]
        public virtual NOMEvaluationSurvey NOMEvaluationSurvey { get; set; }

        [DataMember]
        public virtual Employee Employee { get; set; }

        [NotMapped]
        public List<NOMSurveyCategoryResult> NOMSurveyCategoryResult { get; set; }
        [NotMapped]
        public List<NOMSurveyDomainResult> NOMSurveyDomainResult { get; set; }
    }
}
