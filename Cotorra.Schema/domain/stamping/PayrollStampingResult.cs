using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    public enum PayrollStampingResultStatus
    { 
        Success = 1,
        Fail = 2
    }

    [DataContract]
    public class PayrollStampingResult
    {
        public PayrollStampingResult()
        {
            PayrollStampingResultDetails = new List<PayrollStampingResultDetail>();
        }

        [DataMember]
        public List<PayrollStampingResultDetail> PayrollStampingResultDetails { get; set; }
    }

    [DataContract]
    public class PayrollStampingResultDetail
    {
        [DataMember]
        public Guid OverdraftID { get; set; }

        [DataMember]
        public Overdraft Overdraft { get; set; }

        [DataMember]
        public Guid HistoricEmployeeID { get; set; }

        [DataMember]
        public Guid EmployeeID { get; set; }

        [DataMember]
        public Guid PeriodDetailID { get; set; }

        [DataMember]
        public PeriodDetail PeriodDetail { get; set; }

        [DataMember]
        public Guid UUID { get; set; }

        [DataMember]
        public string XML { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public PayrollStampingResultStatus PayrollStampingResultStatus { get; set; }
    }
}
