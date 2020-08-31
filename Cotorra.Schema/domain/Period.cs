using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    [Table("Period")]
    public class Period : IdentityCatalogEntityExt
    { 
        [DataMember]
        public Int32 FiscalYear { get; set; }

        [DataMember]
        public DateTime InitialDate { get; set; }

        [DataMember]
        public DateTime FinalDate { get; set; }

        [DataMember]
        public bool IsFiscalYearClosed { get; set; }

        [DataMember]
        public bool IsActualFiscalYear { get; set; }

        [DataMember]
        public Guid PeriodTypeID { get; set; }

        [DataMember]
        public virtual List<PeriodDetail> PeriodDetails { get; set; }

        //PeriodType Copy Begin -------------------------------------------------------------------------
        /// <summary>
        /// Dias del periodo
        /// </summary>
        [DataMember]
        public Int32 PeriodTotalDays { get; set; }

        /// <summary>
        /// Días de pago
        /// </summary>
        [DataMember]
        public decimal PaymentDays { get; set; }

        /// <summary>
        /// Si es periodo extraordinario
        /// </summary>
        [DataMember]
        public bool ExtraordinaryPeriod { get; set; }

        //Ajustar al mes calendario
        [DataMember]
        public bool MonthCalendarFixed { get; set; }

        /// <summary>
        /// Días pagados en quincena
        /// </summary>
        [DataMember]
        public AdjustmentPay_16Days_Febrary FortnightPaymentDays { get; set; }

        [DataMember]
        public Int32 PaymentDayPosition { get; set; }

        [DataMember]
        public PaymentPeriodicity PaymentPeriodicity { get; set; }

        [DataMember]
        [NotMapped]
        public string SeventhDayPosition { get; set; }

        [DataMember]
        [NotMapped]
        public int SeventhDays { get; set; }

        //End of Copy  -----------------------------------------------------------

        [DataMember] 
        [JsonIgnore]
        [XmlIgnore]
        public virtual PeriodType PeriodType { get; set; }

    }
}
