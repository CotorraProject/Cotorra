using System;
using System.Runtime.Serialization;

namespace Cotorra.Schema.DTO
{
     
    [DataContract]
    public class MonthlyIncomeTaxDTO
    {
        [DataMember]
        public Guid ID { get; set; }

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
