using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cotorra.Core.Managers.Calculation;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using CotorraNode.Common.Config;
using System.Data;
using AutoMapper;
using MoreLinq;

namespace Cotorra.Core.Managers
{
    /// <summary>
    /// Manager to control the payroll authorization process
    /// </summary>
    public class AuthorizationManager
    {
        private async Task VerifyPeriod(AuthorizationParams authorizationParams)
        {
            var middlewareManagerPeriodDetail = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(),
                new PeriodDetailValidator());
            var periodDetail = (await middlewareManagerPeriodDetail.FindByExpressionAsync(p =>
                    p.ID == authorizationParams.PeriodDetailIDToAuthorize,
                authorizationParams.IdentityWorkID,
                new string[] { "Period" })).FirstOrDefault();

            if (periodDetail != null && periodDetail.PeriodStatus == PeriodStatus.Calculating)
            {
                if (periodDetail.PeriodFiscalYear == PeriodFiscalYear.Final)
                {
                    var middlewareManagerPeriod = new MiddlewareManager<Period>(new BaseRecordManager<Period>(),
                        new PeriodValidator());

                    var periodToCreate = periodDetail.Period;
                    periodToCreate.CreationDate = DateTime.Now;
                    periodToCreate.InitialDate = periodDetail.Period.InitialDate.AddYears(1);
                    periodToCreate.FinalDate = periodDetail.Period.FinalDate.AddYears(1);
                    periodToCreate.FiscalYear = periodDetail.Period.FiscalYear + 1;
                    periodToCreate.ID = Guid.NewGuid();
                    periodToCreate.user = authorizationParams.user;
                    periodToCreate.IsActualFiscalYear = true;
                    periodToCreate.IsFiscalYearClosed = false;
                    periodToCreate.Timestamp = DateTime.Now;
                    periodToCreate.PeriodDetails = null;
                    periodToCreate.PeriodType = null;

                    //Crea el siguiente periodo
                    await middlewareManagerPeriod.CreateAsync(new List<Period> { periodToCreate }, authorizationParams.IdentityWorkID);
                }
            }
            else
            {
                throw new CotorraException(102, "102", "El periodo que se intenta autorizar no está en status calculando.", null);
            }

        }

        private async Task VerifyPeriod(AuthorizationByOverdraftParams authorizationByOverdraftParams,
            List<Overdraft> overdrafts)
        {
            var middlewareManagerPeriodDetail = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(),
                new PeriodDetailValidator());

            //PeriodDetails
            var periodDetailIDs = overdrafts.Select(p => p.PeriodDetailID);
            var periodDetail = (await middlewareManagerPeriodDetail.FindByExpressionAsync(p =>
                    periodDetailIDs.Contains(p.ID),
                authorizationByOverdraftParams.IdentityWorkID,
                new string[] { "Period" })).FirstOrDefault();

