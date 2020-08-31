using System;
using System.Runtime.Serialization;

namespace Cotorra.Schema.Report
{
    [DataContract]
    public class RayaReportParams : BaseReportParams
    {
        [DataMember]
        public Guid? EmployerRegistrationID { get; set; }

        [DataMember]
        public Guid PeriodDetailID { get; set; }

    }
}
