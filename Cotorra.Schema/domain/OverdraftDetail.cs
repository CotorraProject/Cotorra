using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class OverdraftDetail : IdentityCatalogEntityExt, ICloneable
    {
        [DataMember]
        public Guid OverdraftID { get; set; }

        [DataMember]
        public Guid ConceptPaymentID { get; set; }

        /// <summary>
        /// Valor, Días, "Leyenda del valor" en conceptos
        /// </summary>
        [DataMember]
        public decimal Value { get; set; }

        /// <summary>
        /// Total
        /// </summary>
        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public string Label1 { get; set; }

        [DataMember]
        public string Label2 { get; set; }

        [DataMember]
        public string Label3 { get; set; }

        [DataMember]
        public string Label4 { get; set; }

        [DataMember]
        public decimal Taxed { get; set; }

        [DataMember]
        public decimal Exempt { get; set; }

        [DataMember]
        public decimal IMSSTaxed { get; set; }

        [DataMember]
        public decimal IMSSExempt { get; set; }

        [DataMember]
        public bool IsGeneratedByPermanentMovement { get; set; }

        [DataMember]
        public bool IsValueCapturedByUser { get; set; }

        [DataMember]
        public bool IsTotalAmountCapturedByUser { get; set; }

        [DataMember]
        public bool IsAmount1CapturedByUser { get; set; }

        [DataMember]
        public bool IsAmount2CapturedByUser { get; set; }

        [DataMember]
        public bool IsAmount3CapturedByUser { get; set; }

        [DataMember]
        public bool IsAmount4CapturedByUser { get; set; }

        [DataMember]
        [JsonIgnore]
        public virtual Overdraft Overdraft { get; set; }

        [DataMember]
        [JsonIgnore]
        public virtual ConceptPayment ConceptPayment { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
