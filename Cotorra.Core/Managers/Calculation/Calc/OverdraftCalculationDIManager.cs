using AutoMapper;
using MoreLinq;
using Cotorra.Schema;
using Cotorra.Schema.Calculation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cotorra.Core.Managers.Calculation
{
    public class OverdraftCalculationDIManager : ICalculationManager
    {
        #region "Attributes"
        private readonly IMapper _mapper;
        #endregion

        #region "Constructor"
        public OverdraftCalculationDIManager()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OverdraftDI, Overdraft>();
                cfg.CreateMap<OverdraftDetailDI, OverdraftDetail>();
                cfg.CreateMap<ConceptPaymentDI, ConceptPayment>()
                    .ForMember(dest => dest.Formula, opt => opt.MapFrom(src => src.FormulaTotal))
                    .ForMember(dest => dest.Formula1, opt => opt.MapFrom(src => src.FormulaTaxed))
                    .ForMember(dest => dest.Formula2, opt => opt.MapFrom(src => src.FormulaExempt))
                    .ForMember(dest => dest.Formula3, opt => opt.MapFrom(src => src.FormulaIMSSTaxed))
                    .ForMember(dest => dest.Formula4, opt => opt.MapFrom(src => src.FormulaIMSSExempt))
                    ;
                cfg.CreateMap<EmployeeDI, Employee>();
                cfg.CreateMap<WorkshiftDI, Workshift>();
                cfg.CreateMap<PeriodDetailDI, PeriodDetail>();
                cfg.CreateMap<PeriodDI, Period>();
            });

            _mapper = config.CreateMapper();
        }
        #endregion

        /// <summary>
        /// Get Overdraft Data necesary for the calculations
        /// </summary>
        /// <param name="overdraftID"></param>
        /// <param name="instanceID"></param>
        /// <param name="identityWorkID"></param>
        /// <returns></returns>
        private async Task<CalculationBaseResult> GetDataAsync(Guid instanceID, Guid identityWorkID, Guid userID)
        {
            var calculationBaseResult = new CalculationBaseResult();

            //Period variables
            var memoryManager = new MemoryStorageContext();

            //Prepare data          
            calculationBaseResult.MinimunSalaries = memoryManager.GetDefaultMinimunSalaries(identityWorkID, instanceID, userID);
            calculationBaseResult.SGDFLimits = memoryManager.GetDefaultSGDFLimits(identityWorkID, instanceID, userID);
            calculationBaseResult.UMAs = memoryManager.GetdDefaultUMA(identityWorkID, instanceID, userID);
            calculationBaseResult.IMSSEmployeeTables = memoryManager.GetDefaultIMSSEmployeeTable(identityWorkID, instanceID, userID);
            calculationBaseResult.IMSSEmployerTables = memoryManager.GetDefaultIMSSEmployerTable(identityWorkID, instanceID, userID);
            calculationBaseResult.IMSSWorkRisks = memoryManager.GetDefaultIMSSWorkRisk(identityWorkID, instanceID, userID);
            calculationBaseResult.SettlementCatalogs = memoryManager.GetDefaultSettlementCatalogTable(identityWorkID, instanceID, userID);
            calculationBaseResult.MonthlyIncomeTaxes = memoryManager.GetDefaultMonthlyIncomeTax(identityWorkID, instanceID, userID);
            calculationBaseResult.AnualIncomeTaxes = memoryManager.GetDefaultAnualIncomeTax(identityWorkID, instanceID, userID); ;
            calculationBaseResult.MonthlyEmploymentSubsidies = memoryManager.GetDefaultMonthlyEmploymentSubsidy(identityWorkID, instanceID, userID);
            calculationBaseResult.AnualEmploymentSubsidies = memoryManager.GetDefaultAnualEmploymentSubsidy(identityWorkID, instanceID, userID);
            var accumulatedTypes = memoryManager.GetDefaultAccumulatedType(identityWorkID, instanceID, userID);
            (var iconcepts, var conceptRelations) = memoryManager.GetDefaultConcept<ConceptPayment>(identityWorkID, instanceID, userID, accumulatedTypes);
            calculationBaseResult.ConceptPayments = iconcepts.Cast<ConceptPayment>().ToList();
            calculationBaseResult.ConceptPaymentRelationships = memoryManager.GetDefaultConceptPaymentRelationship(iconcepts, accumulatedTypes);
            calculationBaseResult.AccumulatedTypes = accumulatedTypes;

            foreach (var relationship in calculationBaseResult.ConceptPaymentRelationships)
            {
                var accumulatedType = calculationBaseResult.AccumulatedTypes.FirstOrDefault(q => q.ID == relationship.AccumulatedTypeID);
                relationship.AccumulatedType = accumulatedType;
                var conceptPayment = calculationBaseResult.ConceptPayments.FirstOrDefault(q => q.ID == relationship.ConceptPaymentID);
                relationship.ConceptPayment = conceptPayment;
            }

          
            return calculationBaseResult;
        }

        private ConceptPayment fixFormulasConceptPayment(ConceptPayment conceptPayment, CalculationBaseResult dataResponse)
        {
            var conceptDefault = dataResponse.ConceptPayments.FirstOrDefault(p =>
                p.Code == conceptPayment.Code &&
                p.ConceptType == conceptPayment.ConceptType);

            if (conceptPayment.FormulaValue == null)
            {
                conceptPayment.FormulaValue = conceptDefault.FormulaValue;
            }
            if (conceptPayment.Formula == null)
            {
                conceptPayment.Formula = conceptDefault.Formula;
            }
            if (conceptPayment.Formula1 == null)
            {
                conceptPayment.Formula1 = conceptDefault.Formula1;
            }
            if (conceptPayment.Formula2 == null)
            {
                conceptPayment.Formula2 = conceptDefault.Formula2;
            }
            if (conceptPayment.Formula3 == null)
            {
                conceptPayment.Formula3 = conceptDefault.Formula3;
            }
            if (conceptPayment.Formula4 == null)
            {
                conceptPayment.Formula4 = conceptDefault.Formula4;
            }

            if (conceptPayment.SATGroupCode == null)
            {
                conceptPayment.SATGroupCode = String.Empty;
            }

            //Name 
            conceptPayment.Name = conceptDefault.Name;

            return conceptPayment;
        }

        private async Task<CalculationBaseResult> FillDataAsync(CalculateOverdraftDIParams calculateOverdraftDIParams,
            CalculationBaseResult dataResponse)
        {
            dataResponse.Overdraft = _mapper.Map<OverdraftDI, Overdraft>(calculateOverdraftDIParams.OverdraftDI);
            dataResponse.Overdraft.PeriodDetail = _mapper.Map<PeriodDetailDI, PeriodDetail>(calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI);
            dataResponse.Overdraft.PeriodDetail.Period = _mapper.Map<PeriodDI, Period>(calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.PeriodDI);
            dataResponse.Overdraft.Employee = _mapper.Map<EmployeeDI, Employee>(calculateOverdraftDIParams.OverdraftDI.EmployeeDI);
            dataResponse.Overdraft.Employee.Workshift = _mapper.Map<WorkshiftDI, Workshift>(calculateOverdraftDIParams.OverdraftDI.EmployeeDI.WorkshiftDI);
            dataResponse.Overdraft.Employee.HistoricEmployeeSalaryAdjustments = new List<HistoricEmployeeSalaryAdjustment>();
            dataResponse.Overdraft.Employee.HistoricEmployeeSBCAdjustments = new List<HistoricEmployeeSBCAdjustment>();

            if (calculateOverdraftDIParams.OverdraftDI.OverdraftDetailDIs.Any())
            {
                for (int i = 0; i < calculateOverdraftDIParams.OverdraftDI.OverdraftDetailDIs.Count; i++)
                {
                    var detailDI = calculateOverdraftDIParams.OverdraftDI.OverdraftDetailDIs[i];
                    var overdraftDetail = _mapper.Map<OverdraftDetailDI, OverdraftDetail>(detailDI);
                    overdraftDetail.ConceptPayment = fixFormulasConceptPayment(_mapper.Map<ConceptPaymentDI, ConceptPayment>(detailDI.ConceptPaymentDI), dataResponse);

                    dataResponse.Overdraft.OverdraftDetails.Add(overdraftDetail);
                }
            }
            else
            {
                dataResponse.Overdraft.OverdraftDetails = new List<OverdraftDetail>();
                dataResponse.ConceptPayments.Where(p => p.GlobalAutomatic).ForEach(p =>
                  {
                      var overdraftDetail = new OverdraftDetail()
                      {
                          ConceptPayment = p,
                          ConceptPaymentID = p.ID,
                          ID = Guid.NewGuid()
                      };
                      dataResponse.Overdraft.OverdraftDetails.Add(overdraftDetail);
                  });
            }

            //Neccesary
            dataResponse.AccumulatedEmployees = new List<AccumulatedEmployee>();
            dataResponse.AccumulatedTypes.ForEach(p =>
            {

                var accumulatedEmployee = new AccumulatedEmployee()
                {
                    AccumulatedTypeID = p.ID,
                    AccumulatedType = p,
                    Employee = dataResponse.Overdraft.Employee,
                    InitialExerciseAmount = 0,
                    PreviousExerciseAccumulated = 0,
                    ExerciseFiscalYear = calculateOverdraftDIParams.OverdraftDI.PeriodDetailDI.PeriodDI.FiscalYear,
                };

                dataResponse.AccumulatedEmployees.Add(accumulatedEmployee);
            });


            //Other things
            dataResponse.FonacotMovements = new List<FonacotMovement>();
            dataResponse.HistoricAccumulatedEmployees = new List<HistoricAccumulatedEmployee>();
            dataResponse.Incidents = new List<Incident>();
            dataResponse.InfonavitMovements = new List<InfonavitMovement>();
            dataResponse.Inhabilities = new List<Inhability>();
            dataResponse.Vacations = new List<Vacation>();

            return dataResponse;
        }

        public async Task<ICalculateResult> CalculateAsync(ICalculateParams calculateParams)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //result instance
            var calculateResult = new CalculateOverdraftResult();
            var calculateOverdraftDIParams = calculateParams as CalculateOverdraftDIParams;

            //Functions params // Para inyectarle a las formulas los datos necesarios para el calculo (1 sola llamada a BD)
            var dataResponse = await GetDataAsync(
                calculateOverdraftDIParams.InstanceID,
                calculateOverdraftDIParams.IdentityWorkID,
                calculateOverdraftDIParams.UserID);

            //Fill necesary data to calculate          
            dataResponse = await FillDataAsync(calculateOverdraftDIParams, dataResponse);

            //Poner en 0 todos los amounts / previous calculation
            dataResponse.Overdraft.OverdraftDetails.ForEach(p =>
            {
                p.Taxed = 0M;
                p.Exempt = 0M;
                p.IMSSTaxed = 0M;
                p.IMSSExempt = 0M;

                if (calculateOverdraftDIParams.ResetCalculation)
                {
                    p.IsAmount1CapturedByUser = false;
                    p.IsAmount2CapturedByUser = false;
                    p.IsAmount3CapturedByUser = false;
                    p.IsAmount4CapturedByUser = false;
                    p.IsTotalAmountCapturedByUser = false;
                    p.IsValueCapturedByUser = false;
                    p.Value = 0M;
                    p.Amount = 0M;
                }
            });

            var calculateOverdraftParams = new CalculateOverdraftParams();
            calculateOverdraftParams.DeleteAccumulates = false;
            calculateOverdraftParams.IdentityWorkID = calculateOverdraftDIParams.IdentityWorkID;
            calculateOverdraftParams.InstanceID = calculateOverdraftDIParams.InstanceID;
            calculateOverdraftParams.OverdraftID = dataResponse.Overdraft.ID;
            calculateOverdraftParams.ResetCalculation = true;
            calculateOverdraftParams.SaveOverdraft = false;
            calculateOverdraftParams.UserID = calculateOverdraftDIParams.UserID;

            FunctionParams functionParams = new FunctionParams();
            functionParams.CalculationBaseResult = dataResponse;
            functionParams.IdentityWorkID = calculateOverdraftParams.IdentityWorkID;
            functionParams.InstanceID = calculateOverdraftParams.InstanceID;

            //overdraftDetails - All Perceptions, Deductions and Obligations
            List<AccumulatedEmployee> accumulatedEmployees = dataResponse.AccumulatedEmployees;
            var overdraftCalculationManager = new OverdraftCalculationManager();

            //Initializate data, arguments, and functions
            overdraftCalculationManager.Initializate(functionParams);

            //Perceptions
            (Overdraft overdraft, List<AccumulatedEmployee> employeeAccumulates) = await overdraftCalculationManager.CalculateByConceptAsync(dataResponse, ConceptType.SalaryPayment,
                calculateOverdraftParams, null);
            accumulatedEmployees = employeeAccumulates;
            dataResponse.Overdraft = overdraft;

            //Deductions
            (overdraft, employeeAccumulates) = await overdraftCalculationManager.CalculateByConceptAsync(dataResponse, ConceptType.DeductionPayment,
                calculateOverdraftParams, new ConcurrentBag<AccumulatedEmployee>(accumulatedEmployees));
            accumulatedEmployees = employeeAccumulates;
            dataResponse.Overdraft = overdraft;

            //Liabilities
            //Liability 
            dataResponse.IsLiability = true;
            (overdraft, employeeAccumulates) = await overdraftCalculationManager.CalculateByConceptAsync(dataResponse, ConceptType.LiabilityPayment,
                calculateOverdraftParams, new ConcurrentBag<AccumulatedEmployee>(accumulatedEmployees));
            dataResponse.Overdraft = overdraft;
            dataResponse.IsLiability = false;

            //Ajuste al neto
            var overdraftFinal = await overdraftCalculationManager.amountAdjustmentAsync(dataResponse.Overdraft, calculateOverdraftParams, dataResponse);

            //Prepare data to result
            calculateResult.OverdraftResult = overdraftFinal;

            stopwatch.Stop();
            Trace.WriteLine($"Time elapsed in the overdraft di calculation {stopwatch.Elapsed}");

            //return the result
            return calculateResult;
        }

        public async Task<CalculateResult> CalculateFormula(ICalculateParams calculateParams, string formula)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //result instance            
            var calculateOverdraftDIParams = calculateParams as CalculateOverdraftDIParams;

            //Functions params // Para inyectarle a las formulas los datos necesarios para el calculo (1 sola llamada a BD)
            var dataResponse = await GetDataAsync(
                calculateOverdraftDIParams.InstanceID,
                calculateOverdraftDIParams.IdentityWorkID,
                calculateOverdraftDIParams.UserID);

            //Fill necesary data to calculate          
            dataResponse = await FillDataAsync(calculateOverdraftDIParams, dataResponse);

            //Poner en 0 todos los amounts / previous calculation
            dataResponse.Overdraft.OverdraftDetails.ForEach(p =>
            {
                p.Taxed = 0M;
                p.Exempt = 0M;
                p.IMSSTaxed = 0M;
                p.IMSSExempt = 0M;

                if (calculateOverdraftDIParams.ResetCalculation)
                {
                    p.IsAmount1CapturedByUser = false;
                    p.IsAmount2CapturedByUser = false;
                    p.IsAmount3CapturedByUser = false;
                    p.IsAmount4CapturedByUser = false;
                    p.IsTotalAmountCapturedByUser = false;
                    p.IsValueCapturedByUser = false;
                    p.Value = 0M;
                    p.Amount = 0M;
                }
            });

            var calculateOverdraftParams = new CalculateOverdraftParams();
            calculateOverdraftParams.DeleteAccumulates = false;
            calculateOverdraftParams.IdentityWorkID = calculateOverdraftDIParams.IdentityWorkID;
            calculateOverdraftParams.InstanceID = calculateOverdraftDIParams.InstanceID;
            calculateOverdraftParams.OverdraftID = dataResponse.Overdraft.ID;
            calculateOverdraftParams.ResetCalculation = true;
            calculateOverdraftParams.SaveOverdraft = false;
            calculateOverdraftParams.UserID = calculateOverdraftDIParams.UserID;

            FunctionParams functionParams = new FunctionParams();
            functionParams.CalculationBaseResult = dataResponse;
            functionParams.IdentityWorkID = calculateOverdraftParams.IdentityWorkID;
            functionParams.InstanceID = calculateOverdraftParams.InstanceID;

            //overdraftDetails - All Perceptions, Deductions and Obligations
            List<AccumulatedEmployee> accumulatedEmployees = dataResponse.AccumulatedEmployees;
            var overdraftCalculationManager = new OverdraftCalculationManager();

            //Initializate data, arguments, and functions
            overdraftCalculationManager.Initializate(functionParams);

            //Perceptions
            (Overdraft overdraft, List<AccumulatedEmployee> employeeAccumulates) = await overdraftCalculationManager.CalculateByConceptAsync(dataResponse, ConceptType.SalaryPayment,
                calculateOverdraftParams, null);
            accumulatedEmployees = employeeAccumulates;
            dataResponse.Overdraft = overdraft;

            //Deductions
            (overdraft, employeeAccumulates) = await overdraftCalculationManager.CalculateByConceptAsync(dataResponse, ConceptType.DeductionPayment,
                calculateOverdraftParams, new ConcurrentBag<AccumulatedEmployee>(accumulatedEmployees));
            accumulatedEmployees = employeeAccumulates;
            dataResponse.Overdraft = overdraft;

            //Liabilities
            //Liability 
            dataResponse.IsLiability = true;
            (overdraft, employeeAccumulates) = await overdraftCalculationManager.CalculateByConceptAsync(dataResponse, ConceptType.LiabilityPayment,
                calculateOverdraftParams, new ConcurrentBag<AccumulatedEmployee>(accumulatedEmployees));
            dataResponse.Overdraft = overdraft;
            dataResponse.IsLiability = false;

            //Ajuste al neto
            await overdraftCalculationManager.amountAdjustmentAsync(dataResponse.Overdraft, calculateOverdraftParams, dataResponse);

            var calculareResult = overdraftCalculationManager.CalculateFormula(formula);

            stopwatch.Stop();
            Trace.WriteLine($"Time elapsed in the overdraft di calculation {stopwatch.Elapsed}");

            return calculareResult;
        }

        public CalculateResult CalculateFormula(string formula)
        {
            throw new NotImplementedException();
        }
    }
}
