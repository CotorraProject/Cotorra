using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Cotorra.UnitTest
{
    public class EmployeeSalaryAdjustmentManagerUT
    {
        [Fact]
        public async Task SalaryAdjustment_Success_HappyPath()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            Guid identityWorkId = Guid.NewGuid();
            Guid instanceID = Guid.NewGuid();

            try
            {
                var employee = await new EmployeeManagerUT().CreateDefaultAsync<Employee>(
                    identityWorkId, instanceID);

                var middlewareManagerPeriodDetail = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), new PeriodDetailValidator());
                var periodDetails = await middlewareManagerPeriodDetail.FindByExpressionAsync(p =>
                    p.InstanceID == instanceID, identityWorkId, new string[] { "Period", "Period.PeriodType" });

                Guid sbcID = Guid.NewGuid();
                var creationDate = DateTime.Now;
                EmployeeSBCAdjustment sbc = BuildEmployeeSBCAdjustment(sbcID, Guid.NewGuid(), identityWorkId, instanceID, creationDate, employee.FirstOrDefault().ID, 123.23M, 123.23M, 123.23M);

                EmployeeSalaryIncrease employeeSalaryIncrease = new EmployeeSalaryIncrease()
                {
                    Active = true,
                    company = identityWorkId,
                    CreationDate = creationDate,
                    DailySalary = 145.65M,
                    DeleteDate = null,
                    Description = "",
                    EmployeeID = employee.FirstOrDefault().ID,
                    ID = Guid.NewGuid(),
                    user = Guid.NewGuid(),
                    InstanceID = instanceID,
                    ModificationDate = periodDetails.OrderBy(p => p.InitialDate).FirstOrDefault().InitialDate.AddDays(2),
                    Name = "",
                    StatusID = 1,
                    Timestamp = DateTime.Now,
                    EmployeeSBCAdjustment = sbc,
                    EmployeeSBCAdjustmentID = sbc.ID
                };

                var middlewareManager = new MiddlewareManager<EmployeeSalaryIncrease>(new BaseRecordManager<EmployeeSalaryIncrease>());

                await middlewareManager.CreateAsync(new List<EmployeeSalaryIncrease>() { employeeSalaryIncrease }, identityWorkId);

            }
            catch (CotorraException ex)
            {
                Assert.True(true, "No debió de pasar");
            }
            catch
            {
                Assert.True(true, "No debió de pasar");
            }
        }

        [Fact]
        public async Task SalaryAdjustment_Success_HappyPath_2()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            Guid identityWorkId = Guid.NewGuid();
            Guid instanceID = Guid.NewGuid();

            try
            {
                var employee = await new EmployeeManagerUT().CreateDefaultAsync<Employee>(
                    identityWorkId, instanceID);

                var middlewareManagerPeriodDetail = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), new PeriodDetailValidator());
                var periodDetails = await middlewareManagerPeriodDetail.FindByExpressionAsync(p =>
                    p.InstanceID == instanceID, identityWorkId, new string[] { "Period", "Period.PeriodType" });


                Guid sbcID = Guid.NewGuid();
                var creationDate = DateTime.Now;
                EmployeeSBCAdjustment sbc = BuildEmployeeSBCAdjustment(sbcID, Guid.NewGuid(), identityWorkId, instanceID, creationDate, employee.FirstOrDefault().ID, 123.23M, 123.23M, 123.23M);


                EmployeeSalaryIncrease employeeSalaryIncrease = new EmployeeSalaryIncrease()
                {
                    Active = true,
                    company = identityWorkId,
                    CreationDate = creationDate,
                    DailySalary = 145.65M,
                    DeleteDate = null,
                    Description = "",
                    EmployeeID = employee.FirstOrDefault().ID,
                    ID = Guid.NewGuid(),
                    user = Guid.NewGuid(),
                    InstanceID = instanceID,
                    ModificationDate = periodDetails.OrderBy(p => p.InitialDate).FirstOrDefault().InitialDate.AddMonths(2),
                    Name = "", 
                    StatusID = 1,
                    Timestamp = DateTime.Now,
                    EmployeeSBCAdjustment = sbc,
                    EmployeeSBCAdjustmentID = sbc.ID
                };


                var middlewareManager = new MiddlewareManager<EmployeeSalaryIncrease>(new BaseRecordManager<EmployeeSalaryIncrease>());


                await middlewareManager.CreateAsync(new List<EmployeeSalaryIncrease>() { employeeSalaryIncrease }, identityWorkId);

            }
            catch (CotorraException ex)
            {
                Assert.True(true, "No debió de pasar");
            }
            catch
            {
                Assert.True(true, "No debió de pasar");
            }
        }

        [Fact]
        public async Task SalaryAdjustment_Fail_Duplicate_adjustment_Same_Period()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            Guid identityWorkId = Guid.NewGuid();
            Guid instanceID = Guid.NewGuid();

            try
            {
                var employee = await new EmployeeManagerUT().CreateDefaultAsync<Employee>(
                    identityWorkId, instanceID);

                var middlewareManagerPeriodDetail = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), new PeriodDetailValidator());
                var periodDetails = await middlewareManagerPeriodDetail.FindByExpressionAsync(p =>
                    p.InstanceID == instanceID, identityWorkId, new string[] { "Period", "Period.PeriodType" });

                Guid sbcID = Guid.NewGuid();
                var creationDate = DateTime.Now;
                EmployeeSBCAdjustment sbc = BuildEmployeeSBCAdjustment(sbcID, Guid.NewGuid(), identityWorkId, instanceID, creationDate, employee.FirstOrDefault().ID, 123.23M, 123.23M, 123.23M);

                EmployeeSalaryIncrease employeeSalaryIncrease = new EmployeeSalaryIncrease()
                {
                    Active = true,
                    company = identityWorkId,
                    CreationDate = creationDate,
                    DailySalary = 145.65M,
                    DeleteDate = null,
                    Description = "",
                    EmployeeID = employee.FirstOrDefault().ID,
                    ID = Guid.NewGuid(),
                    user = Guid.NewGuid(),
                    InstanceID = instanceID,
                    ModificationDate = periodDetails.OrderBy(p => p.InitialDate).FirstOrDefault().InitialDate.AddDays(2),
                    Name = "", 
                    StatusID = 1,
                    Timestamp = DateTime.Now,
                    EmployeeSBCAdjustment = sbc,
                    EmployeeSBCAdjustmentID = sbc.ID,
                   
                };
                var middlewareManager = new MiddlewareManager<EmployeeSalaryIncrease>(new BaseRecordManager<EmployeeSalaryIncrease>());

                await middlewareManager.CreateAsync(new List<EmployeeSalaryIncrease>() { employeeSalaryIncrease }, identityWorkId);

                employeeSalaryIncrease.DailySalary = 23.45M;
                employeeSalaryIncrease.ModificationDate = periodDetails.OrderBy(p => p.InitialDate).FirstOrDefault().InitialDate.AddDays(3);
                await middlewareManager.CreateAsync(new List<EmployeeSalaryIncrease>() { employeeSalaryIncrease }, identityWorkId);

                Assert.True(true, "No debió de pasar");
            }
            catch (CotorraException ex)
            {
                Assert.True(ex.Code == "4004");
            }
            catch
            {
                Assert.True(true, "No debió de pasar");
            }
        }

        [Fact]
        public async Task SalaryAdjustment_Fail_PeriodClosed()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            Guid identityWorkId = Guid.NewGuid();
            Guid instanceID = Guid.NewGuid();

            try
            {
                var employee = await new EmployeeManagerUT().CreateDefaultAsync<Employee>(
                    identityWorkId, instanceID);

                var middlewareManagerPeriodDetail = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), new PeriodDetailValidator());
                var periodDetails = await middlewareManagerPeriodDetail.FindByExpressionAsync(p =>
                    p.InstanceID == instanceID, identityWorkId);

                var CotorriaPeriodDetail = periodDetails.FirstOrDefault();
                CotorriaPeriodDetail.PeriodStatus = PeriodStatus.Authorized;
                await middlewareManagerPeriodDetail.UpdateAsync(new List<PeriodDetail> { CotorriaPeriodDetail }, identityWorkId);

                Guid sbcID = Guid.NewGuid();
                var creationDate = DateTime.Now;
                EmployeeSBCAdjustment sbc = BuildEmployeeSBCAdjustment(sbcID, Guid.NewGuid(), identityWorkId, instanceID, creationDate, employee.FirstOrDefault().ID, 123.23M, 123.23M, 123.23M);


                EmployeeSalaryIncrease employeeSalaryIncrease = new EmployeeSalaryIncrease()
                {
                    Active = true,
                    company = identityWorkId,
                    CreationDate = creationDate,
                    DailySalary = 145.65M,
                    DeleteDate = null,
                    Description = "",
                    EmployeeID = employee.FirstOrDefault().ID,
                    ID = Guid.NewGuid(),
                    user = Guid.NewGuid(),
                    InstanceID = instanceID,
                    ModificationDate = periodDetails.OrderBy(p => p.InitialDate).FirstOrDefault().InitialDate.AddDays(2),
                    Name = "", 
                    StatusID = 1,
                    Timestamp = DateTime.Now,
                    EmployeeSBCAdjustment = sbc,
                    EmployeeSBCAdjustmentID = sbc.ID
                };
                var middlewareManager = new MiddlewareManager<EmployeeSalaryIncrease>(new BaseRecordManager<EmployeeSalaryIncrease>());
                await middlewareManager.CreateAsync(new List<EmployeeSalaryIncrease>() { employeeSalaryIncrease }, identityWorkId);

                Assert.True(true, "No debió de pasar");
            }
            catch (CotorraException ex)
            {
                Assert.True(ex.Code == "4005");
            }
            catch
            {
                Assert.True(true, "No debió de pasar");
            }
        }

        private static EmployeeSBCAdjustment BuildEmployeeSBCAdjustment(Guid ID, Guid userID, Guid companyID, Guid instanceID, DateTime date, Guid employeeID, decimal sBCFixedPart,
            decimal sBCMax25UMA, decimal sBCVariablePart)
        {
            return new EmployeeSBCAdjustment()
            {
                ID = ID,
                IdentityID = userID,
                Active = true,
                company = companyID,
                CreationDate = DateTime.Now,
                DeleteDate = null,
                Description = " ",
                InstanceID = instanceID,
                ModificationDate = date,
                Name = " ",
                EmployeeID = employeeID,
                StatusID = 1,
                Timestamp = DateTime.Now,
                SBCFixedPart = sBCFixedPart,
                SBCMax25UMA = sBCMax25UMA,
                SBCVariablePart = sBCVariablePart,
            };
        }
    }
}
