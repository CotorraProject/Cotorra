using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    public enum CivilStatus
    {
        Soltero = 1,
        Casado = 2,
        UnionLibre = 3,
        Divorciado = 4,
        Viudo = 5
    }

    [DataContract]
    public enum Gender
    {
        Male = 1,
        Female = 2
    }

    [DataContract]
    [Serializable]
    [Table("Employee")]
    public partial class Employee : StatusIdentityCatalogEntityExt, IStatusFull
    {
        #region "Constructor"
        /// <summary>
        /// Constructor
        /// </summary>
        public Employee()
        {

        }
        #endregion       

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public Guid DepartmentID { get; set; }

        [DataMember]
        public Guid JobPositionID { get; set; }

        [DataMember]
        public Guid WorkshiftID { get; set; }

        [DataMember]
        public Guid PeriodTypeID { get; set; }


        [DataMember]
        public Guid? BankID { get; set; }

        [DataMember]
        public string FirstLastName { get; set; }

        [DataMember]
        public string SecondLastName { get; set; }

        [DataMember]
        [NotMapped]
        public string FullName { get { return $"{Name} {FirstLastName} {SecondLastName}"; } }

        [DataMember]
        public DateTime? BirthDate { get; set; }

        [DataMember]
        public string BornPlace { get; set; }

        [DataMember]
        public CivilStatus CivilStatus { get; set; }

        [DataMember]
        public Gender Gender { get; set; }

        [DataMember]
        public string NSS { get; set; }

        [DataMember]
        public string RFC { get; set; }

        [DataMember]
        public string CURP { get; set; }

        [DataMember]
        public DateTime EntryDate { get; set; }

        [DataMember]
        public DateTime? ReEntryDate { get; set; }
       

        [DataMember]
        public ContractType ContractType { get; set; }

        [DataMember]
        public decimal DailySalary { get; set; }

        [DataMember]
        public BaseQuotation ContributionBase { get; set; }

        [DataMember]
        public decimal SBCFixedPart { get; set; }

        [DataMember]
        public decimal SBCVariablePart { get; set; }

        [DataMember]
        public decimal SettlementSalaryBase { get; set; }

        [DataMember]
        public EmployeeTrustLevel EmployeeTrustLevel { get; set; }


        [DataMember]
        public decimal SBCMax25UMA { get; set; }

        [DataMember]
        public PaymentBase PaymentBase { get; set; }

        [DataMember]
        public PaymentMethod PaymentMethod { get; set; }

        [DataMember]
        public SalaryZone SalaryZone { get; set; }

        [DataMember]
        public EmployeeRegimeType RegimeType { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string UMF { get; set; }
        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public string BankAccount { get; set; }

        [DataMember]
        public Guid? EmployerRegistrationID { get; set; }

        public String ZipCode { get; set; }
        [DataMember]
        public String FederalEntity { get; set; }
        [DataMember]
        public String Municipality { get; set; }
        [DataMember]
        public String Street { get; set; }
        [DataMember]
        public String ExteriorNumber { get; set; }
        [DataMember]
        public String InteriorNumber { get; set; }
        [DataMember]
        public String Suburb { get; set; }
        [DataMember]
        public String Reference { get; set; }

        [DataMember]
        public string CLABE { get; set; }
        [DataMember]
        public string BankBranchNumber { get; set; }

        ///// <summary>
        ///// User identity to use the App and other stuffs
        ///// </summary>
        [DataMember]
        public Guid? IdentityUserID { get; set; }

        /// <summary>
        /// Jefe directo
        /// </summary>
        [DataMember]
        public Guid? ImmediateLeaderEmployeeID { get; set; }

        [DataMember]
        public string Cellphone { get; set; }

        [DataMember]
        public bool IsKioskEnabled { get; set; }

        [DataMember]
        public virtual Department Department { get; set; }

        [DataMember]
        public virtual JobPosition JobPosition { get; set; }

        [DataMember]
        public virtual Workshift Workshift { get; set; }

        [DataMember]
        public virtual EmployerRegistration EmployerRegistration { get; set; }

        [DataMember]
        public virtual PeriodType PeriodType { get; set; }

        [DataMember]
        public virtual Bank Bank { get; set; }


        [DataMember]
        public BenefitTypeValue BenefitType { get; set; }        


        [DataMember]
        public virtual List<Vacation> Vacations { get; set; }
       
        [DataMember]
        public DateTime? UnregisteredDate { get; set; }

        [DataMember]
        public virtual List<HistoricEmployeeSalaryAdjustment> HistoricEmployeeSalaryAdjustments { get; set; }

        [DataMember]
        public virtual List<HistoricEmployeeSBCAdjustment> HistoricEmployeeSBCAdjustments { get; set; }
    }
}
