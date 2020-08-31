using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class MonthlyIncomeTax : IdentityCatalogEntityExt
    {
        [DataMember]
        public DateTime ValidityDate { get; set; }

        /// <summary>
        /// Limite inferior
        /// </summary>
        [DataMember]
        public Decimal LowerLimit { get; set; }

        /// <summary>
        /// Cuota Fija
        /// </summary>
        [DataMember]
        public Decimal FixedFee { get; set; }

        /// <summary>
        /// Porcetaje
        /// </summary>
        [DataMember]
        public Decimal Rate { get; set; }

    }
}
