using CotorraNode.Common.Base.Schema;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public partial class HistoricEmployee : IdentityCatalogEntityExt
    {
        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public Guid PeriodDetailID { get; set; }

        [DataMember]
        public Guid EmployeeID { get; set; }

        [DataMember]
        public string DepartmentDescription { get; set; }

        [DataMember]
        public Guid DepartmentID { get; set; }

        [DataMember]
        public string JobPositionDescription { get; set; }

        [DataMember]
        public Guid JobPositionID { get; set; }

        [DataMember]
        public string WorkshiftDescription { get; set; }

        [DataMember]
        public Guid WorkshiftID { get; set; }

        [DataMember]
        public string PeriodTypeDescription { get; set; }

        [DataMember]
        public Guid PeriodTypeID { get; set; }

        [DataMember]
        public string EmployerRegistrationDescription { get; set; }

        [DataMember]
        public string EmployerRegistrationZipCode { get; set; }

        [DataMember]
        public string EmployerRegistrationCode { get; set; }

        [DataMember]
        public string EmployerRegistrationFederalEntity { get; set; }

        [DataMember]
        public Guid? EmployerRegistrationID { get; set; }

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
        public decimal SBCMax25UMA { get; set; }

        [DataMember]
        public decimal SettlementSalaryBase { get; set; }

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
        public EmployeeTrustLevel EmployeeTrustLevel { get; set; }

        [DataMember]
        public JobPositionRiskType JobPositionRiskType { get; set; }

        [DataMember]
        public PaymentPeriodicity PaymentPeriodicity { get; set; }

        [DataMember]
        public ShiftWorkingDayType ShiftWorkingDayType { get; set; }

        [DataMember]
        public int? BankCode { get; set; }

        [DataMember]
        public BenefitTypeValue BenefitType { get; set; }

        [DataMember]
        public DateTime? UnregisteredDate { get; set; }

        ///// <summary>
        ///// User identity to use the App and other stuffs
        ///// </summary>
        [DataMember]
        public Guid? IdentityUserID { get; set; }

        [DataMember]
        public virtual Employee Employee { get; set; }

        [DataMember]
        public virtual PeriodDetail PeriodDetail { get; set; }

        [DataMember]
        public Guid? BankID { get; set; }
        [DataMember]
        public string CLABE { get; set; }
        [DataMember]
        public string BankBranchNumber { get; set; }
        [DataMember]
        public CotorriaStatus LocalStatus { get; set; }
        [DataMember]
        public DateTime LastStatusChange { get; set; }
        [DataMember]
        public Guid? ImmediateLeaderEmployeeID { get; set; }
    }
}
