using CotorraNode.Common.Base.Schema.Parameters.Base;
using CotorraNube.CommonApp.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class CalculateSettlementProcessParams : IInstanceIDParams
    {
        [DataMember]
        public Guid EmployeeID { get; set; }
        [DataMember]
        public Guid PeriodDetailID { get; set; }
        [DataMember]
        public decimal SettlementBaseSalary { get; set; }
        [DataMember]
        public DateTime SettlementEmployeeSeparationDate { get; set; }
        [DataMember]
        public int SettlementCause { get; set; }
        [DataMember]
        public double CompleteISRYears { get; set; }
        [DataMember]
        public List<ConceptsToApply> ConceptsToApply { get; set; }
        [DataMember]
        public Guid user { get; set; }

    }

    [DataContract]
    [Serializable]
    public class ConceptsToApply
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public int Code { get; set; }
        [DataMember]
        public decimal TotalDays { get; set; }

        [DataMember]
        public bool Apply { get; set; }
    }
}
