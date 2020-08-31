using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.nom035
{
    [DataContract]
    public class NOMSurveyReplyParams
    {
        [DataMember]
        public Guid IdentityWorkId { get; set; }
        [DataMember]
        public Guid InstanceId { get; set; }
        [DataMember]
        public Guid EmployeeId { get; set; }
        [DataMember]
        public Guid NOMEvaluationPeriodId { get; set; }
        [DataMember]
        public Guid NOMEvaluationSurveyId { get; set; }
    }
}
