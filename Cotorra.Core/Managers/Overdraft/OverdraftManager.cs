using Dapper;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using Cotorra.Core.Context;
using Cotorra.Core.Utils;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;

namespace Cotorra.Core.Managers
{
    public class OverdraftManager
    {
        public async Task<List<Overdraft>> CreateByEmployeesAsync(List<Employee> employees,
            List<PeriodDetail> periodDetailActual, List<ConceptPayment> conceptPayments)
        {
            //all good
            var identityWorkId = employees.FirstOrDefault().company;
            var instanceId = employees.FirstOrDefault().InstanceID;
            var user = employees.FirstOrDefault().user;
            List<Overdraft> overdrafts = new List<Overdraft>();

            if (null != periodDetailActual && periodDetailActual.Any())
            {
                var middlewareManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());

                foreach (var employee in employees)
                {
                    var periodDetail = periodDetailActual.FirstOrDefault(p => employee.PeriodTypeID == p.Period.PeriodTypeID
                    && employee.EntryDate <= p.FinalDate);

                    if (null != periodDetail)
                    {
                        var overdraft = new Overdraft()
                        {
                            ID = Guid.NewGuid(),
                            Active = true,
                            company = identityWorkId,
                            Timestamp = DateTime.UtcNow,
                            InstanceID = instanceId,
                            Description = "Sobrerecibo",
                            CreationDate = DateTime.Now,
                            Name = "Sobrerecibo",
                            StatusID = 1,
                            EmployeeID = employee.ID,
                            user = user,
                            PeriodDetailID = periodDetail.ID,
                        };

                        //details
                        conceptPayments.ForEach(concept =>
                        {
                            var overdraftDetail = new OverdraftDetail();
                            overdraftDetail.Active = true;
                            overdraftDetail.Amount = 0;
                            overdraftDetail.Taxed = 0;
                            overdraftDetail.Exempt = 0;
                            overdraftDetail.IMSSTaxed = 0;
                            overdraftDetail.IMSSExempt = 0;
                            overdraftDetail.company = identityWorkId;
                            overdraftDetail.ConceptPaymentID = concept.ID;
                            overdraftDetail.CreationDate = DateTime.UtcNow;
                            overdraftDetail.Name = "";
                            overdraftDetail.Description = "";
                            overdraftDetail.ID = Guid.NewGuid();
                            overdraftDetail.InstanceID = instanceId;
                            overdraftDetail.IsAmount1CapturedByUser = false;
                            overdraftDetail.IsAmount2CapturedByUser = false;
                            overdraftDetail.IsAmount3CapturedByUser = false;
                            overdraftDetail.IsAmount4CapturedByUser = false;
                            overdraftDetail.IsTotalAmountCapturedByUser = false;
                            overdraftDetail.IsValueCapturedByUser = false;
                            overdraftDetail.Label1 = "";
                            overdraftDetail.Label2 = "";
                            overdraftDetail.Label3 = "";
                            overdraftDetail.Label4 = "";
                            overdraftDetail.OverdraftID = overdraft.ID;
                            overdraftDetail.StatusID = 1;
                            overdraftDetail.Timestamp = DateTime.UtcNow;
                            overdraftDetail.user = user;
                            overdraftDetail.Value = 0;

                            overdraft.OverdraftDetails.Add(overdraftDetail);
                        });

                        overdrafts.Add(overdraft);
                    }
                }

                await middlewareManager.CreateAsync(overdrafts.ToList(), identityWorkId);
            }

            return overdrafts.ToList();
        }

