using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.Calculation
{
    [DataContract]
    public class CalculationBaseResult
    {
        [DataMember]
        public Overdraft Overdraft { get; set; }

        [DataMember]
        public bool IsLiability { get; set; }

        [DataMember]
        public IEnumerable<Vacation> Vacations { get; set; }

        [DataMember]
        public IEnumerable<Incident> Incidents { get; set; }

        [DataMember]
        public IEnumerable<Inhability> Inhabilities { get; set; }

        [DataMember]
        public IEnumerable<MinimunSalary> MinimunSalaries { get; set; }

        [DataMember]
        public IEnumerable<SGDFLimits> SGDFLimits { get; set; }

        [DataMember]
        public IEnumerable<UMA> UMAs { get; set; }

        [DataMember]
        public IEnumerable<InfonavitMovement> InfonavitMovements { get; set; }

        [DataMember]
        public IEnumerable<IMSSEmployeeTable> IMSSEmployeeTables { get; set; }

        [DataMember]
        public IEnumerable<IMSSEmployerTable> IMSSEmployerTables { get; set; }

        [DataMember]
        public IEnumerable<IMSSWorkRisk> IMSSWorkRisks { get; set; }

        [DataMember]
        public IEnumerable<HistoricAccumulatedEmployee> HistoricAccumulatedEmployees { get; set; }

        [DataMember]
        public List<AccumulatedEmployee> AccumulatedEmployees { get; set; }

        [DataMember]
        public IEnumerable<SettlementCatalog> SettlementCatalogs { get; set; }

        [DataMember]
        public IEnumerable<MonthlyIncomeTax> MonthlyIncomeTaxes { get; set; }

        [DataMember]
        public IEnumerable<AnualIncomeTax> AnualIncomeTaxes { get; set; }

        [DataMember]
        public IEnumerable<MonthlyEmploymentSubsidy> MonthlyEmploymentSubsidies { get; set; }

        [DataMember]
        public IEnumerable<AnualEmploymentSubsidy> AnualEmploymentSubsidies { get; set; }

        [DataMember]
        public IEnumerable<FonacotMovement> FonacotMovements { get; set; }

        [DataMember]
        public IEnumerable<ConceptPaymentRelationship> ConceptPaymentRelationships { get; set; }

        [DataMember]
        public IEnumerable<ConceptPayment> ConceptPayments { get; set; }

        [DataMember]
        public List<AccumulatedType> AccumulatedTypes { get; set; }

        [DataMember]
        public IEnumerable<InfonavitInsurance> InfonavitInsurances { get; set; }

        [DataMember]
        public IEnumerable<UMI> UMIs { get; set; }

    }
}
