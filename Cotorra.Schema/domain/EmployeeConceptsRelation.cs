using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    /// <summary>
    /// ConceptPayment Status
    /// </summary>
    [DataContract]
    public enum ConceptPaymentStatus
    {
        Inactive = 0,
        Active = 1
    }

    [DataContract]
    public class EmployeeConceptsRelation : IdentityCatalogEntityExt
    {
        /// <summary>
        /// Empleado ID
        /// </summary>
        [DataMember]
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Concepto (Deducción 61)
        /// </summary>
        [DataMember]
        public Guid ConceptPaymentID { get; set; }

        /// <summary>
        /// Monto del crédito
        /// </summary>
        [DataMember]
        public decimal CreditAmount { get; set; }

        /// <summary>
        /// Valor --- Descuento / Incremento mensual/periodo
        /// </summary>
        [DataMember]
        public decimal OverdraftDetailValue { get; set; }

        /// <summary>
        /// Monto --- Descuento / Incremento mensual/periodo
        /// </summary>
        [DataMember]
        public decimal OverdraftDetailAmount { get; set; }

        /// <summary>
        /// Pagos hechos por fuera
        /// </summary>
        [DataMember]
        public decimal PaymentsMadeByOtherMethod { get; set; }

        /// <summary>
        /// Monto acumulado
        /// </summary>
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal AccumulatedAmountWithHeldCalculated { get; private set; }

        /// <summary>
        /// Saldo
        /// </summary>
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal BalanceCalculated { get; private set; }

        /// <summary>
        /// Status del concepto (activo / inactivo)
        /// </summary>
        [DataMember]
        public ConceptPaymentStatus ConceptPaymentStatus { get; set; }

        /// <summary>
        /// Fecha Inicial del crédito
        /// </summary>
        [DataMember]
        public DateTime InitialCreditDate { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual ConceptPayment ConceptPayment { get; set; }

        public virtual List<EmployeeConceptsRelationDetail> EmployeeConceptsRelationDetails { get; set; }
    }
}
