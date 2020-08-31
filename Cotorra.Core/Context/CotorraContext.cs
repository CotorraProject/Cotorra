using CotorraNode.Common.Base.Schema;
using Cotorra.Schema;
using Cotorra.Schema.domain;
using Cotorra.Schema.domain.nom035;
using Cotorra.Schema.nom035;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Cotorra.Core.Context
{

    public static class ModelBuilderExtensions
    {
        public static void RemovePluralizingTableNameConvention(this ModelBuilder modelBuilder)
        {
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        public static void SetIDPrimaryKey(this ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            { 
                var e = entity as EntityType;
                var myList = new List<string>()
                {
                   "ID"
                };
                IReadOnlyList<string> list = myList;
                e.Builder.HasKey(list, ConfigurationSource.Convention);
            }
        }
    }

    public class CotorraContext : DbContext
    {
        #region "Attributes"
        private readonly string _connectionString;
        #endregion

        #region "Constructors"      

        static CotorraContext()
        {
        }

        public CotorraContext(DbContextOptions options, string connectionString) : base(options)
        {
            _connectionString = connectionString;
        }

        public CotorraContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {            
            optionsBuilder.ConfigureWarnings((i =>
            {
                i.Ignore(RelationalEventId.QueryClientEvaluationWarning);
            }));            
            
            if (!string.IsNullOrEmpty(_connectionString))
            {
                var conn = new SqlConnectionStringBuilder(_connectionString)
                {
                    MaxPoolSize = 600,
                    MinPoolSize = 5
                };

                optionsBuilder.UseSqlServer(conn.ToString());
            }

            base.OnConfiguring(optionsBuilder);
        }

        public CotorraContext()
        {
        }
        #endregion

        #region "Public Properties"
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<Period> Period { get; set; }
        public virtual DbSet<PeriodType> PeriodType { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<JobPosition> JobPosition { get; set; }
        public virtual DbSet<PayrollCompanyConfiguration> PayrollCompanyConfiguration { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<Workshift> Workshift { get; set; }
        public virtual DbSet<EmployerRegistration> EmployerRegistration { get; set; }
        public virtual DbSet<MinimunSalary> MinimunSalary { get; set; }
        public virtual DbSet<BenefitType> BenefitType { get; set; }
        public virtual DbSet<MonthlyIncomeTax> MonthlyIncomeTax { get; set; }
        public virtual DbSet<UMA> UMA { get; set; }
        public virtual DbSet<AnualIncomeTax> AnualIncomeTax { get; set; }
        public virtual DbSet<IMSSFare> IMSSFare { get; set; }
        public virtual DbSet<MonthlyEmploymentSubsidy> MonthlyEmploymentSubsidy { get; set; }
        public virtual DbSet<AnualEmploymentSubsidy> AnualEmploymentSubsidy { get; set; }
        public virtual DbSet<ConceptPayment> ConceptPayment { get; set; }
        public virtual DbSet<IncidentType> IncidentType { get; set; }
        public virtual DbSet<AccumulatedType> AccumulatedType { get; set; }
        public virtual DbSet<IncidentTypeRelationship> IncidentTypeRelationship { get; set; }
        public virtual DbSet<Area> Area { get; set; }
        public virtual DbSet<Incident> Incident { get; set; } 
        public virtual DbSet<Overdraft> Overdraft { get; set; }
        public virtual DbSet<OverdraftDetail> OverdraftDetail { get; set; }
        public virtual DbSet<PermanentMovement> PermanentMovement { get; set; }
        public virtual DbSet<FonacotMovement> FonacotMovement { get; set; }
        public virtual DbSet<Vacation> Vacation { get; set; }
        public virtual DbSet<Inhability> Inhability { get; set; }
        public virtual DbSet<InfonavitMovement> InfonavitMovement { get; set; }
        public virtual DbSet<Settlement> Settlement { get; set; }

        public virtual DbSet<ConceptPaymentRelationship> ConceptPaymentRelationship { get; set; }

        public virtual DbSet<Bank> Bank { get; set; }

        public virtual DbSet<EmployerFiscalInformation> EmployerFiscalInformation { get; set; }

        public virtual DbSet<SGDFLimits> SGDFLimits { get; set; }

        public virtual DbSet<IMSSEmployerTable> IMSSEmployerTable { get; set; }

        public virtual DbSet<IMSSEmployeeTable> IMSSEmployeeTable { get; set; }

        public virtual DbSet<IMSSWorkRisk> IMSSWorkRisk { get; set; }

        public virtual DbSet<SettlementCatalog> SettlementCatalog { get; set; }

        public virtual DbSet<AccumulatedEmployee> AccumulatedEmployee { get; set; }

        public virtual DbSet<EmployeeIdentityRegistration> EmployeeIdentityRegistration { get; set; }

        public virtual DbSet<catCFDI_CodigoPostal> catCFDI_CodigoPostal { get; set; }

        public virtual DbSet<UserCustomSettings> UserCustomSettings { get; set; }

        public virtual DbSet<CancelationFiscalDocument> CancelationFiscalDocument { get; set; }

        public virtual DbSet<CancelationFiscalDocumentDetail> CancelationFiscalDocumentDetail { get; set; }

        public virtual DbSet<VacationDaysOff> VacationDaysOff { get; set; }

        public virtual DbSet<EmployeeConceptsRelation> EmployeeConceptsRelation { get; set; }

        public virtual DbSet<EmployeeConceptsRelationDetail> EmployeeConceptsRelationDetail { get; set; }

        //Historic
        public virtual DbSet<HistoricEmployee> HistoricEmployee { get; set; }

        public virtual DbSet<HistoricAccumulatedEmployee> HistoricAccumulatedEmployee { get; set; }

        public virtual DbSet<EmployeeSalaryIncrease> EmployeeSalaryIncrease { get; set; }

        public virtual DbSet<EmployeeSBCAdjustment> EmployeeSBCAdjustment { get; set; }
        
        //NOM
        public virtual DbSet<NOMEvaluationGuide> NOMEvaluationGuide { get; set; }
        public virtual DbSet<NOMEvaluationPeriod> NOMEvaluationPeriod { get; set; }
        public virtual DbSet<NOMEvaluationPhase> NOMEvaluationPhase { get; set; }
        public virtual DbSet<NOMEvaluationCategory> NOMEvaluationCategory { get; set; }
        public virtual DbSet<NOMEvaluationDomain> NOMEvaluationDomain { get; set; }
        public virtual DbSet<NOMEvaluationQuestion> NOMEvaluationQuestion { get; set; }
        public virtual DbSet<NOMEvaluationSurvey> NOMEvaluationSurvey { get; set; }
        public virtual DbSet<WorkCenter> WorkCenter { get; set; }
        public virtual DbSet<NOMAnswer> NOMAnswer { get; set; }
        public virtual DbSet<NOMSurveyReply> NOMSurveyReply { get; set; }

        //Generales
        public virtual DbSet<UMI> UMI { get; set; }

        public virtual DbSet<InfonavitInsurance> InfonavitInsurance { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.SetIDPrimaryKey();
            modelBuilder.RemovePluralizingTableNameConvention();
        }

        #region "Public Methods"
        public string PluginID
        {
            get { return "16810D7A-4E31-4E7E-AFED-82192B980C6B"; }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion
    }
}
