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
    public class NOMSurveyDomainResult : IdentityCatalogEntityExt
    {
        [DataMember]
        public Guid NOMSurveyReplyID { get; set; }
 
        [DataMember]
        public Guid NOMEvaluationDomainID { get; set; }

        [DataMember]
        public NOMSurveyReplyResultType ResultType { get; set; }

        [DataMember]
        public int Result { get; set; }
 
    }
}
