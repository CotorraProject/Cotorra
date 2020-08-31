using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public enum InfonavitCreditType
    { 
        /// <summary>
        /// Por porcentaje
        /// </summary>
        Percentage_D59 = 2,
        /// <summary>
        /// Por Factor de descuento
        /// </summary>
        DiscountFactor_D15 = 3,
        /// <summary>
        /// Por cuota fija
        /// </summary>
        FixQuota_D16 = 4,
        /// <summary>
        /// Seguro de vivienda Infonavit
        /// </summary>
        HomeInsurance_D14 = 5, 
    }
}
