using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    /// <summary>
    /// Crédito Fonacot
    /// </summary>
    [DataContract]
    [Serializable]
    public class FonacotMovement : IdentityCatalogEntityExt
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
        /// Relación a los conceptos del empleado
        /// </summary>
        [DataMember]
        public Guid EmployeeConceptsRelationID { get; set; }

        /// <summary>
        /// No. crédito
        /// </summary>
        [DataMember]
        public string CreditNumber { get; set; }

        /// <summary>
        /// Mes
        /// </summary>
        [DataMember]
        public int Month { get; set; }

        /// <summary>
        /// Año
        /// </summary>
        [DataMember]
        public int Year { get; set; }

        /// <summary>
        /// Tipo de retención
        /// </summary>
        [DataMember]
        public RetentionType RetentionType { get; set; }

        /// <summary>
        /// Estado: Activo / Inactivo
        /// </summary>
        [DataMember]
        public FonacotMovementStatus FonacotMovementStatus { get; set; }

        /// <summary>
        /// Observaciones
        /// </summary>
        [DataMember]
        public string Observations { get; set; }

        /// <summary>
        /// Concepto asociado (Virtual)
        /// </summary>
        [DataMember]
        public virtual ConceptPayment ConceptPayment { get; set; }

        /// <summary>
        /// Empleado asociado (virtual)
        /// </summary>
        [DataMember]
        public virtual Employee Employee { get; set; }

        /// <summary>
        /// Relación de conceptos de empleados (Virtual)
        /// </summary>
        [DataMember]
        public virtual EmployeeConceptsRelation EmployeeConceptsRelation { get; set; }
    }
}
