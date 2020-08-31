using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    /// <summary>
    /// Tipos de prestaciones
    /// </summary>
    [DataContract]
    [Serializable]
    public class BenefitType : IdentityCatalogEntityExt
    {

        /// <summary>
        /// Antigüedad
        /// </summary>
        [DataMember]
        public int Antiquity { get; set; }

        /// <summary>
        /// Días de vacaciones
        /// </summary>
        [DataMember]
        public decimal Holidays { get; set; }

        /// <summary>
        /// Porc. prima vacacional
        /// </summary>
        [DataMember]
        public decimal HolidayPremiumPortion { get; set; }

        /// <summary>
        /// Días de aguinaldo
        /// </summary>
        [DataMember]
        public decimal DaysOfChristmasBonus{ get; set; }

        /// <summary>
        /// Factor de integración
        /// </summary>
        [DataMember]
        public decimal IntegrationFactor { get; set; }

        /// <summary>
        /// Días Antigüedad
        /// </summary>
        [DataMember]
        public decimal DaysOfAntiquity { get; set; }

    }
}
