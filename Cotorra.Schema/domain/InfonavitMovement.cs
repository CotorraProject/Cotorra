using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class InfonavitMovement : IdentityCatalogEntityExt
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
        /// Relación a los conceptos del empleado de seguro de infonavit
        /// </summary>
        [DataMember]
        public Guid? EmployeeConceptsRelationInsuranceID { get; set; }

        //40 length
        [DataMember]
        public string CreditNumber { get; set; }

        [DataMember]
        public InfonavitCreditType InfonavitCreditType { get; set; }
    
        [DataMember]
        public DateTime InitialApplicationDate { get; set; }

        [DataMember]
        public DateTime RegisterDate { get; set; }

        /// <summary>
        /// 999999999
        /// </summary>
        [DataMember]
        public decimal MonthlyFactor { get; set; }

        [DataMember]
        public bool IncludeInsurancePayment_D14 { get; set; }

        /// <summary>
        /// 9999999
        /// </summary>
        [DataMember]
        public decimal AccumulatedAmount { get; set; }

        /// <summary>
        /// 9999
        /// </summary>
        [DataMember]
        public int AppliedTimes { get; set; }

        [DataMember]
        public bool InfonavitStatus { get; set; }

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
        public virtual EmployeeConceptsRelation EmployeeConceptsRelation { get; set; }

        /// <summary>
        /// Relación de conceptos de empleados del seguro de Infonavit (Virtual)
        /// </summary>
        public virtual EmployeeConceptsRelation EmployeeConceptsRelationInsurance { get; set; }
    }
}
