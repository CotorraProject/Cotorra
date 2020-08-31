using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cotorra.Schema
{
    /// <summary>
    /// Ajuste de pago mensual
    /// </summary>
    public enum AdjustmentPay_16Days_Febrary
    {
        /// <summary>
        /// Pagar los días de pago
        /// </summary>
        [Display(Name = "1", Description = "Pagar los días de pago")]
        PayPaymentDays = 1,

        /// <summary>
        /// Pagar los días calendario del periodo
        /// </summary>
        [Display(Name = "2", Description = "Pagar los días calendario del periodo")]
        PayCalendarDays = 2
    }
}
