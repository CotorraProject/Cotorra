using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.Report
{
    [DataContract]
    public class ReportManagerParams
    {
        [DataMember]
        public ReportCatalog ReportCatalog { get; set; }

        [DataMember]
        public IReportParams ReportParams { get; set; }
    }
}
