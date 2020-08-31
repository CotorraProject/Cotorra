using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class IMSSFare : IdentityCatalogEntityExt
    {
        [DataMember]
        public String IMMSBranch { get; set; }
        [DataMember]
        public Decimal EmployerShare { get; set; }
        [DataMember]
        public Decimal EmployeeShare { get; set; }
        [DataMember]
        public Int32 MaxSMDF { get; set; }
    }
}
