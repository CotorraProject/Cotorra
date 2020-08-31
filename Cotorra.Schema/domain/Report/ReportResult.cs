using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;

namespace Cotorra.Schema.Report
{
    [DataContract]
    public class ReportResult
    {
        public ReportResult()
        {
            this.ReportResultDetails = new List<ReportResultDetail>();
        }

        [DataMember]
        public List<ReportResultDetail> ReportResultDetails { get; set; }
    }

    [DataContract]
    public class ReportResultDetail
    {
        public ReportResultDetail()
        { }

        public ReportResultDetail(string datasetName, string jsonResult)
        {
            DataSetName = datasetName;
            JsonResult = jsonResult;
        }

        [DataMember]
        public string DataSetName { get; set; }

        [DataMember]
        public string JsonResult { get; set; }
    }
}
