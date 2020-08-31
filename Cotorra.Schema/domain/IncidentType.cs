using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public enum TypeOfIncident
    {
        Piecework = 1, //Destajos
        Days = 2, //Días
        Hours = 3 //Horas
    }

    [DataContract]
    public enum ItConsiders
    {
        /// <summary>
        /// Ninguno
        /// </summary>
        [DisplayAttribute(Name="1", Description = "Ninguno")]
        None = 1, //Ninguno
        /// <summary>
        /// Ausencia
        /// </summary>
        [DisplayAttribute(Name = "2", Description = "Ausencia")]
        Absence = 2, //Ausencia
        /// <summary>
        /// Incapacidad
        /// </summary>
        [DisplayAttribute(Name = "3", Description = "Incapacidad")]
        Inhability = 3, //Incapacidad
        /// <summary>
        /// Vacaciones
        /// </summary>
        [DisplayAttribute(Name = "4", Description = "Vacaciones")]
        Holidays = 4 //Vacaciones
    }

    [DataContract]
    [Serializable]
    public class IncidentType : IdentityCatalogEntityExt
    {
        public IncidentType()
        {
            AccumulatedTypes = new List<AccumulatedType>();
        }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public TypeOfIncident TypeOfIncident { get; set; }

        [DataMember]
        public decimal UnitValue { get; set; }

        [DataMember]
        public decimal Percentage { get; set; }

        [DataMember]
        public ItConsiders ItConsiders { get; set; }

        [DataMember]
        public bool SalaryRight { get; set; }

        [DataMember]
        public bool DecreasesSeventhDay { get; set; }

        [NotMapped]
        [DataMember]
        public virtual List<AccumulatedType> AccumulatedTypes { get; set; }

    }
}
