using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public static class PaymentPeriodicityTraduction
    {
        private static Dictionary<PaymentPeriodicity, string> _dictionary = new Dictionary<PaymentPeriodicity, string>()
        {
            {PaymentPeriodicity.Biweekly, "Quincenal" },
            {PaymentPeriodicity.Weekly, "Semanal" },
            {PaymentPeriodicity.Monthly, "Mensual" },
            {PaymentPeriodicity.Fourteen, "Catorcenal" },
        };

        public static string GetTraduction(PaymentPeriodicity paymentPeriodicity)
        {
            _dictionary.TryGetValue(paymentPeriodicity, out string result);
            return result;
        }
    }

    public enum PaymentPeriodicity
    {
        /// <summary>
        /// Diario
        /// </summary>
        Daily = 1,
        /// <summary>
        /// Semanal
        /// </summary>
        Weekly = 2, 
        /// <summary>
        /// Catorcenal
        /// </summary>
        Fourteen = 3, 
        /// <summary>
        /// Quicenal
        /// </summary>
        Biweekly = 4, //Quincenal
        /// <summary>
        /// Mensual
        /// </summary>
        Monthly = 5, 
        /// <summary>
        /// Bimestral
        /// </summary>
        Bimonthly = 6, 
        /// <summary>
        /// Unidad de obra
        /// </summary>
        WorkUnit = 7, 
        /// <summary>
        /// Comisión
        /// </summary>
        Commission = 8, 
        /// <summary>
        /// Precio alzado
        /// </summary>
        ElevatedPrice = 9, 
        /// <summary>
        /// Decenal
        /// </summary>
        Decennial = 10, 
        /// <summary>
        /// Otra Periodicidad
        /// </summary>
        OtherPeriodicity = 99, 
    }
}
