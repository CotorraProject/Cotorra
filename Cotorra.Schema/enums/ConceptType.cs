using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    /// <summary>
    /// Tipo de Concepto
    /// </summary>
    [DataContract]
    public enum ConceptType
    {
        /// <summary>
        /// Percepción
        /// </summary>
        SalaryPayment = 1,

        /// <summary>
        /// Obligación
        /// </summary>
        LiabilityPayment = 2,

        /// <summary>
        /// Deducción
        /// </summary>
        DeductionPayment = 3
    }
}
