using Cotorra.Core.Utils;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Cotorra.Schema.Calculation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cotorra.Core.Managers.Calculation
{
    public class OverdraftCalculationManager : CalculationBase, ICalculationManager
    {
        /// <summary>
        /// sumConceptsAccumulatedByDetail
        /// </summary>
        /// <param name="overdraftDetail"></param>
        /// <param name="conceptPaymentRelationship"></param>
        /// <param name="conceptType"></param>
        /// <returns></returns>
        private decimal sumConceptsAccumulatedByDetail(OverdraftDetail overdraftDetail,
           ConceptPaymentRelationship conceptPaymentRelationship, ConceptType conceptType,
           ConceptPaymentType conceptPaymentType)
        {
            var sumconceptsToAccumulate = 0M;
            if (conceptPaymentRelationship.ConceptPaymentType == ConceptPaymentType.TotalAmount &&
                conceptPaymentRelationship.ConceptPaymentType == conceptPaymentType)
            {
                if (overdraftDetail.ConceptPayment.ConceptType == conceptType && overdraftDetail.ConceptPaymentID == conceptPaymentRelationship.ConceptPaymentID)
                {
                    sumconceptsToAccumulate += overdraftDetail.Amount;
                }
            }
            else if (conceptPaymentRelationship.ConceptPaymentType == ConceptPaymentType.Amount1 &&
                conceptPaymentRelationship.ConceptPaymentType == conceptPaymentType)
            {
                if (overdraftDetail.ConceptPayment.ConceptType == conceptType &&
                    overdraftDetail.ConceptPaymentID == conceptPaymentRelationship.ConceptPaymentID)
                {
                    sumconceptsToAccumulate += overdraftDetail.Taxed;
                }
            }
            else if (conceptPaymentRelationship.ConceptPaymentType == ConceptPaymentType.Amount2 &&
                conceptPaymentRelationship.ConceptPaymentType == conceptPaymentType)
            {
                if (overdraftDetail.ConceptPayment.ConceptType == conceptType &&
                   overdraftDetail.ConceptPaymentID == conceptPaymentRelationship.ConceptPaymentID)
                {
                    sumconceptsToAccumulate += overdraftDetail.Exempt;
                }
            }
            else if (conceptPaymentRelationship.ConceptPaymentType == ConceptPaymentType.Amount3 &&
                conceptPaymentRelationship.ConceptPaymentType == conceptPaymentType)
            {
                if (overdraftDetail.ConceptPayment.ConceptType == conceptType &&
                   overdraftDetail.ConceptPaymentID == conceptPaymentRelationship.ConceptPaymentID)
                {
                    sumconceptsToAccumulate += overdraftDetail.IMSSTaxed;
                }
            }
            else if (conceptPaymentRelationship.ConceptPaymentType == ConceptPaymentType.Amount4 &&
                conceptPaymentRelationship.ConceptPaymentType == conceptPaymentType)
            {
                if (overdraftDetail.ConceptPayment.ConceptType == conceptType &&
                  overdraftDetail.ConceptPaymentID == conceptPaymentRelationship.ConceptPaymentID)
                {
                    sumconceptsToAccumulate += overdraftDetail.IMSSExempt;
                }
            }

            return sumconceptsToAccumulate;
        }

        /// <summary>
        /// Templeate para crear los acumulados
        /// </summary>
        /// <param name="conceptPaymentRelationship"></param>
        /// <param name="overdraft"></param>
        /// <param name="periodFiscalYear"></param>
        /// <param name="identityWorkID"></param>
        /// <param name="instanceID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        private AccumulatedEmployee createDefault(ConceptPaymentRelationship conceptPaymentRelationship,
            Guid employeeID, int periodFiscalYear,
            Guid identityWorkID, Guid instanceID, Guid userID)
        {
            var accumulatedEmployee = new AccumulatedEmployee()
            {
                ID = Guid.NewGuid(),
                Active = true,
                AccumulatedTypeID = conceptPaymentRelationship.AccumulatedTypeID,
                company = identityWorkID,
                EmployeeID = employeeID,
                CreationDate = DateTime.UtcNow,
                DeleteDate = null,
                user = userID,
                InstanceID = instanceID,
                Description = String.Empty,
                Name = String.Empty,
                Timestamp = DateTime.UtcNow,
                StatusID = 1,
                ExerciseFiscalYear = periodFiscalYear,
            };

            return accumulatedEmployee;
        }

        /// <summary>
        /// Acumulados a partir de los cálculos en los conceptos
        /// </summary>
        /// <param name="overdraft"></param>
        /// <param name="instanceID"></param>
        /// <param name="identityWorkID"></param>
        /// <param name="userID"></param>
        /// <param name="conceptPaymentRelationships"></param>
        /// <param name="conceptType"></param>
        /// <param name="totalAmount"></param>
        /// <param name="accumulatedEmployeesToCreateUpdate"></param>
        /// <returns></returns>
        private async Task<ConcurrentBag<AccumulatedEmployee>> accumulatesConceptsLocalAsync(
              OverdraftDetail overdraftDetail,
              Guid employeeID,
              int periodFiscalYear,
              int month,
              Guid instanceID, Guid identityWorkID, Guid userID,
              ConcurrentBag<ConceptPaymentRelationship> conceptPaymentRelationships,
              ConceptType conceptType,
              ConceptPaymentType conceptPaymentType,
              ConcurrentBag<AccumulatedEmployee> accumulatedEmployeesToCreateUpdate = null)
        {
            if (null == accumulatedEmployeesToCreateUpdate)
            {
                accumulatedEmployeesToCreateUpdate = new ConcurrentBag<AccumulatedEmployee>(new List<AccumulatedEmployee>());
            }

            var conceptPaymentsRelationSubList =
                conceptPaymentRelationships.Where(p =>
                p.ConceptPaymentID == overdraftDetail.ConceptPaymentID);

            //acumular montos por conceptos 
            Parallel.ForEach(conceptPaymentsRelationSubList, cpr =>
            {
                var conceptPaymentRelationship = cpr;
                var sumconceptsToAccumulate = 0M;

                sumconceptsToAccumulate = sumConceptsAccumulatedByDetail(
                    overdraftDetail, conceptPaymentRelationship,
                    conceptType, conceptPaymentType);

                var accumulatedEmployee = default(AccumulatedEmployee);
                bool isNew = false;

                //si existe en las listas ya no se agrega
                var accuList = accumulatedEmployeesToCreateUpdate.FirstOrDefault(p =>
                        p.InstanceID == instanceID &&
                        p.AccumulatedTypeID == conceptPaymentRelationship.AccumulatedTypeID &&
                        p.EmployeeID == employeeID &&
                        p.ExerciseFiscalYear == periodFiscalYear);

                if (accuList == null || accuList == default(AccumulatedEmployee))
                {
                    //Create entity
                    accumulatedEmployee = createDefault(conceptPaymentRelationship,
                        employeeID,
                        periodFiscalYear,
                        identityWorkID,
                        instanceID,
                        userID);
                    accumulatedEmployee.AccumulatedType = conceptPaymentRelationship.AccumulatedType;
                    isNew = true;
                }
                else
                {
                    accumulatedEmployee = accuList;
                    accumulatedEmployee.AccumulatedType = conceptPaymentRelationship.AccumulatedType;
                }

                if (month == 1) accumulatedEmployee.January = accumulatedEmployee.January + sumconceptsToAccumulate;
                else if (month == 2) accumulatedEmployee.February = accumulatedEmployee.February + sumconceptsToAccumulate;
                else if (month == 3) accumulatedEmployee.March = accumulatedEmployee.March + sumconceptsToAccumulate;
                else if (month == 4) accumulatedEmployee.April = accumulatedEmployee.April + sumconceptsToAccumulate;
                else if (month == 5) accumulatedEmployee.May = accumulatedEmployee.May + sumconceptsToAccumulate;
                else if (month == 6) accumulatedEmployee.June = accumulatedEmployee.June + sumconceptsToAccumulate;
                else if (month == 7) accumulatedEmployee.July = accumulatedEmployee.July + sumconceptsToAccumulate;
                else if (month == 8) accumulatedEmployee.August = accumulatedEmployee.August + sumconceptsToAccumulate;
                else if (month == 9) accumulatedEmployee.September = accumulatedEmployee.September + sumconceptsToAccumulate;
                else if (month == 10) accumulatedEmployee.October = accumulatedEmployee.October + sumconceptsToAccumulate;
                else if (month == 11) accumulatedEmployee.November = accumulatedEmployee.November + sumconceptsToAccumulate;
                else if (month == 12) accumulatedEmployee.December = accumulatedEmployee.December + sumconceptsToAccumulate;

                if (isNew)
                {
                    accumulatedEmployeesToCreateUpdate.Add(accumulatedEmployee);
                }
            });

            return accumulatedEmployeesToCreateUpdate;
        }

        /// <summary>
        /// Calcula todos los conceptos por tipo de concepto
        /// </summary>
        /// <param name="calculationBaseResult"></param>
        /// <param name="conceptType"></param>
        /// <param name="calculateOverdraftParams"></param>
        /// <param name="accumulatedEmployees"></param>
        /// <returns></returns>
        internal async Task<(Overdraft, List<AccumulatedEmployee>)> CalculateByConceptAsync(
            CalculationBaseResult calculationBaseResult,
            ConceptType conceptType,
            CalculateOverdraftParams calculateOverdraftParams,
            ConcurrentBag<AccumulatedEmployee> accumulatedEmployees)
        {
            //1. calculate perceptions         
            var overdraftDetailsToIterate = calculationBaseResult.Overdraft.OverdraftDetails
                .AsParallel()
                .Where(p => p.ConceptPayment.ConceptType == conceptType)
                .OrderBy(p => p.ConceptPayment.Code);

            var employeeID = calculationBaseResult.Overdraft.EmployeeID;
            var year = calculationBaseResult.Overdraft.PeriodDetail.FinalDate.Year;
            var month = calculationBaseResult.Overdraft.PeriodDetail.FinalDate.Month;

            foreach (var overdraftDetail in overdraftDetailsToIterate)
            {
                var formula = overdraftDetail.ConceptPayment.Formula;
                var formulaValue = overdraftDetail.ConceptPayment.FormulaValue;
                var formula1 = overdraftDetail.ConceptPayment.Formula1;
                var formula2 = overdraftDetail.ConceptPayment.Formula2;
                var formula3 = overdraftDetail.ConceptPayment.Formula3;
                var formula4 = overdraftDetail.ConceptPayment.Formula4;

                //Valor / Dias / etc
                if (!String.IsNullOrEmpty(formulaValue) && !overdraftDetail.IsValueCapturedByUser)
                {
                    var calculateResultValueLocal = base.Calculate(formulaValue);
                    overdraftDetail.Value = calculateResultValueLocal.Result;
                }

                if (!String.IsNullOrEmpty(formula) && !overdraftDetail.IsTotalAmountCapturedByUser)
                {
                    //Monto / Total
                    var calculateResultLocal = base.Calculate(formula);
                    overdraftDetail.Amount = calculateResultLocal.Result;
                }

                //Calculate the accumulates for perceptions of the period
                accumulatedEmployees = await accumulatesConceptsLocalAsync(
                    overdraftDetail,
                    employeeID,
                    year,
                    month,
                    calculateOverdraftParams.InstanceID,
                    calculateOverdraftParams.IdentityWorkID,
                    calculateOverdraftParams.UserID,
                    new ConcurrentBag<ConceptPaymentRelationship>(
                        calculationBaseResult.ConceptPaymentRelationships),
                    conceptType,
                    ConceptPaymentType.TotalAmount,
                    accumulatedEmployees
                    );

                ////Refresh accumulates for calculation
                base.ReplaceAccumulates(accumulatedEmployees);

                if (!String.IsNullOrEmpty(formula1) && !overdraftDetail.IsAmount1CapturedByUser)
                {
                    var calculateResultLocal1 = base.Calculate(formula1);
                    overdraftDetail.Taxed = calculateResultLocal1.Result;
                }

                //Calculate the accumulates for perceptions of the period
                accumulatedEmployees = await accumulatesConceptsLocalAsync(
                    overdraftDetail,
                    employeeID,
                    year,
                    month,
                    calculateOverdraftParams.InstanceID,
                    calculateOverdraftParams.IdentityWorkID,
                    calculateOverdraftParams.UserID,
                    new ConcurrentBag<ConceptPaymentRelationship>(
                        calculationBaseResult.ConceptPaymentRelationships),
                    conceptType,
                    ConceptPaymentType.Amount1,
                    accumulatedEmployees
                    );

                ////Refresh accumulates for calculation
                base.ReplaceAccumulates(accumulatedEmployees);

                if (!String.IsNullOrEmpty(formula2) && !overdraftDetail.IsAmount2CapturedByUser)
                {
                    var calculateResultLocal2 = base.Calculate(formula2);
                    overdraftDetail.Exempt = calculateResultLocal2.Result;
                }

                //Calculate the accumulates for perceptions of the period
                accumulatedEmployees = await accumulatesConceptsLocalAsync(
                    overdraftDetail,
                    employeeID,
                    year,
                    month,
                    calculateOverdraftParams.InstanceID,
                    calculateOverdraftParams.IdentityWorkID,
                    calculateOverdraftParams.UserID,
                    new ConcurrentBag<ConceptPaymentRelationship>(
                        calculationBaseResult.ConceptPaymentRelationships),
                    conceptType,
                    ConceptPaymentType.Amount2,
                    accumulatedEmployees
                    );

                ////Refresh accumulates for calculation
                base.ReplaceAccumulates(accumulatedEmployees);

                if (!String.IsNullOrEmpty(formula3) && !overdraftDetail.IsAmount3CapturedByUser)
                {
                    var calculateResultLocal3 = base.Calculate(formula3);
                    overdraftDetail.IMSSTaxed = calculateResultLocal3.Result;
                }

                //Calculate the accumulates for perceptions of the period
                accumulatedEmployees = await accumulatesConceptsLocalAsync(
                    overdraftDetail,
                    employeeID,
                    year,
                    month,
                    calculateOverdraftParams.InstanceID,
                    calculateOverdraftParams.IdentityWorkID,
                    calculateOverdraftParams.UserID,
                    new ConcurrentBag<ConceptPaymentRelationship>(
                        calculationBaseResult.ConceptPaymentRelationships),
                    conceptType,
                    ConceptPaymentType.Amount3,
                    accumulatedEmployees
                    );

                ////Refresh accumulates for calculation
                base.ReplaceAccumulates(accumulatedEmployees);

                if (!String.IsNullOrEmpty(formula4) && !overdraftDetail.IsAmount4CapturedByUser)
                {
                    var calculateResultLocal4 = base.Calculate(formula4);
                    overdraftDetail.IMSSExempt = calculateResultLocal4.Result;
                }

                //Calculate the accumulates for perceptions of the period
                accumulatedEmployees = await accumulatesConceptsLocalAsync(
                    overdraftDetail,
                    employeeID,
                    year,
                    month,
                    calculateOverdraftParams.InstanceID,
                    calculateOverdraftParams.IdentityWorkID,
                    calculateOverdraftParams.UserID,
                    new ConcurrentBag<ConceptPaymentRelationship>(
                        calculationBaseResult.ConceptPaymentRelationships),
                    conceptType,
                    ConceptPaymentType.Amount4,
                    accumulatedEmployees
                    );

                ////Refresh accumulates for calculation
                base.ReplaceAccumulates(accumulatedEmployees);
            }

            return (calculationBaseResult.Overdraft, accumulatedEmployees.ToList());
        }

        /// <summary>
        /// Saves overdraft and overdraft details, and the accumulates for employee
        /// </summary>
        /// <param name="overdraftToSave"></param>
        /// <param name="accumulatedEmployees"></param>
        /// <param name="identityWorkID"></param>
        /// <returns></returns>
        private async Task saveOverdraft(Overdraft overdraftToSave,
            CalculateOverdraftParams calculateOverdraftParams)
        {
            //Prepare datatable to save           
            DataTable dtOverdraftGuidList = new DataTable();
            dtOverdraftGuidList.Columns.Add("ID", typeof(Guid));
            dtOverdraftGuidList.Columns.Add("OverdraftID", typeof(Guid));
            dtOverdraftGuidList.Columns.Add("ConceptPaymentID", typeof(Guid));
            dtOverdraftGuidList.Columns.Add("Value", typeof(decimal));
            dtOverdraftGuidList.Columns.Add("Amount", typeof(decimal));
            dtOverdraftGuidList.Columns.Add("Taxed", typeof(decimal));
            dtOverdraftGuidList.Columns.Add("Exempt", typeof(decimal));
            dtOverdraftGuidList.Columns.Add("IMSSTaxed", typeof(decimal));
            dtOverdraftGuidList.Columns.Add("IMSSExempt", typeof(decimal));
            dtOverdraftGuidList.Columns.Add("IsGeneratedByPermanentMovement", typeof(bool));

            overdraftToSave.OverdraftDetails.ForEach(detail =>
            {
                dtOverdraftGuidList.Rows.Add(
                    detail.ID,
                    detail.OverdraftID,
                    detail.ConceptPaymentID,
                    detail.Value,
                    detail.Amount,
                    detail.Taxed,
                    detail.Exempt,
                    detail.IMSSTaxed,
                    detail.IMSSExempt,
                    detail.IsGeneratedByPermanentMovement);

            });


            //connection
            using (var connection = new SqlConnection(ConnectionManager.ConfigConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "CalculousOverdraft";

                    //OverdraftDetails Parameter                                    
                    SqlParameter paramOverdraftIdsTable = new SqlParameter("@OverdraftDetails", SqlDbType.Structured)
                    {
                        TypeName = "dbo.stampoverdraftdetailtabletype",
                        Value = dtOverdraftGuidList
                    };
                    command.Parameters.Add(paramOverdraftIdsTable);

                    command.Parameters.AddWithValue("@InstanceId", calculateOverdraftParams.InstanceID);
                    command.Parameters.AddWithValue("@company", calculateOverdraftParams.IdentityWorkID);
                    command.Parameters.AddWithValue("@user", calculateOverdraftParams.UserID);

                    //Execute SP de autorización
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Create Or Update ConceptPayment
        /// </summary>
        /// <param name="overdraftID"></param>
        /// <param name="conceptPaymentID"></param>
        /// <param name="amount"></param>
        /// <param name="instanceID"></param>
        /// <param name="identityWorkID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<Guid> createUpdateConceptPayment(
            Guid overdraftID, Guid conceptPaymentID,
            decimal amount, Guid instanceID,
            Guid identityWorkID, Guid user
            )
        {
            Guid ID = Guid.Empty;

            using (var connection = new SqlConnection(ConnectionManager.ConfigConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "CreateUpdateConceptPayment";

                    //@Amount
                    command.Parameters.AddWithValue("@OverdraftId", overdraftID);
                    command.Parameters.AddWithValue("@ConceptPaymentId", conceptPaymentID);
                    command.Parameters.AddWithValue("@Amount", amount);
                    command.Parameters.AddWithValue("@InstanceId", instanceID);
                    command.Parameters.AddWithValue("@company", identityWorkID);
                    command.Parameters.AddWithValue("@user", user);

                    //Execute SP de autorización
                    var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        ID = Guid.Parse(reader[0].ToString());
                    }
                    await reader.CloseAsync();
                }
            }

            return ID;
        }

        /// <summary>
        /// Ajuste al neto
        /// </summary>
        /// <param name="overdraftFinal"></param>
        /// <param name="calculateOverdraftParams"></param>
        /// <param name="dataResponse"></param>
        /// <returns></returns>
        internal async Task<Overdraft> amountAdjustmentAsync(Overdraft overdraftFinal,
            CalculateOverdraftParams calculateOverdraftParams, CalculationBaseResult dataResponse)
        {
            //Ajuste al neto o ajuste en sueldos
            const int BASE_ROUND_RATE = 10;
            const int BASE_PERCENTAGE = 100;
            const int AMOUNT_ADJUSTMENT_CODE = 99;

            var overdraftManager = new OverdraftManager();
            var totals = overdraftManager.GetTotals(overdraftFinal, new RoundUtil("MXN"));

            //neto
            var perceptions = totals.TotalSalaryTotals;
            var deductions = totals.TotalDeductionPayments;
            var neto = perceptions - deductions;
            var netoCents = ((neto * BASE_ROUND_RATE) - Math.Truncate(neto * BASE_ROUND_RATE)) / BASE_ROUND_RATE;
            var perceptionsCents = ((perceptions * BASE_ROUND_RATE) - Math.Truncate(perceptions * BASE_ROUND_RATE)) / BASE_ROUND_RATE;
            var deductionsCents = ((deductions * BASE_ROUND_RATE) - Math.Truncate(deductions * BASE_ROUND_RATE)) / BASE_ROUND_RATE;

            //calculate difference
            var difference = 0M;
            if ((netoCents * BASE_PERCENTAGE) % BASE_ROUND_RATE != 0)
            {
                difference = Math.Abs(netoCents - (BASE_ROUND_RATE / BASE_PERCENTAGE));
                if (difference != 0)
                {
                    difference = -1 * Math.Abs(0.1M - difference);
                }
            }

            var detail = overdraftFinal.OverdraftDetails.FirstOrDefault(
                p => p.ConceptPayment.Code == AMOUNT_ADJUSTMENT_CODE &&
                p.ConceptPayment.ConceptType == ConceptType.DeductionPayment);

            //connection
            var conceptPayment = dataResponse.ConceptPayments.FirstOrDefault(p =>
                p.Code == 99 &&
                p.ConceptType == ConceptType.DeductionPayment);

            var overdraftID = overdraftFinal.ID;
            var conceptPaymentID = conceptPayment.ID;

            //Create or Update ConceptPayment Neto Adjustment
            var ID = Guid.NewGuid();

            if (calculateOverdraftParams.SaveOverdraft)
            {
                ID = await createUpdateConceptPayment(overdraftID, conceptPaymentID, difference,
                    calculateOverdraftParams.InstanceID, calculateOverdraftParams.IdentityWorkID,
                    calculateOverdraftParams.UserID);
            }

            if (detail == null)
            {
                var overdraftDetail = new OverdraftDetail()
                {
                    Active = true,
                    Amount = difference,
                    company = calculateOverdraftParams.IdentityWorkID,
                    ConceptPaymentID = conceptPayment.ID,
                    ConceptPayment = null,
                    CreationDate = DateTime.UtcNow,
                    DeleteDate = null,
                    Description = "Ajuste al neto (autogenerado)",
                    Name = "Ajuste al neto (autogenerado)",
                    ID = ID,
                    InstanceID = calculateOverdraftParams.InstanceID,
                    OverdraftID = overdraftFinal.ID,
                    user = calculateOverdraftParams.UserID,
                    Timestamp = DateTime.UtcNow,
                    StatusID = 1,
                    Label1 = "Gravado",
                    Label2 = "Exento",
                    Label3 = "IMSS Gravado",
                    Label4 = "IMSS Exento",
                };

                //add to overdraftFinal
                overdraftDetail.ConceptPayment = conceptPayment;
                overdraftFinal.OverdraftDetails.Add(overdraftDetail);
            }
            else
            {
                detail.Amount = difference;
            }

            return overdraftFinal;
        }

        /// <summary>
        /// Do parallel calculation and deletes accumulates
        /// </summary>
        /// <param name="overdrafts"></param>
        /// <param name="identityWorkID"></param>
        /// <param name="instanceID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        private async Task doParallelCalculationAsync(IEnumerable<Overdraft> overdrafts, Guid identityWorkID, Guid instanceID, Guid userID)
        {
            //10. Calcula los Overdraft de todos los empleados de ese periodo
            foreach (var overdraftToCalculate in overdrafts)
            {
                ICalculationManager calculationManager = new OverdraftCalculationManager();
                CalculateOverdraftParams calculateOverdraftParams = new CalculateOverdraftParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraftToCalculate.ID,
                    ResetCalculation = false,
                    DeleteAccumulates = false,
                    UserID = userID
                };
                calculationManager.CalculateAsync(calculateOverdraftParams);
            }
        }

        private async Task doCalculationAsync(IEnumerable<Overdraft> overdrafts, Guid identityWorkID, Guid instanceID, Guid userID)
        {
            //10. Calcula los Overdraft de todos los empleados de ese periodo
            foreach (var overdraftToCalculate in overdrafts)
            {
                ICalculationManager calculationManager = new OverdraftCalculationManager();
                CalculateOverdraftParams calculateOverdraftParams = new CalculateOverdraftParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraftToCalculate.ID,
                    ResetCalculation = false,
                    DeleteAccumulates = false,
                    UserID = userID
                };
                await calculationManager.CalculateAsync(calculateOverdraftParams);
            }
        }

        /// <summary>
        /// Calculation for overdraftsIds
        /// </summary>
        /// <param name="overdraftsIds"></param>
        /// <param name="identityWorkID"></param>
        /// <param name="instanceID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        private async Task doParallelCalculationAsync(IEnumerable<Guid> overdraftsIds, Guid identityWorkID, Guid instanceID, Guid userID)
        {
            //10. Calcula los Overdraft de todos los empleados de ese periodo
            foreach (var overdraftToCalculate in overdraftsIds)
            {
                ICalculationManager calculationManager = new OverdraftCalculationManager();
                CalculateOverdraftParams calculateOverdraftParams = new CalculateOverdraftParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraftToCalculate,
                    ResetCalculation = false,
                    DeleteAccumulates = false,
                    UserID = userID
                };
                calculationManager.CalculateAsync(calculateOverdraftParams);
            }
        }

        private EmployeeConceptsRelationDetail getDefaultEmployeeConceptRelation(Guid instanceID, Guid companyID, Guid user,
            decimal amountApplied, Overdraft overdraft, Guid employeeConceptRelationID)
        {
            var employeeConceptRelationDetail = new EmployeeConceptsRelationDetail()
            {
                Active = true,
                AmountApplied = amountApplied,
                company = companyID,
                ConceptsRelationPaymentStatus = ConceptsRelationPaymentStatus.Pending,
                CreationDate = DateTime.Now,
                CompanyID = companyID,
                DeleteDate = null,
                Description = "",
                EmployeeConceptsRelationID = employeeConceptRelationID,
                ID = Guid.NewGuid(),
                IdentityID = user,
                InstanceID = instanceID,
                Name = "",
                OverdraftID = overdraft.ID,
                PaymentDate = overdraft.PeriodDetail.InitialDate,
                StatusID = 1,
                Timestamp = DateTime.Now,
                ValueApplied = 0,
                IsAmountAppliedCapturedByUser = false
            };
            return employeeConceptRelationDetail;
        }

        private async Task<List<EmployeeConceptsRelationDetail>> getConceptRelated(
             Overdraft overdraft,
             int code,
             ConceptType conceptType)
        {
            var lstDetails = new List<EmployeeConceptsRelationDetail>();
            var companyID = overdraft.company;
            var instanceID = overdraft.InstanceID;
            var user = overdraft.user;

            var middlewareManager = new MiddlewareManager<EmployeeConceptsRelation>(new BaseRecordManager<EmployeeConceptsRelation>(),
                 new EmployeeConceptsRelationValidator());

            var overdraftDetail = overdraft.OverdraftDetails
                .FirstOrDefault(p =>
                    p.ConceptPayment.Code == code &&
                    p.ConceptPayment.ConceptType == conceptType);

            if (null != overdraftDetail)
            {
                var employeeConceptsRelations = await middlewareManager.FindByExpressionAsync(p =>
                    p.ConceptPaymentID == overdraftDetail.ConceptPaymentID &&
                    p.InstanceID == instanceID &&
                    p.EmployeeID == overdraft.EmployeeID &&                    
                    p.ConceptPaymentStatus == ConceptPaymentStatus.Active, companyID);

                if (employeeConceptsRelations.Any())
                {
                    lstDetails.Add(getDefaultEmployeeConceptRelation(
                        instanceID,
                        companyID,
                        user,
                        overdraftDetail.Amount,
                        overdraft,
                        employeeConceptsRelations.FirstOrDefault().ID
                        ));
                }
            }

            return lstDetails;
        }

        private async Task<List<EmployeeConceptsRelationDetail>> getInfonavitConceptRelated(
        Overdraft overdraft,
        InfonavitCreditType infonavitCreditType,
        ConceptType conceptType)
        {
            var code = infonavitCreditType switch
            {
                InfonavitCreditType.FixQuota_D16 => 16,
                InfonavitCreditType.DiscountFactor_D15 => 15,
                InfonavitCreditType.Percentage_D59 => 59,
                InfonavitCreditType.HomeInsurance_D14 => 14,
                _ => throw new NotImplementedException("Tipo de retención infonavit inexistente"),
            };

            return await getConceptRelated(overdraft, code, conceptType);
        }

        private async Task saveConceptRelations(FunctionParams functionParams,
            CalculateOverdraftParams calculateOverdraftParams, Overdraft overdraft)
        {
            var details = functionParams.EmployeeConceptsRelationDetails;
            var identityWorkID = calculateOverdraftParams.IdentityWorkID;
            var middlewareManager = new MiddlewareManager<EmployeeConceptsRelationDetail>(new BaseRecordManager<EmployeeConceptsRelationDetail>(),
                   new EmployeeConceptsRelationDetailValidator());
            var toCreate = new List<EmployeeConceptsRelationDetail>();
            var toUpdate = new List<EmployeeConceptsRelationDetail>();

            //get infonavit movements
            details.AddRange(await getInfonavitConceptRelated(overdraft, InfonavitCreditType.Percentage_D59, ConceptType.DeductionPayment));
            details.AddRange(await getInfonavitConceptRelated(overdraft, InfonavitCreditType.FixQuota_D16, ConceptType.DeductionPayment));
            details.AddRange(await getInfonavitConceptRelated(overdraft, InfonavitCreditType.DiscountFactor_D15, ConceptType.DeductionPayment));
            details.AddRange(await getInfonavitConceptRelated(overdraft, InfonavitCreditType.HomeInsurance_D14, ConceptType.DeductionPayment));

            //Consultar a BD todos los EmployeeConceptsRelationID
            if (details.Any())
            {
                var ids = details.Select(p => p.EmployeeConceptsRelationID);

                var detailsFound = await middlewareManager.FindByExpressionAsync(p =>
                                 ids.Contains(p.EmployeeConceptsRelationID), identityWorkID);

                //Ver si existen en BD o no
                details.ForEach(detail =>
                {
                    //Encontrar el EmployeeConceptsRelation correspondiente para ver si es create or update
                    var detailInListFound = detailsFound.Where(p =>
                        p.OverdraftID == detail.OverdraftID &&
                        p.EmployeeConceptsRelationID == detail.EmployeeConceptsRelationID &&
                        p.InstanceID == detail.InstanceID);

                    if (detailInListFound.Any())
                    {
                        detailInListFound.FirstOrDefault().AmountApplied = detail.AmountApplied;
                        detailInListFound.FirstOrDefault().Timestamp = DateTime.Now;
                        toUpdate.Add(detailInListFound.FirstOrDefault());
                    }
                    else
                    {
                        toCreate.Add(detail);
                    }
                });

                //Escribir a BD
                if (toCreate.Any())
                {
                    await middlewareManager.CreateAsync(toCreate, identityWorkID);
                }
                if (toUpdate.Any())
                {
                    await middlewareManager.UpdateAsync(toUpdate, identityWorkID);
                }
            }
        }

        /// <summary>
        /// Calculates the formula by overdraft
        /// </summary>
        /// <param name="calculateByOverdraftIDParams"></param>
        /// <returns></returns>
        public async Task<ICalculateResult> CalculateAsync(ICalculateParams calculateParams)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //result instance
            var calculateResult = new CalculateOverdraftResult();
            var calculateOverdraftParams = calculateParams as CalculateOverdraftParams;

            //Functions params // Para inyectarle a las formulas los datos necesarios para el calculo (1 sola llamada a BD)
            var dataResponse = await GetDataAsync(
                calculateOverdraftParams.OverdraftID,
                calculateOverdraftParams.InstanceID,
                calculateOverdraftParams.IdentityWorkID);

            //Poner en 0 todos los amounts / previous calculation
            dataResponse.Overdraft.OverdraftDetails.ForEach(p =>
            {
                p.Taxed = 0M;
                p.Exempt = 0M;
                p.IMSSTaxed = 0M;
                p.IMSSExempt = 0M;

                if (calculateOverdraftParams.ResetCalculation)
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

            FunctionParams functionParams = new FunctionParams();
            functionParams.CalculationBaseResult = dataResponse;
            functionParams.IdentityWorkID = calculateOverdraftParams.IdentityWorkID;
            functionParams.InstanceID = calculateOverdraftParams.InstanceID;

            //Initializate data, arguments, and functions
            base.Initializate(functionParams);

            //overdraftDetails - All Perceptions, Deductions and Obligations
            List<AccumulatedEmployee> accumulatedEmployees = null;

            //Perceptions
            (var overdraft, var employeeAccumulates) = await CalculateByConceptAsync(dataResponse, ConceptType.SalaryPayment,
                calculateOverdraftParams, null);
            accumulatedEmployees = employeeAccumulates;
            dataResponse.Overdraft = overdraft;

            //Deductions
            (overdraft, employeeAccumulates) = await CalculateByConceptAsync(dataResponse, ConceptType.DeductionPayment,
                calculateOverdraftParams, new ConcurrentBag<AccumulatedEmployee>(accumulatedEmployees));
            accumulatedEmployees = employeeAccumulates;
            dataResponse.Overdraft = overdraft;

            //Liabilities
            //Liability 
            dataResponse.IsLiability = true;
            (overdraft, employeeAccumulates) = await CalculateByConceptAsync(dataResponse, ConceptType.LiabilityPayment,
                calculateOverdraftParams, new ConcurrentBag<AccumulatedEmployee>(accumulatedEmployees));
            dataResponse.Overdraft = overdraft;
            dataResponse.IsLiability = false;

            //Ajuste al neto
            var overdraftFinal = await amountAdjustmentAsync(dataResponse.Overdraft, calculateOverdraftParams, dataResponse);

            //Prepare data to result
            calculateResult.OverdraftResult = overdraftFinal;

            //Afecta el sobrerecibo y sus acumulados del periodo en cálculo
            if (calculateOverdraftParams.SaveOverdraft)
            {
                //save conceptRelations - fonacot, infonavit, etc.
                await saveConceptRelations(functionParams, calculateOverdraftParams, overdraftFinal);
                //save overdraft and details
                await saveOverdraft(overdraftFinal, calculateOverdraftParams);
            }

            stopwatch.Stop();
            Trace.WriteLine($"Time elapsed in the overdraft calculation {stopwatch.Elapsed}");

            //return the result
            return calculateResult;
        }

        /// <summary>
        /// calculate single formula
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        public CalculateResult CalculateFormula(string formula)
        {
            return base.Calculate(formula, true);
        }

        /// <summary>
        /// do the calculation for each overdraft per employee in the new period
        /// </summary>
        /// <param name="newOverdrafts"></param>
        /// <param name="authorizationParams"></param>
        public async Task CalculationFireAndForgetAsync(IEnumerable<Overdraft> overdrafts, Guid identityWorkID, Guid instanceID, Guid userID)
        {
            Task.Run(async () =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                if (overdrafts.Any())
                {
                    await doParallelCalculationAsync(overdrafts, identityWorkID, instanceID, userID);
                }

                stopwatch.Stop();
                Trace.WriteLine($"Time elapsed in the fire and forget calculation {stopwatch.Elapsed} for {overdrafts.Count()} overdrafts");

            });
        }


        /// <summary>
        /// do the calculation for each overdraft per employee in the new period
        /// </summary>
        /// <param name="newOverdrafts"></param>
        /// <param name="authorizationParams"></param>
        public async Task CalculationFireAndForgetAsync(IEnumerable<Guid> overdraftsIds, Guid identityWorkID, Guid instanceID, Guid userID)
        {
            Task.Run(async () =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                if (overdraftsIds.Any())
                {
                    await doParallelCalculationAsync(overdraftsIds, identityWorkID, instanceID, userID);
                }

                stopwatch.Stop();
                Trace.WriteLine($"Time elapsed in the fire and forget calculation {stopwatch.Elapsed} for {overdraftsIds.Count()} overdrafts");

            });
        }

        /// <summary>
        /// do the calculation for each overdraft per employee in the new period by employees Ids
        /// </summary>
        /// <param name="employeeIds"></param>
        /// <param name="identityWorkID"></param>
        /// <param name="instanceID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task CalculationFireAndForgetByEmployeesAsync(IEnumerable<Guid> employeeIds, Guid identityWorkID, Guid instanceID, Guid userID)
        {
            Task.Run(async () =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                //Get overdrafts to do calculation
                var middlewareOverdraft = new MiddlewareManager<Overdraft>(
                new BaseRecordManager<Overdraft>(), new OverdraftValidator());

                var overdrafts = await middlewareOverdraft.FindByExpressionAsync(p =>
                        p.InstanceID == instanceID &&
                        employeeIds.Contains(p.EmployeeID) &&
                        p.OverdraftStatus == OverdraftStatus.None
                    , identityWorkID);

                await doParallelCalculationAsync(overdrafts, identityWorkID, instanceID, userID);

                stopwatch.Stop();
                Trace.WriteLine($"Time elapsed in the fire and forget calculation {stopwatch.Elapsed} for {overdrafts.Count()} overdrafts");

            });
        }


        /// <summary>
        /// do the calculation for each overdraft per employee in the new period by employees Ids
        /// </summary>
        /// <param name="employeeIds"></param>
        /// <param name="identityWorkID"></param>
        /// <param name="instanceID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task CalculationByEmployeesAsync(IEnumerable<Guid> employeeIds, Guid identityWorkID, Guid instanceID, Guid userID)
        {

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //Get overdrafts to do calculation
            var middlewareOverdraft = new MiddlewareManager<Overdraft>(
            new BaseRecordManager<Overdraft>(), new OverdraftValidator());

            var overdrafts = await middlewareOverdraft.FindByExpressionAsync(p =>
                    p.InstanceID == instanceID &&
                    employeeIds.Contains(p.EmployeeID) &&
                    p.OverdraftStatus == OverdraftStatus.None
                , identityWorkID);

            await doCalculationAsync(overdrafts, identityWorkID, instanceID, userID);

            stopwatch.Stop();
            Trace.WriteLine($"Time elapsed in calculation by employees {stopwatch.Elapsed} for {overdrafts.Count()} overdrafts");

        }


        /// <summary>
        /// do the calculation for each overdraft per employee in the new period by employees Ids
        /// </summary>
        /// <param name="employeeIds"></param>
        /// <param name="identityWorkID"></param>
        /// <param name="instanceID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task CalculationFireAndForgetByPeriodIdsAsync(IEnumerable<Guid> periodIds, Guid identityWorkID, Guid instanceID, Guid userID)
        {
            Task.Run(async () =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                //Get overdrafts to do calculation
                var middlewareOverdraft = new MiddlewareManager<Overdraft>(
                new BaseRecordManager<Overdraft>(), new OverdraftValidator());

                var overdrafts = await middlewareOverdraft.FindByExpressionAsync(p =>
                    p.InstanceID == instanceID &&
                    periodIds.Contains(p.PeriodDetail.PeriodID) &&
                    p.PeriodDetail.PeriodStatus == PeriodStatus.Calculating &&
                    p.OverdraftStatus == OverdraftStatus.None
                , identityWorkID, new string[] { "PeriodDetail" });

                await doParallelCalculationAsync(overdrafts, identityWorkID, instanceID, userID);

                stopwatch.Stop();
                Trace.WriteLine($"Time elapsed in the fire and forget calculation {stopwatch.Elapsed} for {overdrafts.Count()} overdrafts");

            });
        }

    }
}
