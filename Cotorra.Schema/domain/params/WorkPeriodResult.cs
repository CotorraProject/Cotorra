using CotorraNode.Common.Library.Private;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class WorkPeriodResult
    {
        [DataMember]
        public Guid OverdraftID { get; set; }

        [DataMember]
        public OverdraftStatus OverdraftStatus { get; set; }

        [DataMember]
        public OverdraftType OverdraftType { get; set; }

        [DataMember]
        public Guid EmployeeID { get; set; }

        [DataMember]
        public Decimal TotalPerceptions { get; set; }

        [DataMember]
        public Decimal TotalDeductions { get; set; }

        [DataMember]
        public Decimal TotalLiabilities { get; set; }

        [DataMember]
        public Guid? UUID { get; set; }
    }
}
