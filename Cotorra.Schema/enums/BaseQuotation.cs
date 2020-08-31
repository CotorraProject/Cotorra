using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cotorra.Schema
{
    /// <summary>
    /// Base de cotización
    /// </summary>
    public enum BaseQuotation
    {
        /// <summary>
        /// Fijo
        /// </summary>
        [Display(Name = "1", Description = "Fijo")]
        Fixed = 1,

        /// <summary>
        /// Variable
        /// </summary>
        [Display(Name = "2", Description = "Variable")]
        Variable = 2,

        /// <summary>
        /// Mixto
        /// </summary>
        [Display(Name = "3", Description = "Mixto")]
        Mixed = 3,
    }
}
