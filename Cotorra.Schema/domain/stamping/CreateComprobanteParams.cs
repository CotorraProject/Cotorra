using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class CreateComprobanteParams
    {
        [DataMember]
        public PayrollStampingDetail PayrollStampingDetail { get; set; }
        [DataMember]
        public PayrollStampingParams PayrollStampingParams { get; set; }
        [DataMember]
        public Overdraft Overdraft { get; set; }
        [DataMember]
        public OverdraftTotalsResult OverdraftResults { get; set; }
        [DataMember]
        public PayrollCompanyConfiguration PayrollCompanyConfiguration { get; set; }
        [DataMember]
        public DateTime CFDIDateTimeStamp { get; set; }
        [DataMember]
        public string ZipCode { get; set; }
        [DataMember]
        public IRoundUtil RoundUtil { get; set; }
        [DataMember]
        public List<Incident> Incidents { get; set; } 
        [DataMember]
        public List<Inhability> Inhabilities { get; set; }
    }
}
