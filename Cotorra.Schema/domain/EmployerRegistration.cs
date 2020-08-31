using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class EmployerRegistration : IdentityCatalogEntityExt
    {
        /// <summary>
        /// Código del registro patronal
        /// </summary>
        [DataMember]
        public String Code { get; set; }

        /// <summary>
        /// Clase de riesgo de trabajo
        /// </summary>
        [DataMember]
        public String RiskClass { get; set; }

        /// <summary>
        /// Fracción de riesgo de trabajo
        /// </summary>
        [DataMember]
        public Decimal RiskClassFraction { get; set; }

        [DataMember]
        public Guid? AddressID { get; set; }
        
        [DataMember]
        public virtual Address? Address { get; set; }
    }
}
