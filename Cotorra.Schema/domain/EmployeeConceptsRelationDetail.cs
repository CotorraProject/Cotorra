using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public enum ConceptsRelationPaymentStatus
    { 
        Pending = 0,
        Applied = 1
    }

    [DataContract]
    public class EmployeeConceptsRelationDetail : IdentityCatalogEntityExt
    {
        /// <summary>
        /// Relación a los conceptos del empleado
        /// </summary>
        [DataMember]
        public Guid EmployeeConceptsRelationID { get; set; }

        /// <summary>
        /// Fecha de pago
        /// </summary>
        [DataMember]
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// Recibo donde se aplicó el pago
        /// </summary>
        [DataMember]
        public Guid OverdraftID { get; set; }

        /// <summary>
        /// Valor aplicado
        /// </summary>
        [DataMember]
        public decimal ValueApplied { get; set; }

        /// <summary>
        /// Monto aplicado
        /// </summary>
        [DataMember]
        public decimal AmountApplied { get; set; }

        /// <summary>
        /// El monto es capturado por el usuario
        /// </summary>
        [DataMember]
        public bool IsAmountAppliedCapturedByUser { get; set; }

        /// <summary>
        /// Status del pago
        /// </summary>
        [DataMember]
        public ConceptsRelationPaymentStatus ConceptsRelationPaymentStatus { get; set; }

        /// <summary>
        /// Relacion de conceptos del empleado (Virtual)
        /// </summary>
        public virtual EmployeeConceptsRelation EmployeeConceptsRelation { get; set; }
    }
}
