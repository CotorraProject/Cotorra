using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    /// <summary>
    /// Tipo de Retención
    /// </summary>
    public enum RetentionType
    {
        /// <summary>
        /// Importe fijo
        /// </summary>
        FixedAmount = 1,
        /// <summary>
        /// Proporción a días pagados
        /// </summary>
        ProportionDaysPayed = 2
    }
}
