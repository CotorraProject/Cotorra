using Cotorra.Core.Utils;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Cotorra.Schema.Calculation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cotorra.Core.Managers.Calculation
{
    public abstract class CalculationBase
    {
        #region "Attributes"
        private IFormulaManager _formulaManager;
        #endregion

        /// <summary>
        /// Get Overdraft Data necesary for the calculations
        /// </summary>
        /// <param name="overdraftID"></param>
        /// <param name="instanceID"></param>
        /// <param name="identityWorkID"></param>
        /// <returns></returns>
        public async static Task<CalculationBaseResult> GetDataAsync(Guid? overdraftID, Guid instanceID, Guid identityWorkID)
        {
            Stopwatch stopwatchGetData = new Stopwatch();
            stopwatchGetData.Start();

            var calculationBaseResult = new CalculationBaseResult();

            //validate OverdraftID
            if (null == overdraftID || overdraftID == Guid.Empty)
            {
                return calculationBaseResult;
            }

            //Get Overdraft and relationships
            var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
            var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                p.ID == overdraftID &&
                p.InstanceID == instanceID &&
                p.company == identityWorkID &&
                p.Active,
                identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.Workshift",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                    "Employee.HistoricEmployeeSBCAdjustments"
                     });

            Trace.WriteLine($"Get overdraft data elapsed time: {stopwatchGetData.Elapsed}");

            //validate overdrafts
            if (!overdrafts.Any())
            {
                return calculationBaseResult;
            }

            var overdraft = overdrafts.AsParallel().FirstOrDefault();

            //Period variables
            var periodDetailInitialDate = overdraft.PeriodDetail.InitialDate;
            var periodDetailFinalDate = overdraft.PeriodDetail.FinalDate;
            var periodDetailID = overdraft.PeriodDetailID;

            var employeeID = overdraft.EmployeeID;

            IEnumerable<Vacation> vacations = null;
            IEnumerable<VacationDaysOff> vacationDaysOff = null;
            IEnumerable<Incident> incidents = null;
            IEnumerable<Inhability> inhabilities = null;
            IEnumerable<MinimunSalary> minimunSalaries = null;
            IEnumerable<UMA> umas = null;
            IEnumerable<InfonavitMovement> infonavitMovements = null;
            IEnumerable<SGDFLimits> sGDFLimits = null;
            IEnumerable<IMSSEmployeeTable> iMSSEmployeeTables = null;
            IEnumerable<IMSSEmployerTable> iMSSEmployerTables = null;
            IEnumerable<IMSSWorkRisk> iMSSWorkRisks = null;
            IEnumerable<HistoricAccumulatedEmployee> historicAccumulatedEmployees = null;
            IEnumerable<SettlementCatalog> settlementCatalogs = null;
            IEnumerable<MonthlyIncomeTax> monthlyIncomeTaxes = null;
            IEnumerable<AnualIncomeTax> anualIncomeTaxes = null;
            IEnumerable<MonthlyEmploymentSubsidy> monthlyEmploymentSubsidies = null;
            IEnumerable<AnualEmploymentSubsidy> anualEmploymentSubsidies = null;
            IEnumerable<FonacotMovement> fonacotMovements = null;
            IEnumerable<IncidentType> incidentTypes = null;
            IEnumerable<AccumulatedType> accumulatedTypes = null;
            IEnumerable<ConceptPaymentRelationship> conceptPaymentRelationships = null;
            IEnumerable<ConceptPayment> conceptPayments = null;
            IEnumerable<EmployeeConceptsRelation> employeeConceptsRelations = null;
            IEnumerable<InfonavitInsurance> infonavitInsurances = null;
            IEnumerable<UMI> uMIs = null;

            using (var conn = new SqlConnection(ConnectionManager.ConfigConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }

                SqlDataAdapter da = new SqlDataAdapter("GetRequiredOverdraftCalculationData", conn);
                da.SelectCommand.Parameters.AddWithValue("@CompanyID", identityWorkID);
                da.SelectCommand.Parameters.AddWithValue("@InstanceID", instanceID);
                da.SelectCommand.Parameters.AddWithValue("@EmployeeID", employeeID);
                da.SelectCommand.Parameters.AddWithValue("@PeriodDetailID", periodDetailID);
                da.SelectCommand.Parameters.AddWithValue("@PeriodDetailInitialDate", periodDetailInitialDate);
                da.SelectCommand.Parameters.AddWithValue("@PeriodDetailFinalDate", periodDetailFinalDate);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                DataSet ds = new DataSet();
                da.Fill(ds);

                vacations = DataTableUtil.ConvertDataTable<Vacation>(ds.Tables[0]);
                incidents = DataTableUtil.ConvertDataTable<Incident>(ds.Tables[1]);
                inhabilities = DataTableUtil.ConvertDataTable<Inhability>(ds.Tables[2]);
                minimunSalaries = DataTableUtil.ConvertDataTable<MinimunSalary>(ds.Tables[3]);
                umas = DataTableUtil.ConvertDataTable<UMA>(ds.Tables[4]);
                infonavitMovements = DataTableUtil.ConvertDataTable<InfonavitMovement>(ds.Tables[5]);
                sGDFLimits = DataTableUtil.ConvertDataTable<SGDFLimits>(ds.Tables[6]);
                iMSSEmployeeTables = DataTableUtil.ConvertDataTable<IMSSEmployeeTable>(ds.Tables[7]);
                iMSSEmployerTables = DataTableUtil.ConvertDataTable<IMSSEmployerTable>(ds.Tables[8]);
                iMSSWorkRisks = DataTableUtil.ConvertDataTable<IMSSWorkRisk>(ds.Tables[9]);
                historicAccumulatedEmployees = DataTableUtil.ConvertDataTable<HistoricAccumulatedEmployee>(ds.Tables[10]);
                settlementCatalogs = DataTableUtil.ConvertDataTable<SettlementCatalog>(ds.Tables[11]);
                monthlyIncomeTaxes = DataTableUtil.ConvertDataTable<MonthlyIncomeTax>(ds.Tables[12]);
                anualIncomeTaxes = DataTableUtil.ConvertDataTable<AnualIncomeTax>(ds.Tables[13]);
                monthlyEmploymentSubsidies = DataTableUtil.ConvertDataTable<MonthlyEmploymentSubsidy>(ds.Tables[14]);
                anualEmploymentSubsidies = DataTableUtil.ConvertDataTable<AnualEmploymentSubsidy>(ds.Tables[15]);
                fonacotMovements = DataTableUtil.ConvertDataTable<FonacotMovement>(ds.Tables[16]);
                incidentTypes = DataTableUtil.ConvertDataTable<IncidentType>(ds.Tables[17]);
                accumulatedTypes = DataTableUtil.ConvertDataTable<AccumulatedType>(ds.Tables[18]);
                conceptPaymentRelationships = DataTableUtil.ConvertDataTable<ConceptPaymentRelationship>(ds.Tables[19]);
                conceptPayments = DataTableUtil.ConvertDataTable<ConceptPayment>(ds.Tables[20]);
                vacationDaysOff = DataTableUtil.ConvertDataTable<VacationDaysOff>(ds.Tables[21]);
                employeeConceptsRelations = DataTableUtil.ConvertDataTable<EmployeeConceptsRelation>(ds.Tables[22]);
                infonavitInsurances = DataTableUtil.ConvertDataTable<InfonavitInsurance>(ds.Tables[23]);
                uMIs = DataTableUtil.ConvertDataTable<UMI>(ds.Tables[24]);
            }

            Parallel.ForEach(vacations, vacation =>
            {
                var vacationDayOff = vacationDaysOff.Where(p => p.VacationID == vacation.ID);
                if (vacationDayOff.Any())
                {
                    vacation.VacationDaysOff.AddRange(vacationDayOff);
                }
            });

            Parallel.ForEach(fonacotMovements, fonacotMovement =>
            {
                fonacotMovement.EmployeeConceptsRelation = employeeConceptsRelations.FirstOrDefault(p => p.ID == fonacotMovement.EmployeeConceptsRelationID);
            });

            Parallel.ForEach(incidents, incident =>
            {
                incident.IncidentType = incidentTypes.FirstOrDefault(p => p.ID == incident.IncidentTypeID);
            });

            Parallel.ForEach(inhabilities, inhability =>
            {
                inhability.IncidentType = incidentTypes.FirstOrDefault(p => p.ID == inhability.IncidentTypeID);
            });

            Parallel.ForEach(historicAccumulatedEmployees, accumulatedEmployee =>
            {
                accumulatedEmployee.AccumulatedType = accumulatedTypes.FirstOrDefault(p => p.ID == accumulatedEmployee.AccumulatedTypeID);
            });

            Parallel.ForEach(conceptPaymentRelationships, conceptPaymentRelationship =>
            {
                conceptPaymentRelationship.AccumulatedType = accumulatedTypes.FirstOrDefault(p => p.ID == conceptPaymentRelationship.AccumulatedTypeID);
            });

            //Prepare data
            calculationBaseResult.Overdraft = overdraft;
            calculationBaseResult.Vacations = vacations;
            calculationBaseResult.Incidents = incidents;
            calculationBaseResult.Inhabilities = inhabilities;
            calculationBaseResult.MinimunSalaries = minimunSalaries;
            calculationBaseResult.SGDFLimits = sGDFLimits;
            calculationBaseResult.UMAs = umas;
            calculationBaseResult.InfonavitMovements = infonavitMovements;
            calculationBaseResult.IMSSEmployeeTables = iMSSEmployeeTables;
            calculationBaseResult.IMSSEmployerTables = iMSSEmployerTables;
            calculationBaseResult.IMSSWorkRisks = iMSSWorkRisks;
            calculationBaseResult.HistoricAccumulatedEmployees = historicAccumulatedEmployees;
            calculationBaseResult.SettlementCatalogs = settlementCatalogs;
            calculationBaseResult.MonthlyIncomeTaxes = monthlyIncomeTaxes;
            calculationBaseResult.AnualIncomeTaxes = anualIncomeTaxes;
            calculationBaseResult.MonthlyEmploymentSubsidies = monthlyEmploymentSubsidies;
            calculationBaseResult.AnualEmploymentSubsidies = anualEmploymentSubsidies;
            calculationBaseResult.FonacotMovements = fonacotMovements;
            calculationBaseResult.ConceptPaymentRelationships = conceptPaymentRelationships;
            calculationBaseResult.ConceptPayments = conceptPayments;
            calculationBaseResult.InfonavitInsurances = infonavitInsurances;
            calculationBaseResult.UMIs = uMIs;

            //track elapsed time
            stopwatchGetData.Stop();
            Trace.WriteLine($"GetData elapsed time: {stopwatchGetData.Elapsed}");

            return calculationBaseResult;
        }

        /// <summary>
        /// Replace the accumulates (InMemory) for the calculation
        /// </summary>
        /// <param name="accumulatedEmployees"></param>
        public void ReplaceAccumulates(IEnumerable<AccumulatedEmployee> accumulatedEmployees)
        {
            _formulaManager.FunctionParamsProperty.CalculationBaseResult.AccumulatedEmployees = accumulatedEmployees.ToList();
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="functionParams"></param>
        public void Initializate(FunctionParams functionParams)
        {
            _formulaManager = new FormulaManager(functionParams);
        }

        /// <summary>
        /// Do the calculation from formula specified
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        public CalculateResult Calculate(string formula, bool includeDetail = false)
        {
            return _formulaManager.Calculate(formula, includeDetail);
        }

    }
}
