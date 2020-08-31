using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class ConceptPayment : IdentityCatalogEntityExt, IConcept, ICloneable
    {
        [DataMember]
        public int Code { get; set; }

        [DataMember]
        public ConceptType ConceptType { get; set; }

        [DataMember]
        public bool GlobalAutomatic { get; set; }

        [DataMember]
        public bool AutomaticDismissal { get; set; }

        [DataMember]
        public bool Kind { get; set; }

        [DataMember]
        public bool Print { get; set; }

        [DataMember]
        public string SATGroupCode { get; set; }

        [DataMember]
        public string Label { get; set; }

        /// <summary>
        /// Formula de Total
        /// </summary>
        [DataMember]
        public string Formula { get; set; }

        /// <summary>
        /// Formula del Valor
        /// </summary>
        [DataMember]
        public string FormulaValue { get; set; }

        [DataMember]
        public string Label1 { get; set; }

        [DataMember]
        public string Label2 { get; set; }

        [DataMember]
        public string Label3 { get; set; }

        [DataMember]
        public string Label4 { get; set; }

        /// <summary>
        /// Formula de ISR Gravado
        /// </summary>
        [DataMember]
        public string Formula1 { get; set; }

        /// <summary>
        /// Formula de ISR Exento
        /// </summary>
        [DataMember]
        public string Formula2 { get; set; }

        /// <summary>
        /// Formula de IMSS Gravado
        /// </summary>
        [DataMember]
        public string Formula3 { get; set; }

        /// <summary>
        /// Formula de IMSS Exento
        /// </summary>
        [DataMember]
        public string Formula4 { get; set; }

        [DataMember]
        public virtual List<ConceptPaymentRelationship> ConceptPaymentRelationship { get; set; }
       

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
