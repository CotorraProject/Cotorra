using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Cotorra.Schema
{
    public enum PeriodMonth
    {
        None = 0,
        Initial = 1,
        Final = 2,
        Both = 3
    }

    public enum PeriodFiscalYear
    {
        None = 0,
        Initial = 1,
        Final = 2
    }

    public enum PeriodBimonthlyIMSS
    {
        None = 0,
        Initial = 1,
        Final = 2
    }

    public enum PeriodStatus
    {
        Open = 0,
        Calculating = 1,
        Authorized = 2,
        Stamped = 3,
        NA = 99
    }

    [DataContract]
    [Serializable]
    [Table("PeriodDetail")]
    public class PeriodDetail : IdentityCatalogEntityExt
    {
        [DataMember]
        public int Number { get; set; }

        [NotMapped]
        [DataMember]
        public int Month { get => FinalDate.Month; }

        [DataMember]
        public DateTime InitialDate { get; set; }

        [DataMember]
        public DateTime FinalDate { get; set; }

        [DataMember]
        public Guid PeriodID { get; set; }

        [DataMember]
        public PeriodMonth PeriodMonth { get; set; }

        [DataMember]
        public PeriodBimonthlyIMSS PeriodBimonthlyIMSS { get; set; }

        [DataMember]
        public PeriodFiscalYear PeriodFiscalYear { get; set; }

        [DataMember]
        public decimal PaymentDays { get; set; }

        [DataMember]
        public PeriodStatus PeriodStatus { get; set; }

        [DataMember] 
        public string SeventhDayPosition { get; set; }

        [DataMember]
        public int SeventhDays { get; set; }

        /// <summary>
        /// Period Relationship
        /// </summary>
        [DataMember]
        [JsonIgnore]
        [XmlIgnore]
        public virtual Period Period { get; set; }
    }
}
