using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class Settlement  : IdentityCatalogEntityExt
    {

        [DataMember]
        public Guid EmployeeID { get; set; }

        [DataMember]
        public decimal SettlementBaseSalary { get; set; }

        [DataMember]
        public DateTime SettlementEmployeeSeparationDate { get; set; }

        [DataMember]
        public SettlementCause SettlementCause { get; set; }

        [DataMember]
        public decimal CompleteISRYears { get; set; }

        [DataMember]
        public bool ISRoSUBSDirectCalculus { get; set; }

        [DataMember]
        public bool ApplyEmployeeSubsidyInISRUSMOCalculus { get; set; }

    }
}
