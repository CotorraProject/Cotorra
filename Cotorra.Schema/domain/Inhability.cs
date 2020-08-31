using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class Inhability : IdentityCatalogEntityExt
    {
        /// <summary>
        /// Description must have 40 max
        /// </summary>
        [DataMember]
        public Guid EmployeeID { get; set; }

        //9 caracteres alfanumerico
        [DataMember]
        public string Folio { get; set; }

        /// <summary>
        /// It considers = Inhability
        /// </summary>
        [DataMember]
        public Guid IncidentTypeID { get; set; }

        /// <summary>
        /// no mas de 90
        /// </summary>
        [DataMember]
        public int AuthorizedDays { get; set; }

        /// <summary>
        /// Initial Date (La fecha inicial de la tarjeta no puede ser menor a la fecha de inicio de periodo)
        /// </summary>
        [DataMember]
        public DateTime InitialDate { get; set; }

        [DataMember]
        [NotMapped]
        public DateTime FinalDate
        {
            get
            {
                var finalDate = this.InitialDate.AddDays(AuthorizedDays - 1);
                return finalDate;
            }
        }

        /// <summary>
        /// Category Insurance
        /// </summary>
        [DataMember]
        public CategoryInsurance CategoryInsurance { get; set; }

        /// <summary>
        /// RiskType
        /// </summary>
        [DataMember]
        public RiskType RiskType { get; set; }

        /// <summary>
        /// Percentage <100
        /// </summary>
        [DataMember]
        public decimal Percentage { get; set; }

        /// <summary>
        /// Consequence
        /// </summary>
        [DataMember]
        public Consequence Consequence { get; set; }

        /// <summary>
        /// InhabilityControl
        /// </summary>
        [DataMember]
        public InhabilityControl InhabilityControl { get; set; }

        /// <summary>
        /// Incident Type
        /// </summary>

        [DataMember]
        public virtual IncidentType IncidentType { get; set; }

        [DataMember]
        public virtual Employee Employee { get; set; }
    }
}