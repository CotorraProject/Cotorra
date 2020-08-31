using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class CancelPayrollStampingResult
    {
        public CancelPayrollStampingResult()
        {
            CancelPayrollStampingResultDetails = new List<CancelPayrollStampingResultDetail>();
        }

        [DataMember]
        public string CancelacionXMLRequest { get; set; }

        [DataMember]
        public string CancelacionXMLAcknowledgeResponse { get; set; }

        [DataMember]
        public bool WithErrors { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public Guid InstanceID { get; set; }

        [DataMember]
        public List<CancelPayrollStampingResultDetail> CancelPayrollStampingResultDetails { get; set; }
    }

    [DataContract]
    public class CancelPayrollStampingResultDetail
    {
        [DataMember]
        public Guid OverdraftID { get; set; }

        [DataMember]
        public Guid UUID { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string CancelationAcknowledgmentReceipt { get; set; }

        [DataMember]
        public PayrollStampingResultStatus PayrollStampingResultStatus { get; set; }
    }
}
