using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    [Table("PeriodType")]
    public class PeriodType : IdentityCatalogEntityExt
    {
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
        public string SeventhDayPosition { get; set; }


        [DataMember]        
        public int SeventhDays { get; set; }

        [DataMember]
        public HolidayPaymentConfiguration HolidayPremiumPaymentType { get; set; }


        [DataMember]
        public virtual List<Period> Periods { get; set; }

    }
}
