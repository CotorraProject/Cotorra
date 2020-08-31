using Cotorra.Core;
using Cotorra.Core.Managers;
using Cotorra.Core.Managers.Calculation;
using Cotorra.Core.Utils;
using Cotorra.Core.Validator;
using Cotorra.ExpressionParser.Core.Cotorra;
using Cotorra.Schema;
using Cotorra.Schema.Calculation;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Cotorra.UnitTest.Manager.calculation
{
    public class CalculationManagerUT
    {
        public class OverdraftTest
        {
            [Fact]
            public async Task TM_CalculateByOverdraft()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkId, instanceID);

                ICalculateParams calculateByOverdraftParams = new Schema.Calculation.CalculateOverdraftParams()
                {
                    OverdraftID = overdraft.ID,
                    IdentityWorkID = identityWorkId,
                    InstanceID = instanceID
                };

                //Act
                ICalculationManager overdraftCalculationManager = new OverdraftCalculationManager();
                var calculateResult = await overdraftCalculationManager.CalculateAsync(calculateByOverdraftParams);
                var calculateOverdraftResult = calculateResult as CalculateOverdraftResult;

                //Assert
                //SALARIO
                var sumSalary = calculateOverdraftResult
                   .OverdraftResult
                   .OverdraftDetails
                       .Where(p => p.ConceptPayment.Code == 1 &&
                                   p.ConceptPayment.ConceptType == ConceptType.SalaryPayment)
                       .Sum(p => p.Amount);
                Assert.True(sumSalary == 32256.75M);

                //ISR del mes
                var isrMesDetail = calculateOverdraftResult
                   .OverdraftResult
                   .OverdraftDetails
                       .Where(p => p.ConceptPayment.Code == 45 &&
                                   p.ConceptPayment.ConceptType == ConceptType.DeductionPayment);

                var isrSum = isrMesDetail.Sum(p => p.Amount);
                Assert.True(isrSum == 16543.9542434211M);

                //IMSS
                var imssOverdraftDetail = calculateOverdraftResult
                   .OverdraftResult
                   .OverdraftDetails
                       .Where(p => p.ConceptPayment.Code == 52 &&
                                   p.ConceptPayment.ConceptType == ConceptType.DeductionPayment);

                var imssSum = imssOverdraftDetail.Sum(p => p.Amount);

                //autorizacion de la nómina
                var periodDetailID = overdraft.PeriodDetailID;
                overdraft = calculateOverdraftResult.OverdraftResult;

                //Autorización de la nómina
                var authorizationManager = new AuthorizationManager();
                var authorizationParams = new AuthorizationParams()
                {
                    IdentityWorkID = identityWorkId,
                    InstanceID = instanceID,
                    PeriodDetailIDToAuthorize = periodDetailID,
                    ResourceID = Guid.Empty,
                    user = Guid.Empty
                };
                var historicOverdrafts = await authorizationManager.AuthorizationAsync(authorizationParams);

                var overdraftManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdraftsPrevious = await overdraftManager.FindByExpressionAsync(p => p.PeriodDetailID == periodDetailID, identityWorkId);

                //Timbrado
                var manager = new PayrollStampingManager();
                var dateTime = DateTime.Now;
                var stampingParms = new PayrollStampingParams()
                {
                    FiscalStampingVersion = FiscalStampingVersion.CFDI33_Nom12,
                    IdentityWorkID = identityWorkId,
                    InstanceID = instanceID,
                    PeriodDetailID = periodDetailID,
                    Detail = new List<PayrollStampingDetail>()
                    {
                        new PayrollStampingDetail()
                        {
                            Folio = "2020",
                            Series = "S1",
                            PaymentDate = dateTime.AddDays(-2),
                            RFCOriginEmployer = null,
                            SNCFEntity = null, 
                            OverdraftID = overdraftsPrevious.FirstOrDefault().ID
                        } 
                    },

                    Currency = Currency.MXN
                };

                var payrollStampingResult = await manager.PayrollStampingAsync(stampingParms);
                Assert.Contains(payrollStampingResult.PayrollStampingResultDetails, p => p.PayrollStampingResultStatus == PayrollStampingResultStatus.Success);

                //Transform to HTML and PDF

                var fiscalPreviewManager = new FiscalPreviewerManager();
                var previewTransformParams = new PreviewTransformParams();
                previewTransformParams.FiscalStampingVersion = FiscalStampingVersion.CFDI33_Nom12;
                previewTransformParams.InstanceID = instanceID;
                previewTransformParams.IdentityWorkID = identityWorkId;
                previewTransformParams.PreviewTransformParamsDetails.Add(new PreviewTransformParamsDetail()
                {
                    OverdraftID = historicOverdrafts.FirstOrDefault().ID
                });

                var previewTransformResult = await fiscalPreviewManager.TransformAsync(previewTransformParams);

                var outFilePath = Path.Combine(DirectoryUtil.AssemblyDirectory, "example.html");
                await File.WriteAllTextAsync(outFilePath, previewTransformResult.PreviewTransformResultDetails.FirstOrDefault().TransformHTMLResult);

                var pdfFile = previewTransformResult.PreviewTransformResultDetails.FirstOrDefault().TransformPDFResult;
                var outPDFFilePath = Path.Combine(DirectoryUtil.AssemblyDirectory, "example.pdf");
                await File.WriteAllBytesAsync(outPDFFilePath, pdfFile);

                Assert.True(File.Exists(outFilePath));

            }
        }

        public class AAA
        {
            /// <summary>
            /// Valida la fórmula VPeriodoDeVacaciones, si el trabajador estuvo todo el periodo de 
            /// vacaciones regresa 1, de lo contrario 0
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VPeriodoDeVacaciones_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VPeriodoDeVacaciones"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0);

                //create vacations for all the period
                await new VacationManagerUT().CreateDefaultAsync(identityWorkID, instanceID, overdraft.EmployeeID,
                    overdraft.PeriodDetail.InitialDate, overdraft.PeriodDetail.FinalDate);

                //validate, all the days was in vacations
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 1);
            }
        }

        public class WithoutCategory
        {
            /// <summary>
            /// Valida la fórmula VPeriodoDeVacaciones, si el trabajador estuvo todo el periodo de 
            /// vacaciones regresa 1, de lo contrario 0
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VPeriodoDeVacaciones_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VPeriodoDeVacaciones"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0);

                //create vacations for all the period
                await new VacationManagerUT().CreateDefaultAsync(identityWorkID, instanceID, overdraft.EmployeeID,
                    overdraft.PeriodDetail.InitialDate, overdraft.PeriodDetail.FinalDate);

                //validate, all the days was in vacations
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 1);
            }

            /// <summary>
            /// Validate la fórmula VDiasDerechoSueldoAnterior, si el trabajador tuvo días del periodo con 
            /// un salario diferente al actual, regresa el número de días con el salario anterior.
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VDiasDerechoSueldoAnterior_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VDiasDerechoSueldoAnterior"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0);

                //arrange
                var employeeID = overdraft.EmployeeID;
                var days = 3;
                var salaryAdjustment = 5487;

                //Update Daily Salary of employee to generate the HistoricEmployeeSalaryAdjustment
                await new EmployeeManagerUT().UpdateEmployeeDailySalary(overdraft.Employee, salaryAdjustment, identityWorkID);

                //Update Modification Date to be inside of the period
                var modificationDate = overdraft.PeriodDetail.InitialDate.AddDays(days);
                await new HistoricEmployeeSalaryAdjustmentManagerUT().UpdateModificationDateAsync(employeeID, modificationDate, identityWorkID);

                //Assert
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == days);
            }

            /// <summary>
            /// Valida la fórmula VSalDiarioAnt, si el trabajador tuvo días del periodo con 
            /// un salario diferente al actual, regresa el salario anterior.
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VSalDiarioAnt_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VSalDiarioAnt"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0);

                //arrange
                var employeeID = overdraft.EmployeeID;
                var days = 3;
                var salaryAdjustment = 5487;
                var oldSalary = overdraft.Employee.DailySalary;

                //Update Daily Salary of employee to generate the HistoricEmployeeSalaryAdjustment
                await new EmployeeManagerUT().UpdateEmployeeDailySalary(overdraft.Employee, salaryAdjustment, identityWorkID);

                //Update Modification Date to be inside of the period
                var modificationDate = overdraft.PeriodDetail.InitialDate.AddDays(days);
                await new HistoricEmployeeSalaryAdjustmentManagerUT().UpdateModificationDateAsync(employeeID, modificationDate, identityWorkID);

                //Assert
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == oldSalary);
            }

            /// <summary>
            /// Valida la fórmula VDiasDerechoSueldoVigente, si el trabajador tuvo días del periodo con
            /// un salario diferente al actual, regresa los días con el nuevo salario.
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VDiasDerechoSueldoVigente_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VDiasDerechoSueldoVigente"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == overdraft.PeriodDetail.PaymentDays);

                //arrange
                var employeeID = overdraft.EmployeeID;
                var days = 3;
                var salaryAdjustment = 5487;
                var oldSalary = overdraft.Employee.DailySalary;

                //Update Daily Salary of employee to generate the HistoricEmployeeSalaryAdjustment
                await new EmployeeManagerUT().UpdateEmployeeDailySalary(overdraft.Employee, salaryAdjustment, identityWorkID);

                //Update Modification Date to be inside of the period
                var modificationDate = overdraft.PeriodDetail.InitialDate.AddDays(days);
                await new HistoricEmployeeSalaryAdjustmentManagerUT().UpdateModificationDateAsync(employeeID, modificationDate, identityWorkID);

                //Assert
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == (overdraft.PeriodDetail.PaymentDays - days));
            }

            /// <summary>
            /// Valida la fórmula VSalDiarioAnt, si el trabajador tuvo días del periodo con 
            /// un salario diferente al actual, regresa el salario anterior.
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VSalDiarioVigente_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VSalDiarioVigente"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == overdraft.Employee.DailySalary);

                //arrange
                var employeeID = overdraft.EmployeeID;
                var days = 3;
                var salaryAdjustment = 5487;
                var oldSalary = overdraft.Employee.DailySalary;

                //Update Daily Salary of employee to generate the HistoricEmployeeSalaryAdjustment
                await new EmployeeManagerUT().UpdateEmployeeDailySalary(overdraft.Employee, salaryAdjustment, identityWorkID);

                //Update Modification Date to be inside of the period
                var modificationDate = overdraft.PeriodDetail.InitialDate.AddDays(days);
                await new HistoricEmployeeSalaryAdjustmentManagerUT().UpdateModificationDateAsync(employeeID, modificationDate, identityWorkID);

                //Assert
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == salaryAdjustment);
            }

            /// <summary>
            /// Valida el cálculo de retardos (horas)
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VImpRetardos_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
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
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VSalarioXhoraVig"
                };

                //Do the caltulation VSalarioXhoraVig
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 268.80625M);

                calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VSalarioXhoraAnt"
                };

                //Do the caltulation VSalarioXhoraAnt
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0m);

                calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VImpRetardos"
                };

                //Do the caltulation VSalarioXhoraAnt
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0m);

                //arrange
                var employeeID = overdraft.EmployeeID;
                var days = 3;
                var salaryAdjustment = 5487m;
                var oldSalary = overdraft.Employee.DailySalary;
                var periodDetailID = overdraft.PeriodDetailID;
                var perhours = Convert.ToDecimal(overdraft.Employee.Workshift.Hours);

                //Update Daily Salary of employee to generate the HistoricEmployeeSalaryAdjustment
                await new EmployeeManagerUT().UpdateEmployeeDailySalary(overdraft.Employee, salaryAdjustment, identityWorkID);

                //Update Modification Date to be inside of the period
                var modificationDate = overdraft.PeriodDetail.InitialDate.AddDays(days);
                await new HistoricEmployeeSalaryAdjustmentManagerUT().UpdateModificationDateAsync(employeeID, modificationDate, identityWorkID);

                //Create Incidents
                var incidentDate = overdraft.PeriodDetail.InitialDate.AddDays(days - 1);
                var salaryRigth = false;
                var incidents = await new IncidentManagerUT().CreateDefaultAsyncWithPeriodDetailID<Incident>(identityWorkID,
                    instanceID, employeeID, periodDetailID, salaryRigth, incidentDate);

                //Re calculate
                //(VHorasRetVig * VSalarioXhoraVig) + (VHorasRetAnt * VSalarioXhoraAnt)
                var res = (0 * (salaryAdjustment / perhours)) + (incidents.FirstOrDefault().Value * (oldSalary / perhours));
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == res);
            }

            /// <summary>
            /// VDiasPeriodo
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VDiasdePeriodo_Formula()
            {
                //VDiasdePeriodo
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
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
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VDiasdePeriodo"
                };

                //Do the caltulation VDiasdePeriodo
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == overdraft.PeriodDetail.PaymentDays);
            }

            /// <summary>
            /// Valida la fórmula VPago_SueldoFin, 
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VPago_SueldoFin_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
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
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VSalDiarioVigente"
                };

                //Do the caltulation TipoProceso()
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == overdraft.Employee.DailySalary);

                calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "IIF(TipoProceso() = 70 , 1 , 0)"
                };

                //Do the caltulation TipoProceso()
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0m);

                //VPago_SueldoFin
                calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VPago_SueldoFin"
                };

                //Do the caltulation VPago_SueldoFin
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0m);
            }

            /// <summary>
            /// TopeEnfGeneral
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_TopeEnfGeneral_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
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
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "UMA"
                };

                //Do the caltulation UMA
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 86.88M);
                var uma = result.Result;

                calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "TopeEnfGeneral"
                };

                //Do the caltulation TopeEnfGeneral
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == uma * 25M);
            }

            /// <summary>
            /// SalarioZonaA
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_SalariosMinimos_ZonaA()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
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
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "SalarioMinimoDF"
                };

                //Do the caltulation SalariosMinimos_ZonaA
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 123.22M);

            }

            [Fact]
            public async Task TM_Expression()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = null,
                    Formula = "3+2"
                };

                var result = await calculationManager.CalculateAsync(calculateGenericParams);

                Assert.True((result as CalculateGenericResult).Result == 5);

            }

            [Fact]
            public async Task TM_RoundTo()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = null,
                    Formula = "ROUNDTO(23.4563, 2)"
                };

                var result = await calculationManager.CalculateAsync(calculateGenericParams);
                Assert.True((result as CalculateGenericResult).Result == 23.46M);

            }

            /// <summary>
            /// SalarioMinimo
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_SalariosMinimos_MinimumZone()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
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
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "SalarioMinimo"
                };

                //Do the caltulation SalariosMinimos_ZonaA
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 123.22M);

            }

            /// <summary>
            /// VSalarioPrestaciones
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VSalarioPrestaciones()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
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
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VSalarioPrestaciones"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == overdraft.Employee.DailySalary);
            }

            /// <summary>
            /// VSalarioPrestaciones
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_UMAValor_UMA()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.PeriodDetailID == p.PeriodDetail.ID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.Workshift",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "UMA"
                };

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;

                stopwatch.Stop();
                Trace.WriteLine($"UMA Unit Test calculation elapsed time: {stopwatch.Elapsed}");

                Assert.True(result.Result == 86.88M);
            }

            /// <summary>
            /// VSeptimos_Dias
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VSeptimos_Dias_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
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
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "MIN(2, 3)"
                };

                //Do the caltulation MIN
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 2);

                calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "MAX(2, 3)"
                };

                //Do the caltulation MAX
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 3);

                calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "FRAC(2.33)"
                };

                //Do the caltulation FRAC
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0.33m);

                calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VSeptimos_Dias"
                };

                //Do the caltulation MAX
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0);
            }
        }

        public class IMSS
        {
            [Fact]
            public async Task Should_Calculate_SalarioMinimoZonadelEmpleado()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
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
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "SalarioMinimoZonadelEmpleado"
                };

                //MinimunSalary
                var minimunSalaryManager = new MiddlewareManager<MinimunSalary>(
                    new BaseRecordManager<MinimunSalary>(), new MinimunSalaryValidator());
                var minimumSalaries = await minimunSalaryManager.FindByExpressionAsync(p =>
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.ExpirationDate.Year == DateTime.UtcNow.Year &&
                    p.Active,
                    identityWorkID);

                //Do the caltulation SalariosMinimos_ZonaA
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == minimumSalaries.FirstOrDefault().ZoneA);
            }

            /// <summary>
            /// Validate la fórmula VDiasIMSSAnterior, si el trabajador tuvo días del periodo con 
            /// un salario diferente al actual, regresa el número de días con el salario anterior.
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VDiasIMSSAnterior_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VDiasIMSSAnterior"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0);

                //arrange
                var employeeID = overdraft.EmployeeID;
                var days = 3;
                var salaryAdjustment = 5487;

                //Update Daily Salary of employee to generate the HistoricEmployeeSalaryAdjustment
                await new EmployeeManagerUT().UpdateEmployeeDailySalary(overdraft.Employee, salaryAdjustment, identityWorkID);

                //Update Modification Date to be inside of the period
                var modificationDate = overdraft.PeriodDetail.InitialDate.AddDays(days);
                await new HistoricEmployeeSalaryAdjustmentManagerUT().UpdateModificationDateAsync(employeeID, modificationDate, identityWorkID);

                //Assert
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == days);
            }

            /// <summary>
            /// Valida la fórmula VDiasIMSSVigente, si el trabajador tuvo días del periodo con
            /// un salario diferente al actual, regresa los días con el nuevo salario.
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VDiasIMSSVigente_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VDiasIMSSVigente"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == overdraft.PeriodDetail.PaymentDays);

                //arrange
                var employeeID = overdraft.EmployeeID;
                var days = 3;
                var salaryAdjustment = 5487;
                var oldSalary = overdraft.Employee.DailySalary;

                //Update Daily Salary of employee to generate the HistoricEmployeeSalaryAdjustment
                await new EmployeeManagerUT().UpdateEmployeeDailySalary(overdraft.Employee, salaryAdjustment, identityWorkID);

                //Update Modification Date to be inside of the period
                var modificationDate = overdraft.PeriodDetail.InitialDate.AddDays(days);
                await new HistoricEmployeeSalaryAdjustmentManagerUT().UpdateModificationDateAsync(employeeID, modificationDate, identityWorkID);

                //Assert
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == (overdraft.PeriodDetail.PaymentDays - days));
            }

            /// <summary>
            /// Valida la fórmula VAusentismoIMSSAnterior
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VAusentismoIMSSAnterior_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
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
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VAusentismoIMSSAnterior"
                };

                //Do the caltulation VSalarioXhoraVig
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);

                //arrange
                var employeeID = overdraft.EmployeeID;
                var days = 3;
                var salaryAdjustment = 5487m;
                var oldSalary = overdraft.Employee.DailySalary;
                var periodDetailID = overdraft.PeriodDetailID;
                var perhours = Convert.ToDecimal(overdraft.Employee.Workshift.Hours);

                //Update Daily Salary of employee to generate the HistoricEmployeeSalaryAdjustment
                await new EmployeeManagerUT().UpdateEmployeeDailySalary(overdraft.Employee, salaryAdjustment, identityWorkID);

                //Update Modification Date to be inside of the period
                var modificationDate = overdraft.PeriodDetail.InitialDate.AddDays(days);
                await new HistoricEmployeeSalaryAdjustmentManagerUT().UpdateModificationDateAsync(employeeID, modificationDate, identityWorkID);

                //Create Incidents
                var incidentDate = overdraft.PeriodDetail.InitialDate.AddDays(days - 1);
                var salaryRigth = false;
                var incidents = await new IncidentManagerUT().CreateDefaultAsyncWithPeriodDetailID<Incident>(identityWorkID,
                    instanceID, employeeID, periodDetailID, salaryRigth, incidentDate);

                //Re calculate 2 hours de retardo sin derecho a sueldo
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0.25M);
            }

            /// <summary>
            /// Valida la fórmula VAusentismoIMSSVigente
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VAusentismoIMSSVigente_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
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
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VAusentismoIMSSVigente"
                };

                //Do the caltulation VSalarioXhoraVig
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);

                //arrange
                var employeeID = overdraft.EmployeeID;
                var days = 3;
                var salaryAdjustment = 5487m;
                var oldSalary = overdraft.Employee.DailySalary;
                var periodDetailID = overdraft.PeriodDetailID;
                var perhours = Convert.ToDecimal(overdraft.Employee.Workshift.Hours);

                //Update Daily Salary of employee to generate the HistoricEmployeeSalaryAdjustment
                await new EmployeeManagerUT().UpdateEmployeeDailySalary(overdraft.Employee, salaryAdjustment, identityWorkID);

                //Update Modification Date to be inside of the period
                var modificationDate = overdraft.PeriodDetail.InitialDate.AddDays(days);
                await new HistoricEmployeeSalaryAdjustmentManagerUT().UpdateModificationDateAsync(employeeID, modificationDate, identityWorkID);

                //Create Incidents
                var incidentDate = overdraft.PeriodDetail.InitialDate.AddDays(days + 1);
                var salaryRigth = false;
                var incidents = await new IncidentManagerUT().CreateDefaultAsyncWithPeriodDetailID<Incident>(identityWorkID,
                    instanceID, employeeID, periodDetailID, salaryRigth, incidentDate);

                //Re calculate 2 hours de retardo sin derecho a sueldo
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0.25M);
            }

            /// <summary>
            /// VIncapacidadesIMSSAnterior
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VIncapacidadesIMSSAnterior_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
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
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VIncapacidadesIMSSAnterior"
                };

                //Do the caltulation VSalarioXhoraVig
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);

                //arrange
                var employeeID = overdraft.EmployeeID;
                var days = 3;
                var salaryAdjustment = 5487m;
                var oldSalary = overdraft.Employee.DailySalary;
                var periodDetailID = overdraft.PeriodDetailID;
                var perhours = Convert.ToDecimal(overdraft.Employee.Workshift.Hours);

                //Update Daily Salary of employee to generate the HistoricEmployeeSalaryAdjustment
                await new EmployeeManagerUT().UpdateEmployeeDailySalary(overdraft.Employee, salaryAdjustment, identityWorkID);

                //Update Modification Date to be inside of the period
                var modificationDate = overdraft.PeriodDetail.InitialDate.AddDays(days);
                await new HistoricEmployeeSalaryAdjustmentManagerUT().UpdateModificationDateAsync(employeeID, modificationDate, identityWorkID);

                //Create Incapacities
                var inhabilityDate = overdraft.PeriodDetail.InitialDate.AddDays(days - 1);
                var inhabilities = await new InhabilityManagerUT().CreateDefaultAsyncWithDate<Inhability>(identityWorkID,
                    instanceID, employeeID, inhabilityDate);

                //Re calculate 2 hours de retardo sin derecho a sueldo
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 1M);
            }

            /// <summary>
            /// VIncapacidadesIMSSVigente
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VIncapacidadesIMSSVigente_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
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
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VIncapacidadesIMSSVigente"
                };

                //Do the caltulation VSalarioXhoraVig
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);

                //arrange
                var employeeID = overdraft.EmployeeID;
                var days = 3;
                var salaryAdjustment = 5487m;
                var oldSalary = overdraft.Employee.DailySalary;
                var periodDetailID = overdraft.PeriodDetailID;
                var perhours = Convert.ToDecimal(overdraft.Employee.Workshift.Hours);

                //Update Daily Salary of employee to generate the HistoricEmployeeSalaryAdjustment
                await new EmployeeManagerUT().UpdateEmployeeDailySalary(overdraft.Employee, salaryAdjustment, identityWorkID);

                //Update Modification Date to be inside of the period
                var modificationDate = overdraft.PeriodDetail.InitialDate.AddDays(days);
                await new HistoricEmployeeSalaryAdjustmentManagerUT().UpdateModificationDateAsync(employeeID, modificationDate, identityWorkID);

                //Create Incapacities
                var inhabilityDate = overdraft.PeriodDetail.InitialDate.AddDays(days + 1);
                var inhabilities = await new InhabilityManagerUT().CreateDefaultAsyncWithDate<Inhability>(identityWorkID,
                    instanceID, employeeID, inhabilityDate);

                //Re calculate 2 hours de retardo sin derecho a sueldo
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 11M);
            }

            /// <summary>
            /// VSBCAnterior
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VSBCAnterior_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
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
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VSBCAnterior"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);

                //arrange
                var employeeID = overdraft.EmployeeID;
                var days = 3;
                var salaryAdjustment = 5487m;
                var newSBC = 342m;
                var oldSBC = overdraft.Employee.SBCMax25UMA;

                //Update Daily Salary of employee to generate the HistoricEmployeeSalaryAdjustment
                await new EmployeeManagerUT().UpdateEmployeeDailySalaryAndSBC(overdraft.Employee, salaryAdjustment, newSBC, identityWorkID);

                //Update Modification Date to be inside of the period
                var modificationDate = overdraft.PeriodDetail.InitialDate.AddDays(days);
                await new HistoricEmployeeSalaryAdjustmentManagerUT().UpdateModificationDateAsync(employeeID, modificationDate, identityWorkID);

                //Re calculate 2 hours de retardo sin derecho a sueldo
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == oldSBC);
            }

            /// <summary>
            /// VSBCVigente
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VSBCVigente_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
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
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VSBCVigente"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == overdraft.Employee.SBCMax25UMA);

                //arrange
                var employeeID = overdraft.EmployeeID;
                var days = 3;
                var salaryAdjustment = 5487m;
                var newSBC = 342.05m;
                var oldSBC = overdraft.Employee.SBCMax25UMA;

                //Update Daily Salary of employee to generate the HistoricEmployeeSalaryAdjustment
                await new EmployeeManagerUT().UpdateEmployeeDailySalaryAndSBC(overdraft.Employee, salaryAdjustment, newSBC, identityWorkID);

                //Update Modification Date to be inside of the period
                var modificationDate = overdraft.PeriodDetail.InitialDate.AddDays(days);
                await new HistoricEmployeeSalaryAdjustmentManagerUT().UpdateModificationDateAsync(employeeID, modificationDate, identityWorkID);

                //Re calculate 2 hours de retardo sin derecho a sueldo
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == newSBC);
            }

            /// <summary>
            /// Valida la fórmula VSalCuotaDiariaIMSSAnt, si el trabajador tuvo días del periodo con 
            /// un salario diferente al actual, regresa el salario anterior.
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VSalCuotaDiariaIMSSAnt_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VSalCuotaDiariaIMSSAnt"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0);

                //arrange
                var employeeID = overdraft.EmployeeID;
                var days = 3;
                var salaryAdjustment = 5487;
                var oldSalary = overdraft.Employee.DailySalary;

                //Update Daily Salary of employee to generate the HistoricEmployeeSalaryAdjustment
                await new EmployeeManagerUT().UpdateEmployeeDailySalary(overdraft.Employee, salaryAdjustment, identityWorkID);

                //Update Modification Date to be inside of the period
                var modificationDate = overdraft.PeriodDetail.InitialDate.AddDays(days);
                await new HistoricEmployeeSalaryAdjustmentManagerUT().UpdateModificationDateAsync(employeeID, modificationDate, identityWorkID);

                //Assert
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == oldSalary);
            }

            /// <summary>
            /// Valida la fórmula VSalCuotaDiariaIMSSVig, si el trabajador tuvo días del periodo con 
            /// un salario diferente al actual, regresa el salario anterior.
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VSalCuotaDiariaIMSSVig_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VSalCuotaDiariaIMSSVig"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == overdraft.Employee.DailySalary);

                //arrange
                var employeeID = overdraft.EmployeeID;
                var days = 3;
                var salaryAdjustment = 5487;
                var oldSalary = overdraft.Employee.DailySalary;

                //Update Daily Salary of employee to generate the HistoricEmployeeSalaryAdjustment
                await new EmployeeManagerUT().UpdateEmployeeDailySalary(overdraft.Employee, salaryAdjustment, identityWorkID);

                //Update Modification Date to be inside of the period
                var modificationDate = overdraft.PeriodDetail.InitialDate.AddDays(days);
                await new HistoricEmployeeSalaryAdjustmentManagerUT().UpdateModificationDateAsync(employeeID, modificationDate, identityWorkID);

                //Assert
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == salaryAdjustment);
            }

            /// <summary>
            /// VIncidencias
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VIncidencias_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
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
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VIncidencias"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);

                //arrange
                var employeeID = overdraft.EmployeeID;
                var days = 3;
                var salaryAdjustment = 5487m;
                var oldSalary = overdraft.Employee.DailySalary;
                var periodDetailID = overdraft.PeriodDetailID;
                var perhours = Convert.ToDecimal(overdraft.Employee.Workshift.Hours);

                //Update Daily Salary of employee to generate the HistoricEmployeeSalaryAdjustment
                await new EmployeeManagerUT().UpdateEmployeeDailySalary(overdraft.Employee, salaryAdjustment, identityWorkID);

                //Update Modification Date to be inside of the period
                var modificationDate = overdraft.PeriodDetail.InitialDate.AddDays(days);
                await new HistoricEmployeeSalaryAdjustmentManagerUT().UpdateModificationDateAsync(employeeID, modificationDate, identityWorkID);

                //Create Incidents
                var incidentDate = overdraft.PeriodDetail.InitialDate.AddDays(days + 1);
                var salaryRigth = false;
                var incidents = await new IncidentManagerUT().CreateDefaultAsyncWithPeriodDetailID<Incident>(identityWorkID,
                    instanceID, employeeID, periodDetailID, salaryRigth, incidentDate);

                //Create Incapacities
                var inhabilityDate = overdraft.PeriodDetail.InitialDate.AddDays(days - 1);
                var inhabilities = await new InhabilityManagerUT().CreateDefaultAsyncWithDate<Inhability>(identityWorkID,
                    instanceID, employeeID, inhabilityDate);

                //Re calculate 2 hours de retardo sin derecho a sueldo
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 1.25M);
            }

            /// <summary>
            /// FactorDescINFONAVIT
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_FactorDescINFONAVIT_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
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
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "FactorDescINFONAVIT"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);

                //create infonavitMovements
                var days = 3;
                var modificationDate = overdraft.PeriodDetail.InitialDate.AddDays(days);
                var infonavitMovements = await new InfonavitMovementManagerUT().CreateDefaultAsync<InfonavitMovement>(identityWorkID,
                    instanceID, overdraft.EmployeeID, 0.263M, modificationDate);

                //Do the caltulation
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == infonavitMovements.FirstOrDefault().MonthlyFactor);
            }
        }

        public class IMSSTables
        {
            /// <summary>
            /// Valida la fórmula DiasVigenteInvalidezyVida
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_DiasVigenteInvalidezyVida_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "DiasVigenteInvalidezyVida"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 15);
            }

            /// <summary>
            /// Valida la fórmula CuotaObreroEG4
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_CuotaObreroEG4_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "CuotaObreroEG4"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0.02M);
            }

            /// <summary>
            /// Valida la fórmula TopeGuarderias
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_TopeGuarderias_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "TopeGuarderias"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 2112.25M);
            }

            /// <summary>
            /// Valida la fórmula CuotaGuarderias7
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_CuotaGuarderias7_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "CuotaGuarderias7"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0.075M);
            }

            /// <summary>
            /// Valida la fórmula CuotaRetiro8
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_CuotaRetiro8_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "CuotaRetiro8"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0.15M);
            }

            /// <summary>
            /// Valida la fórmula Riesgo_trabajo
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_Riesgo_trabajo_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "Riesgo_trabajo()"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0.06M);
            }
        }

        public class Vacations
        {
            /// <summary>
            /// Valida la fórmula VDiasVacaciones
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VDiasVacaciones_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VDiasVacaciones"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0);

                var vacationDays = 2;
                //create vacations for all the period
                await new VacationManagerUT().CreateDefaultAsync(identityWorkID, instanceID, overdraft.EmployeeID,
                    overdraft.PeriodDetail.InitialDate.AddDays(-1 * vacationDays), overdraft.PeriodDetail.InitialDate.AddDays(1));

                //validate, all the days was in vacations
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == (vacationDays + 2));
            }

            /// <summary>
            /// Valida la fórmula VDiasVacacionesPeriodo
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VDiasVacacionesPeriodo_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VDiasVacacionesPeriodo"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0);

                var vacationDays = 2;
                //create vacations for a part of period, starting in other period and finalize in the current period
                await new VacationManagerUT().CreateDefaultAsync(identityWorkID, instanceID, overdraft.EmployeeID,
                    overdraft.PeriodDetail.InitialDate.AddDays(-1 * vacationDays), overdraft.PeriodDetail.InitialDate.AddDays(1));

                //validate, all the days was in vacations
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == (vacationDays));
            }

            /// <summary>
            /// Valida la fórmula VDiasXVacaciones
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VDiasXVacaciones_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VDiasXVacaciones"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0);

                var vacationDays = 2;
                //create vacations for a part of period, starting in other period and finalize in the current period
                await new VacationManagerUT().CreateDefaultAsync(identityWorkID, instanceID, overdraft.EmployeeID,
                    overdraft.PeriodDetail.InitialDate.AddDays(-1 * vacationDays), overdraft.PeriodDetail.InitialDate.AddDays(1));

                //validate, all the days was in vacations
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == (vacationDays));
            }

            /// <summary>
            /// VSeptimos_Dias
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VSeptimos_Dias_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VSeptimos_Dias"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0);

                var vacationDays = 2;
                //create vacations for a part of period, starting in other period and finalize in the current period
                await new VacationManagerUT().CreateDefaultAsync(identityWorkID, instanceID, overdraft.EmployeeID,
                    overdraft.PeriodDetail.InitialDate.AddDays(-1 * vacationDays), overdraft.PeriodDetail.InitialDate.AddDays(1));

                //validate, all the days was in vacations
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0);
            }
        }

        public class ISPT_Articulo_142
        {
            /// <summary>
            /// Valida la fórmula VArt142_BaseGravada
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VArt142_BaseGravada_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VArt142_BaseGravada"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0);
            }

            /// <summary>
            /// Valida la fórmula VArt142_Aplica_Subs
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VArt142_Aplica_Subs_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VArt142_Aplica_Subs"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0);
            }

            /// <summary>
            /// Valida la fórmula VArt142_CuotaDiaria
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VArt142_CuotaDiaria_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VArt142_CuotaDiaria"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 2150.45M);
            }

            /// <summary>
            /// Valida la fórmula TVigISRMensual.Limite_inferior
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_TVigISRMensual_Limite_inferior_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "TVigISRMensual.Limite_inferior{125}"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0.01M);
            }

            ////// <summary>
            /// Valida la fórmula VArt142_2IM
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VArt142_2IM_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VArt142_2IM"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 7900.74M);
            }

            ////// <summary>
            /// Valida la fórmula VArt142_3CF
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VArt142_3CF_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VArt142_3CF"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 7162.74M);
            }

            ////// <summary>
            /// Valida la fórmula Vart142_5ISPToSE
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_Vart142_5ISPToSE_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "Vart142_5ISPToSE"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 15063.48M);

                calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "TVigSubEmpMensual.Subs_al_empleo{150}"
                };

                //Do the caltulation
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 407.02M);
            }

            ////// <summary>
            /// Valida la fórmula VArt142_2EspIM
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VArt142_2EspIM_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VArt142_2EspIM"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 7900.74M);
            }

            ////// <summary>
            /// Valida la fórmula VArt142_3EspCF
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VArt142_3EspCF_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VArt142_3EspCF"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 7162.74M);
            }
        }

        public class ISR_Finiquitos
        {
            ////// <summary>
            /// Valida la fórmula VAplica_SE_a_USMO_Finiquitos
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VAplica_SE_a_USMO_Finiquitos_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VAplica_SE_a_USMO_Finiquitos"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);
            }

            ////// <summary>
            /// Valida la fórmula VFin_1BISPT
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VFin_1BISPT_Finiquitos_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VFin_1BISPT"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 64513.5M);
            }

            ////// <summary>
            /// Valida la fórmula VFin_2IM
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VFin_2IM_Finiquitos_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VFin_2IM"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 7900.74M);
            }

            ////// <summary>
            /// Valida la fórmula VFin_3CF
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VFin_3CF_Finiquitos_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VFin_3CF"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 7162.74M);
            }

            ////// <summary>
            /// Valida la fórmula VFin_5ISPT
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VFin_5ISPT_Finiquitos_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VFin_5ISPT"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 15063.48M);
            }

            /// <summary>
            /// vDiasDePago_P
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_vDiasDePago_P_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "vDiasDePago_P"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 15M);
            }

            /// <summary>
            /// vDiasDespuesBaja
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_vDiasDespuesBaja_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "vDiasDespuesBaja"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);

                var daysAfterDeleteEmployee = 2M;
                var employee = overdraft.Employee;
                employee.DeleteDate = overdraft.PeriodDetail.FinalDate.AddDays(Convert.ToDouble(-1M * daysAfterDeleteEmployee));

                var middlewareEmployee = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(),
                    new EmployeeValidator());
                await middlewareEmployee.UpdateAsync(new List<Employee> { employee }, identityWorkID);

                //Do the caltulation
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == daysAfterDeleteEmployee);



            }

            /// <summary>
            /// vDiasAntesIngreso
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_vDiasAntesIngreso_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "vDiasAntesIngreso"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);

                var daysBeforeEntryDateEmployee = 2M;
                var employee = overdraft.Employee;
                employee.EntryDate = overdraft.PeriodDetail.InitialDate.AddDays(Convert.ToDouble(daysBeforeEntryDateEmployee));

                var middlewareEmployee = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(),
                    new EmployeeValidator());
                await middlewareEmployee.UpdateAsync(new List<Employee> { employee }, identityWorkID);

                //Do the caltulation
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == daysBeforeEntryDateEmployee);



            }

            /// <summary>
            /// vDiasNoConsiderar
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_vDiasNoConsiderar_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "vDiasNoConsiderar"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);

                var daysBeforeEntryDateEmployee = 2M;
                var employee = overdraft.Employee;
                employee.EntryDate = overdraft.PeriodDetail.InitialDate.AddDays(Convert.ToDouble(daysBeforeEntryDateEmployee));

                var middlewareEmployee = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(),
                    new EmployeeValidator());
                await middlewareEmployee.UpdateAsync(new List<Employee> { employee }, identityWorkID);

                //Do the caltulation
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                //Depends on DescontarIncidencias y OpcionDiasPeriodo
                //Assert.True(result.Result == daysBeforeEntryDateEmployee);
                Assert.True(result.Result == 0M);
            }

            /// <summary>
            /// vDiasTarifaISPT
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_vDiasTarifaISPT_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "vDiasTarifaISPT"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 15M);
            }

            /// <summary>
            /// vDiasTarifaSE
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_vDiasTarifaSE_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "vDiasTarifaSE"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 15M);

                var daysBeforeEntryDateEmployee = 2M;
                var employee = overdraft.Employee;
                employee.EntryDate = overdraft.PeriodDetail.InitialDate.AddDays(Convert.ToDouble(daysBeforeEntryDateEmployee));

                var middlewareEmployee = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(),
                    new EmployeeValidator());
                await middlewareEmployee.UpdateAsync(new List<Employee> { employee }, identityWorkID);

                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;

                //Depends on vOpcionDiasPeriodo
                //Assert.True(result.Result == 15M + daysBeforeEntryDateEmployee);
                Assert.True(result.Result == 15M);
            }
        }

        public class ISPT_Monthly
        {
            ////// <summary>
            /// Valida la fórmula vISPT_RETENER_P
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_vISPT_RETENER_P_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "vISPT_RETENER_P"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);
            }

            ////// <summary>
            /// Valida la fórmula vSE_ENTREGAR_P
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_vSE_ENTREGAR_P_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "vSE_ENTREGAR_P"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);
            }
        }

        public class ISPT_Monthlylized
        {
            ////// <summary>
            /// Valida la fórmula vNoCalcular
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_vSE_vNoCalcular_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "vNoCalcular"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 1M);
            }

            ////// <summary>
            /// Valida la fórmula vDiasCompletarBase
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_vSE_vDiasCompletarBase_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "vDiasCompletarBase"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 30.4M);
            }

            ////// <summary>
            /// Valida la fórmula vBaseNormalPorRecibir
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_vBaseNormalPorRecibir_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "vBaseNormalPorRecibir"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);
            }

            ////// <summary>
            /// Valida la fórmula v01_BaseMensual_SinEsp
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_v01_BaseMensual_SinEsp_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "v01_BaseMensual_SinEsp"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);
            }

            ////// <summary>
            /// Valida la fórmula v01_LimiteInferior
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_v01_LimiteInferior_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "v01_LimiteInferior"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);
            }

            ////// <summary>
            /// Valida la fórmula vISPT_RETENERmz_P
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_vISPT_RETENERmz_P_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "vISPT_RETENERmz_P"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);
            }
        }

        public class LoanAndCredits
        {
            ////// <summary>
            /// Valida la fórmula vRetencionFONACOTPeriodo
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_vRetencionFONACOTPeriodo_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "vRetencionFONACOTPeriodo"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);

                Guid conceptPaymentID = overdraft.OverdraftDetails.FirstOrDefault(p => p.ConceptPayment.ConceptType == ConceptType.DeductionPayment).ConceptPaymentID;
                var fonactorMovements = await new FonacotMovementManagerUT().CreateDefaultAsync<FonacotMovement>(identityWorkID, instanceID,
                    overdraft.EmployeeID, conceptPaymentID);

                //Recalculate
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;

            }
        }

        public class MonthlyFixedSubsidy
        {
            ////// <summary>
            /// Valida la fórmula VAjuste8_BaseGravadaMes
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VAjuste8_BaseGravadaMes_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VAjuste8_BaseGravadaMes"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);
            }

            ////// <summary>
            /// Valida la fórmula Periodo[Fin mes]
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_Periodo_Finmes_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "Periodo[Fin mes]"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);
            }

            ////// <summary>
            /// Valida la fórmula VAjuste9_Subs_Causado_Mes
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VAjuste9_Subs_Causado_Mes_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VAjuste9_Subs_Causado_Mes"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);
            }

            /// <summary>
            /// TipoRegimenEmpleado
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_TipoRegimenEmpleado_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "TipoRegimenEmpleado()"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 2M);
            }

            /// <summary>
            /// VAjuste10_SubsCausaCorrespDef
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VAjuste10_SubsCausaCorrespDef_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VAjuste10_SubsCausaCorrespDef"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);
            }

            /// <summary>
            /// VAjuste10_SubsCausaCorrespDef
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VAjuste11_ISR_RETENER_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VAjuste11_ISR_RETENER"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);
            }

            /// <summary>
            /// VAjuste13_LimiteInferiorMes
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VAjuste13_LimiteInferiorMes_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VAjuste13_LimiteInferiorMes"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);
            }

            /// <summary>
            /// VAjuste17_SubsidioCausadoMes
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VAjuste17_SubsidioCausadoMes_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VAjuste17_SubsidioCausadoMes"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);
            }

            /// <summary>
            /// VAjuste18_ISR_RETENER_MES
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VAjuste18_ISR_RETENER_MES_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VAjuste18_ISR_RETENER_MES"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);
            }

            /// <summary>
            /// VAjuste20_SUBSIDIO_ENTREGAR_DEF
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VAjuste20_SUBSIDIO_ENTREGAR_DEF_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VAjuste20_SUBSIDIO_ENTREGAR_DEF"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);
            }

            /// <summary>
            /// VAjuste22_AcumuladoMes_ISRRetenido
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_VAjuste22_AcumuladoMes_ISRRetenido_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "VAjuste22_AcumuladoMes_ISRRetenido"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);
            }

        }

        public class INFONAVIT
        {
            ////// <summary>
            /// Valida la fórmula CuotaINFONAVIT
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_CuotaINFONAVIT_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "CuotaINFONAVIT"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 1584.1875M);
            }

        }

        public class Misc
        {
            ////// <summary>
            /// Valida la fórmula Invalidez y Vida
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_InvalidezyVida_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "Invalidez_y_Vida"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 554.47M);
            }

            ////// <summary>
            /// Valida la fórmula Cesantia_y_Vejez
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_CesantiayVejez_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "Cesantia_y_Vejez"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 998.04M);
            }

            ////// <summary>
            /// Valida la fórmula Enf_y_Mat_Patron
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_Enf_y_Mat_Patron_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "Enf_y_Mat_Patron"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 1125.84M);
            }

            ////// <summary>
            /// Valida la fórmula 2_Fondo_retiro_SAR_8
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_2_Fondo_retiro_SAR_8_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "Dos_Porciento_Fondo_retiro_SAR_8"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 633.675M);
            }

            ////// <summary>
            /// Valida la fórmula Dos_Porciento_Impuesto_estatal
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_Dos_Porciento_Impuesto_estatal_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "Dos_Porciento_Impuesto_estatal"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);
            }

            //Riesgo_de_trabajo_9
            ////// <summary>
            /// Valida la fórmula Riesgo_de_trabajo_9
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_Riesgo_de_trabajo_9_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "Riesgo_de_trabajo_9"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 1901.025M);
            }

            ////// <summary>
            /// Valida la fórmula IMSS_empresa
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_IMSS_empresa_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "IMSS_empresa"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 2678.35M);
            }

            //Infonavit empresa
            ////// <summary>
            /// Valida la fórmula Infonavit empresa
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_Infonavit_empresa_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "Infonavit empresa"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 1584.1875M);
            }

        }

        public class Perceptions
        {
            ////// <summary>
            /// Valida la fórmula Sueldo
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_Sueldo_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "Sueldo"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 32256.75M);
            }

            ////// <summary>
            /// Valida la fórmula Séptimo día
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_Septimodia_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "Séptimo día"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 0M);
            }
        }

        public class Deductions
        {
            ////// <summary>
            /// Valida la fórmula I.M.S.S.
            /// </summary>
            /// <returns></returns>
            [Fact]
            public async Task TM_Calculate_IMSS_Formula()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkID = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                //creates overdraft                                           
                var overdraft = await new PayrollStampingManagerUT().CreateRealOverdraftAsync(identityWorkID, instanceID);

                var overdraftDetailManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = await overdraftDetailManager.FindByExpressionAsync(p =>
                    p.ID == overdraft.ID &&
                    p.InstanceID == instanceID &&
                    p.company == identityWorkID &&
                    p.Active,
                    identityWorkID, new string[] {
                    "OverdraftDetails",
                    "OverdraftDetails.ConceptPayment",
                    "PeriodDetail",
                    "PeriodDetail.Period",
                    "Employee",
                    "Employee.HistoricEmployeeSalaryAdjustments",
                         });
                overdraft = overdrafts.FirstOrDefault();

                //calculation Manager
                ICalculationManager calculationManager = new CalculationGenericManager();

                ICalculateParams calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "DiasVigenteEnfermedadGeneral"
                };

                //Do the caltulation
                var result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 15M);

                calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "UMA"
                };

                //Do the caltulation
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 86.88M);

                calculateGenericParams = new Schema.Calculation.CalculateGenericParams()
                {
                    IdentityWorkID = identityWorkID,
                    InstanceID = instanceID,
                    OverdraftID = overdraft.ID,
                    Formula = "I.M.S.S."
                };

                //Do the caltulation
                result = await calculationManager.CalculateAsync(calculateGenericParams) as CalculateGenericResult;
                Assert.True(result.Result == 863.58M);

            }

        }
    }
}