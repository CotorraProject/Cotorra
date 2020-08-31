using CotorraNode.Common.Base.Schema.Parameters.Base;
using CotorraNube.CommonApp.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class ApplySettlementProcessParams : LicenseParams
    {
        [DataMember]
        public Guid EmployeeID { get; set; }
        [DataMember]
        public Guid PeriodDetailID { get; set; }
        [DataMember]
        public Guid user { get; set; }
        [DataMember]
        public DateTime SettlementEmployeeSeparationDate { get; set; }
        [DataMember]
        public bool ChangeOverdrafts { get; set; }
        [DataMember]
        public Guid OrdinaryID { get; set; }
        [DataMember]
        public Guid IndemnizationOverID { get; set; }

        [DataMember]
        public List<Guid> OrdinaryOverDetailsIDs { get; set; }

        [DataMember]
        public List<Guid> IndemnizationOverDetailsIDs { get; set; }
       

    }

   
}
