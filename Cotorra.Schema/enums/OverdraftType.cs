using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public enum OverdraftType
    {
        /// <summary>
        /// Ordinario
        /// </summary>
        Ordinary = 1,
        /// <summary>
        /// Extraordinario
        /// </summary>
        Extraordinary = 2,
        /// <summary>
        /// Finiquito ordinario
        /// </summary>
        OrdinarySettlement = 3,
        /// <summary>
        /// Finiquito compensación / liquidación
        /// </summary>
        CompensationSettlement = 4
    }
}
