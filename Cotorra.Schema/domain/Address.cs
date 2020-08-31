using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class Address: IdentityCatalogEntityExt
    {
        /// <summary>
        /// Código Postal
        /// </summary>
        [DataMember]
        public String ZipCode { get; set; }

        /// <summary>
        /// Entidad Federativa
        /// </summary>
        [DataMember]
        public String FederalEntity { get; set; }

        /// <summary>
        /// Municipio
        /// </summary>
        [DataMember]
        public String Municipality { get; set; }

        /// <summary>
        /// Calle
        /// </summary>
        [DataMember]
        public String Street { get; set; }

        /// <summary>
        /// No. Exterior
        /// </summary>
        [DataMember]
        public String ExteriorNumber { get; set; }

        /// <summary>
        /// No. Interior
        /// </summary>
        [DataMember]
        public String InteriorNumber { get; set; }

        /// <summary>
        /// Colonia
        /// </summary>
        [DataMember]
        public String Suburb { get; set; }

        /// <summary>
        /// Referencia
        /// </summary>
        [DataMember]
        public String Reference { get; set; }
    }
}
