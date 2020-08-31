using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class OverdraftTotalsResult
    {
        [DataMember]
        public decimal FixAmount { get; set; }

        [DataMember]
        public decimal WorkingDays { get; set; }

        /// <summary>
        /// Ajuste al neto
        /// </summary>
        [DataMember]
        public decimal AdjustmentAmount { get; set; }

        [DataMember]
        public decimal TotalSalaryPayments { get; set; }

        [DataMember]
        public decimal TotalLiabilityPayments { get; set; }

        [DataMember]
        public decimal TotalDeductionPayments { get; set; }

        [DataMember]
        public decimal TotalDeductionPaymentsISR { get; set; }

        [DataMember]
        public decimal TotalOtherDeductions { get; set; }

        [DataMember]
        public decimal Total { get; set; }

        [DataMember]
        public decimal TotalOtherPayments { get; set; }

        /// <summary>
        /// TotalSeparacionIndemnizacion
        /// </summary>
        [DataMember]
        public decimal TotalSeparationCompensation { get; set; }

        /// <summary>
        /// TotalJubilcacionPensionRetiro
        /// </summary>
        [DataMember]
        public decimal TotalRetirementPensionWithdrawal { get; set; }

        /// <summary>
        /// TotalGravado
        /// </summary>
        [DataMember]
        public decimal TotalTaxed { get; set; }

        /// <summary>
        /// Total Gravado del Finiquito
        /// </summary>
        [DataMember]
        public decimal TotalTaxedSettlement { get; set; }

        /// <summary>
        /// Total exento
        /// </summary>
        [DataMember]
        public decimal TotalExempt { get; set; }

        /// <summary>
        /// Total Exento del Finiquito
        /// </summary>
        [DataMember]
        public decimal TotalExemptSettlement { get; set; }

        /// <summary>
        /// Total salay totals
        /// </summary>
        [DataMember]
        public decimal TotalSalaryTotals { get; set; }
    }
}