        public async Task<List<Overdraft>> CreateByEmployeesAsync(List<Employee> employees)
        {
            //all good
            var identityWorkId = employees.FirstOrDefault().company;
            var instanceId = employees.FirstOrDefault().InstanceID;

            var middlewarePeriodDetailManager = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), new PeriodDetailValidator());
            var periodDetailActual = await middlewarePeriodDetailManager.FindByExpressionAsync(p =>
                p.InstanceID == instanceId
                && p.Active
                && p.PeriodStatus == PeriodStatus.Calculating, identityWorkId, new string[] { "Period" });

            var middlewareSalaryPaymentManager = new MiddlewareManager<ConceptPayment>(new BaseRecordManager<ConceptPayment>(),
              new ConceptPaymentValidator());

            var conceptPayments = await middlewareSalaryPaymentManager.FindByExpressionAsync(p =>
                    p.InstanceID == instanceId
                    && p.GlobalAutomatic
                    && p.Active, identityWorkId);

            return await CreateByEmployeesAsync(employees, periodDetailActual, conceptPayments);
        }

        public async Task<List<Overdraft>> UpdateByEmployeesAsync(List<Employee> employees)
        {
            //all good
            var identityWorkId = employees.FirstOrDefault().company;

            //Get the overdrafts to delete
            var employeesID = employees.Select(p => p.ID).ToList();
            var middlewareManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
            var middlewareOverdraftDetailManager = new MiddlewareManager<OverdraftDetail>(new BaseRecordManager<OverdraftDetail>(),
                new OverdraftDetailValidator());

            var overdraftsToDelete = await middlewareManager.FindByExpressionAsync(p => employeesID.Contains(p.EmployeeID), identityWorkId);
            if (overdraftsToDelete.Any())
            {
                //delete previous overdraft (no timbrados ni autorizados ni cancelados)
                var overdraftIds = overdraftsToDelete.Where(p => p.OverdraftStatus == OverdraftStatus.None).Select(p => p.ID).ToList();
                if (overdraftIds.Any())
                {
                    await middlewareOverdraftDetailManager.DeleteByExpresssionAsync(p => overdraftIds.Contains(p.OverdraftID), identityWorkId);
                    await middlewareManager.DeleteAsync(overdraftIds, identityWorkId);
                }
            }

            return await CreateByEmployeesAsync(employees);
        }

        public List<OverdraftDetail> GetOtherPayments(Overdraft overdraft)
        {
            //Marcado como SATCode OP
            var lstDetailsTemp = new List<OverdraftDetail>(overdraft.OverdraftDetails.DeepClone());

            //copy conceptPayment
            lstDetailsTemp.ForEach(p =>
            {
                var conceptPayment = overdraft.OverdraftDetails.FirstOrDefault(w => w.ID == p.ID).ConceptPayment;
                p.ConceptPayment = conceptPayment;
            });

            return lstDetailsTemp.Where(p =>
                    (p.ConceptPayment.SATGroupCode.Contains("OP")
                    && p.ConceptPayment.ConceptType == ConceptType.DeductionPayment) ||
                    (p.ConceptPayment.Code == 99
                    && p.ConceptPayment.ConceptType == ConceptType.DeductionPayment)
                ).Select(p =>
                {
                    if (p.Amount < 0)
                    {
                        p.Amount = p.Amount * -1;
                    }
                    return p;
                }).ToList();
        }

        public OverdraftTotalsResult GetTotals(Overdraft overdraft, RoundUtil roundUtil)
        {
            var overdraftTotalsResult = new OverdraftTotalsResult();

            var concepts = overdraft.OverdraftDetails
                .Where(p =>
                    p.ConceptPayment.Print && !p.ConceptPayment.Kind)
                .Select(p => p.ConceptPayment);

            if (overdraft.OverdraftDetails.Any())
            {
                //TotalSalaryPayments By Concepts              
                var salaryOverdraftDetails = overdraft.OverdraftDetails
                    .Where(p =>
                        p.ConceptPayment.ConceptType == ConceptType.SalaryPayment);

                var deductionOverdraftDetails = overdraft.OverdraftDetails
                    .Where(p =>
                        p.ConceptPayment.ConceptType == ConceptType.DeductionPayment &&
                        p.ConceptPayment.Print &&
                        !p.ConceptPayment.Kind);

                //WorkingDays // Días pagados / laborados
                overdraftTotalsResult.WorkingDays = salaryOverdraftDetails
                    .Where(p =>
                        p.ConceptPayment.SATGroupCode == "P-001" ||
                        p.ConceptPayment.SATGroupCode == "P-046"
                        )
                    .Sum(p => p.Value);

                //TotalGravado (que no sea indemnización / separación
                overdraftTotalsResult.TotalTaxed = salaryOverdraftDetails
                    .Sum(p => roundUtil.RoundValue(p.Taxed));

                //TotalGravado Settlement
                overdraftTotalsResult.TotalTaxedSettlement = salaryOverdraftDetails
                     .Where(p => p.ConceptPayment.SATGroupCode == "P-022" ||
                               p.ConceptPayment.SATGroupCode == "P-023" ||
                               p.ConceptPayment.SATGroupCode == "P-025")
                    .Sum(p => roundUtil.RoundValue(p.Taxed));

                //ajuste al neto
                overdraftTotalsResult.FixAmount = GetOtherPayments(overdraft)
                    .Sum(p => p.Amount);

                //TotalExento (que no sea indemnización / separación
                overdraftTotalsResult.TotalExempt = salaryOverdraftDetails
                    .Sum(p => roundUtil.RoundValue(p.Exempt));

                //Total Exento Settlement
                overdraftTotalsResult.TotalExemptSettlement = salaryOverdraftDetails
                     .Where(p => p.ConceptPayment.SATGroupCode == "P-022" ||
                               p.ConceptPayment.SATGroupCode == "P-023" ||
                               p.ConceptPayment.SATGroupCode == "P-025")
                    .Sum(p => roundUtil.RoundValue(p.Exempt));

                //TotalDeductionPayments -Excepts or payments           
                overdraftTotalsResult.TotalDeductionPayments = deductionOverdraftDetails
                    .Where(p =>
                        !p.ConceptPayment.SATGroupCode.Contains("OP") && p.ConceptPayment.Code != 99)
                    .Sum(p => roundUtil.RoundValue(p.Amount));

                //Ajuste al neto
                overdraftTotalsResult.AdjustmentAmount = deductionOverdraftDetails
                    .Where(p =>
                        p.ConceptPayment.Code == 99 &&
                        p.ConceptPayment.ConceptType == ConceptType.DeductionPayment)
                    .Sum(p => p.Amount);

                //TotalLiabilityPayments
                var overdraftLiabilities = overdraft.OverdraftDetails.Where(p => p.ConceptPayment.ConceptType == ConceptType.LiabilityPayment);
                overdraftTotalsResult.TotalLiabilityPayments = overdraftLiabilities.Sum(p => roundUtil.RoundValue(p.Amount));

                //Total de impuestos retenidos "002" (ISR)
                var conceptDeductionPaymentISR = concepts
                    .Where(p =>
                        p.ConceptType == ConceptType.DeductionPayment &&
                        p.Code != 99 &&
                        p.SATGroupCode == "D-002");
                var conceptDeductionPaymentISRIds = conceptDeductionPaymentISR.Select(p => p.ID);

                if (conceptDeductionPaymentISRIds.Any())
                {
                    overdraftTotalsResult.TotalDeductionPaymentsISR = overdraft.OverdraftDetails.Where(p =>
                        conceptDeductionPaymentISRIds.Contains(p.ConceptPaymentID)).Sum(p => roundUtil.RoundValue(p.Amount));
                }

                //Total de Indemnización
                overdraftTotalsResult.TotalSeparationCompensation = salaryOverdraftDetails
                    .Where(p => p.ConceptPayment.SATGroupCode == "P-022" ||
                               p.ConceptPayment.SATGroupCode == "P-023" ||
                               p.ConceptPayment.SATGroupCode == "P-025")
                    .Sum(p => roundUtil.RoundValue(p.Amount));

                overdraftTotalsResult.TotalSalaryPayments =
                  roundUtil.RoundValue(overdraftTotalsResult.TotalExempt)
                 + roundUtil.RoundValue(overdraftTotalsResult.TotalTaxed)
                 + roundUtil.RoundValue(overdraftTotalsResult.TotalSeparationCompensation);

                //TotalSalaryTotals
                //Total de Sueldos
                overdraftTotalsResult.TotalSalaryTotals = salaryOverdraftDetails
                    .Where(p => p.ConceptPayment.SATGroupCode != "P-022" &&
                                p.ConceptPayment.SATGroupCode != "P-023" &&
                                p.ConceptPayment.SATGroupCode != "P-025" &&
                                p.ConceptPayment.SATGroupCode != "P-039" &&
                                p.ConceptPayment.SATGroupCode != "P-044"
                                )
                    .Sum(p => roundUtil.RoundValue(p.Amount));

                //TotalOtherDeductions
                overdraftTotalsResult.TotalOtherDeductions = roundUtil.RoundValue(overdraftTotalsResult.TotalDeductionPayments)
                    - roundUtil.RoundValue(overdraftTotalsResult.TotalDeductionPaymentsISR);

                //Total OtherPayments
                overdraftTotalsResult.TotalOtherPayments = GetOtherPayments(overdraft).DistinctBy(p => p.ConceptPaymentID)
                    .Sum(p => p.Amount);

                //Total
                overdraftTotalsResult.Total = roundUtil.RoundValue(overdraftTotalsResult.TotalSalaryPayments) -
                    roundUtil.RoundValue(overdraftTotalsResult.TotalDeductionPayments);
            }

            roudAllValues(ref overdraftTotalsResult, roundUtil);

            return overdraftTotalsResult;
        }

        public decimal GetNetAmount(List<Overdraft> overdrafts)
        {
            decimal netTotal = 0m;

            foreach (var overdraft in overdrafts)
            {
                netTotal += GetNetAmount(overdraft);
            }

            return netTotal;
        }
        public decimal GetNetAmount(Overdraft overdraft)
        {
            var salariesOverdraftDetails = overdraft.OverdraftDetails.Where(p => (p.ConceptPayment.ConceptType == ConceptType.SalaryPayment) && !p.ConceptPayment.Kind);
            var deductionsOverdraftDetails = overdraft.OverdraftDetails.Where(p => (p.ConceptPayment.ConceptType == ConceptType.DeductionPayment) && !p.ConceptPayment.Kind);

            decimal salariesTotal = salariesOverdraftDetails.Sum(p => p.Amount);
            decimal deductionsTotal = deductionsOverdraftDetails.Sum(p => p.Amount);

            return salariesTotal - deductionsTotal;
        }

        public async Task<List<WorkPeriodResult>> GetWorkPeriodAsync(
            Guid companyID, Guid instanceID,
            Guid? periodDetailID = null, Guid? employeeID = null, Guid? overdraftID = null)
        {

            var workPeriodResults = new List<WorkPeriodResult>();

            //Get DB Connection
            using (var connection = new SqlConnection(ConnectionManager.ConfigConnectionString))
            {
                //Create the command
                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 15;
                command.CommandText = "GetWorkPeriod";
                command.Parameters.AddWithValue("@InstanceId", instanceID);
                command.Parameters.AddWithValue("@company", companyID);
                command.Parameters.AddWithValue("@PeriodDetailID", periodDetailID);
                command.Parameters.AddWithValue("@OverdraftID", overdraftID.HasValue ? overdraftID.Value : (Guid?)null);

                command.Connection = connection;

                //Open connection
                await connection.OpenAsync();

                //Execute query
                using (var reader = await command.ExecuteReaderAsync())
                {
                    //Bind result object
                    while (await reader.ReadAsync())
                    {
                        //Create object result
                        var wpr = new WorkPeriodResult
                        {
                            OverdraftID = Guid.Parse(reader["OverdraftID"].ToString()),
                            OverdraftStatus = (OverdraftStatus)(Int32.Parse(reader["OverdraftStatus"].ToString())),
                            OverdraftType = (OverdraftType)(Int32.Parse(reader["OverdraftType"].ToString())),
                            EmployeeID = Guid.Parse(reader["EmployeeID"].ToString()),
                            TotalPerceptions = Decimal.Parse(reader["TotalPerceptions"].ToString()),
                            TotalDeductions = Decimal.Parse(reader["TotalDeductions"].ToString()),
                            TotalLiabilities = Decimal.Parse(reader["TotalLiabilities"].ToString()),
                            UUID = Guid.Parse(reader["UUID"].ToString()),
                        };

                        //Add to return list
                        workPeriodResults.Add(wpr);
                    }

                }
            }

            return workPeriodResults;
        }

        private IDisposable SqlConnection(string configConnectionString)
        {
            throw new NotImplementedException();
        }

        private void roudAllValues(ref OverdraftTotalsResult overdraftTotalsResult, RoundUtil roundUtil)
        {
            overdraftTotalsResult.FixAmount = overdraftTotalsResult.FixAmount.ToCustomRound(roundUtil);
            overdraftTotalsResult.Total = overdraftTotalsResult.Total.ToCustomRound(roundUtil);
            overdraftTotalsResult.TotalDeductionPayments = overdraftTotalsResult.TotalDeductionPayments.ToCustomRound(roundUtil);
            overdraftTotalsResult.TotalDeductionPaymentsISR = overdraftTotalsResult.TotalDeductionPaymentsISR.ToCustomRound(roundUtil);
            overdraftTotalsResult.TotalExempt = overdraftTotalsResult.TotalExempt.ToCustomRound(roundUtil);
            overdraftTotalsResult.TotalLiabilityPayments = overdraftTotalsResult.TotalLiabilityPayments.ToCustomRound(roundUtil);
            overdraftTotalsResult.TotalOtherDeductions = overdraftTotalsResult.TotalOtherDeductions.ToCustomRound(roundUtil);
            overdraftTotalsResult.TotalOtherPayments = overdraftTotalsResult.TotalOtherPayments.ToCustomRound(roundUtil);
            overdraftTotalsResult.TotalRetirementPensionWithdrawal = overdraftTotalsResult.TotalRetirementPensionWithdrawal.ToCustomRound(roundUtil);
            overdraftTotalsResult.TotalSalaryPayments = overdraftTotalsResult.TotalSalaryPayments.ToCustomRound(roundUtil);
            overdraftTotalsResult.TotalSalaryTotals = overdraftTotalsResult.TotalSalaryTotals.ToCustomRound(roundUtil);
            overdraftTotalsResult.TotalSeparationCompensation = overdraftTotalsResult.TotalSeparationCompensation.ToCustomRound(roundUtil);
            overdraftTotalsResult.TotalTaxed = overdraftTotalsResult.TotalTaxed.ToCustomRound(roundUtil);
        }

    }
}
