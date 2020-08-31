using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    /// <summary>
    /// Categoría de Seguro
    /// </summary>
    [DataContract]
    public enum CategoryInsurance
    {
        /// <summary>
        /// Riesgo de Trabajo
        /// </summary>
        [Display(Name = "1", Description = "Riesgo de Trabajo")]
        WorkRisk = 1,

        /// <summary>
        /// Enfermedad general
        /// </summary>
        [Display(Name = "2", Description = "Enfermedad general")]
        GeneralDisease = 2,

        /// <summary>
        /// Maternidad pre-natal
        /// </summary>
        [Display(Name = "3", Description = "Maternidad")]
        PrenatalMaternity = 3,


    }
}
