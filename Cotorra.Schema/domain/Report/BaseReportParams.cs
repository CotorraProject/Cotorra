using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.Report
{
    [DataContract]
    public class BaseReportParams : IReportParams
    {
        [DataMember]
        public Guid company { get; set; }
        [DataMember]
        public Guid InstanceID { get; set; }
        [DataMember]
        public Guid user { get; set; }
    }
}
