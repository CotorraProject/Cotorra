using Cotorra.Schema.nom035;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.nom035
{
    [DataContract]
    public class NOMSurveyReplyResult
    {
        [DataMember]
        public string Token { get; set; }
        [DataMember]
        public NOMSurveyReply NOMSurveyReply { get; set; }
    }
}