            if (periodDetail != null && periodDetail.PeriodStatus == PeriodStatus.Calculating)
            {
                if (periodDetail.PeriodFiscalYear == PeriodFiscalYear.Final)
                {
                    var middlewareManagerPeriod = new MiddlewareManager<Period>(new BaseRecordManager<Period>(),
                        new PeriodValidator());

                    var periodToCreate = periodDetail.Period;
                    periodToCreate.CreationDate = DateTime.Now;
                    periodToCreate.InitialDate = periodDetail.Period.InitialDate.AddYears(1);
                    periodToCreate.FinalDate = periodDetail.Period.FinalDate.AddYears(1);
                    periodToCreate.FiscalYear = periodDetail.Period.FiscalYear + 1;
                    periodToCreate.ID = Guid.NewGuid();
                    periodToCreate.user = authorizationByOverdraftParams.user;
                    periodToCreate.IsActualFiscalYear = true;
                    periodToCreate.IsFiscalYearClosed = false;
                    periodToCreate.Timestamp = DateTime.Now;
                    periodToCreate.PeriodDetails = null;
                    periodToCreate.PeriodType = null;

                    //Crea el siguiente periodo
                    await middlewareManagerPeriod.CreateAsync(new List<Period> { periodToCreate },
                        authorizationByOverdraftParams.IdentityWorkID);
                }
            }
            else
            {
                throw new CotorraException(102, "102", "El periodo que se intenta autorizar no está en status calculando.", null);
            }

        }

        /// <summary>
        /// Payroll authorization
        /// </summary>
        /// <returns></returns>
        public async Task<List<Overdraft>> AuthorizationAsync(AuthorizationParams authorizationParams)
        {
            List<Overdraft> overdraftToCalculate = null;

            try
            {
                //VerifyPeriod - Si el periodo detalle siguiente no existe (porke fue el último) 
                //habilitar el siguiente año
                await VerifyPeriod(authorizationParams);

                using (var connection = new SqlConnection(ConnectionManager.ConfigConnectionString))
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        await connection.OpenAsync();
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "AuthorizeOverdraft";
                        command.Parameters.AddWithValue("@PeriodDetailId", authorizationParams.PeriodDetailIDToAuthorize);
                        command.Parameters.AddWithValue("@InstanceId", authorizationParams.InstanceID);
                        command.Parameters.AddWithValue("@company", authorizationParams.IdentityWorkID);
                        command.Parameters.AddWithValue("@user", authorizationParams.user);

                        //Execute SP de autorización
                        await command.ExecuteNonQueryAsync();
                    }
                }

                //Encontrar los nuevos sobre recibos para hacerles el cálculo
                var periodDetailManager = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), new PeriodDetailValidator());
                var periodDetail = await periodDetailManager.FindByExpressionAsync(p => 
                    p.PeriodStatus == PeriodStatus.Calculating,
                    authorizationParams.IdentityWorkID);
                var lstPeriodIds = periodDetail.Select(p => p.PeriodID);

                //11. Calculation overdrafts (Fire and Forget)
                await new OverdraftCalculationManager().CalculationFireAndForgetByPeriodIdsAsync(lstPeriodIds, authorizationParams.IdentityWorkID,
                                authorizationParams.InstanceID, authorizationParams.user);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Ocurrió un error al autorizar la nómina: {ex.ToString()}");
                throw new CotorraException(70000, "70000", "Ocurrió un error al autorizar la nómina", ex);
            }


            return overdraftToCalculate;
        }

        /// <summary>
        /// Payroll authorization by overdrafts
        /// </summary>
        /// <returns></returns>
        public async Task<List<Overdraft>> AuthorizationByOverdraftAsync(AuthorizationByOverdraftParams authorizationByOverdraftParams)
        {
            List<Overdraft> overdraftToCalculate = null;

            try
            {
                var middlewareManagerOverdraft = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(),
                    new OverdraftValidator());
                var overdrafts = await middlewareManagerOverdraft.FindByExpressionAsync(p =>
                    authorizationByOverdraftParams.OverdraftIDs.Contains(p.ID), 
                    authorizationByOverdraftParams.IdentityWorkID);
                                
                using (var connection = new SqlConnection(ConnectionManager.ConfigConnectionString))
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        await connection.OpenAsync();
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "AuthorizeByOverdrafts";

                        DataTable dtGuidList = new DataTable();
                        dtGuidList.Columns.Add("ID", typeof(string));
                        overdrafts.Select(p => p.ID).ForEach(p =>
                          {
                              dtGuidList.Rows.Add(p);
                          });
                        SqlParameter param = new SqlParameter("@OverdraftIDs", SqlDbType.Structured)
                        {
                            TypeName = "dbo.guidlisttabletype",
                            Value = dtGuidList
                        };
                        command.Parameters.Add(param);
                        command.Parameters.AddWithValue("@InstanceId", authorizationByOverdraftParams.InstanceID);
                        command.Parameters.AddWithValue("@company", authorizationByOverdraftParams.IdentityWorkID);
                        command.Parameters.AddWithValue("@user", authorizationByOverdraftParams.user);

                        //Execute SP de autorización
                        await command.ExecuteNonQueryAsync();
                    }
                }

                //Encontrar los empleados para realizar el cálculo
                var employeeIds = overdrafts.Select(p => p.EmployeeID);

                //11. Calculation overdrafts (Fire and Forget)
                await new OverdraftCalculationManager().CalculationByEmployeesAsync(employeeIds,
                    authorizationByOverdraftParams.IdentityWorkID, authorizationByOverdraftParams.InstanceID,
                    authorizationByOverdraftParams.user);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Ocurrió un error al autorizar la nómina: {ex.ToString()}");
                throw new CotorraException(70000, "70000", "Ocurrió un error al autorizar la nómina", ex);
            }


            return overdraftToCalculate;
        }

    }
}
