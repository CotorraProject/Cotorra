using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cotorra.Schema
{
    /// <summary>
    /// Tipo de código del Empleado
    /// </summary>
    public enum CodeTypeEmployee
    {
        /// <summary>
        /// Numérico
        /// </summary>
        [Display(Name = "1", Description = "Numérico")]
        Numeric = 1
    }
}
