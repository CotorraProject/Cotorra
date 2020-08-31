using System;
using System.Runtime.Serialization;

namespace Cotorra.Schema.Report
{
    [DataContract]
    public class FonacotReportParams : BaseReportParams
    {
        
        [DataMember]
        public int InitialFiscalYear { get; set; }


        [DataMember]
        public int FinalFiscalYear { get; set; }


        [DataMember]
        public int InitialMonth { get; set; }

        [DataMember]
        public int FinalMonth { get; set; }

        [DataMember]
        public Guid? EmployeeFilter { get; set; }

        [DataMember]
        public int CreditStatus { get; set; }

        [DataMember]
        public int EmployeeStatus { get; set; }

       
    }
}
