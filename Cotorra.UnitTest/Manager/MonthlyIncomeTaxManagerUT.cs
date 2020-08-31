using Cotorra.Core;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Cotorra.Core.Extensions;
using Cotorra.Core.Validator;
using CotorraNode.Common.Base.Schema;
using System.Transactions;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cotorra.UnitTest
{
    public class MonthlyIncomeTaxManagerUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceId) where T : BaseEntity
        {
            var MonthlyIncomeTaxs = new MemoryStorageContext().GetDefaultMonthlyIncomeTax(identityWorkId, instanceId, Guid.Empty);

            var middlewareManager = new MiddlewareManager<MonthlyIncomeTax>(new BaseRecordManager<MonthlyIncomeTax>(), 
                new MonthlyIncomeTaxValidator());
            await middlewareManager.CreateAsync(MonthlyIncomeTaxs, identityWorkId);

            return MonthlyIncomeTaxs as List<T>;
        }

        public class Create
        {
            [Fact]
            public async Task Should_Create_MonthlyIncomeTax_And_Get_ToValidate_Finally_do_Delete()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();

                var MonthlyIncomeTaxs = await new MonthlyIncomeTaxManagerUT().CreateDefaultAsync<MonthlyIncomeTax>(identityWorkId, instanceId);

                //Act
                var middlewareManager = new MiddlewareManager<MonthlyIncomeTax>(new BaseRecordManager<MonthlyIncomeTax>(), new MonthlyIncomeTaxValidator());

                //Asserts
                //Get
                var result = await middlewareManager
                    .GetByIdsAsync(MonthlyIncomeTaxs.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());

                //Delete
                await middlewareManager.DeleteAsync(MonthlyIncomeTaxs.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.FirstOrDefault().ID == MonthlyIncomeTaxs.FirstOrDefault().ID);

                //Get it again to verify if the registry it was deleted
                var result2 = await middlewareManager
                    .GetByIdsAsync(MonthlyIncomeTaxs.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(!result2.Any());
            }

            [Fact]
            public async Task Should_Fail_When_FixedFee_is_less_than_zero()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();

                //Arrange
                var MonthlyIncomeTaxs = new List<MonthlyIncomeTax>();
                MonthlyIncomeTaxs.Add(new MonthlyIncomeTax()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    FixedFee = 1M,
                    LowerLimit = 6942.21M,
                    Rate = .0648M,
                    InstanceID = instanceId,
                    Description = "Some Fee",
                    CreationDate = DateTime.Now,
                    Name = "Nominas fee",
                    StatusID = 1,
                });

                var middlewareManager = new MiddlewareManager<MonthlyIncomeTax>(new BaseRecordManager<MonthlyIncomeTax>(), new MonthlyIncomeTaxValidator());

                //Act

                try
                {
                    await middlewareManager.CreateAsync(MonthlyIncomeTaxs, identityWorkId);
                }
                catch (CotorraException ex)
                {
                    Assert.Equal(4002, ex.ErrorCode);
                }
            }

            [Fact]
            public async Task Should_Fail_When_LowerLimit_is_less_than_zero()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();

                //Arrange
                var MonthlyIncomeTaxs = new List<MonthlyIncomeTax>();
                MonthlyIncomeTaxs.Add(new MonthlyIncomeTax()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    FixedFee = 1M,
                    LowerLimit = -21M,
                    Rate = .0648M,
                    InstanceID = instanceId,
                    Description = "Some Fee",
                    CreationDate = DateTime.Now,
                    Name = "Nominas fee",
                    StatusID = 1,
                });

                var middlewareManager = new MiddlewareManager<MonthlyIncomeTax>(new BaseRecordManager<MonthlyIncomeTax>(), new MonthlyIncomeTaxValidator());

                //Act

                try
                {
                    await middlewareManager.CreateAsync(MonthlyIncomeTaxs, identityWorkId);
                }
                catch (CotorraException ex)
                {
                    Assert.Equal(4001, ex.ErrorCode);
                }
            }

            [Fact]
            public async Task Should_Fail_When_Rate_is_less_than_zero()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();

                //Arrange
                var MonthlyIncomeTaxs = new List<MonthlyIncomeTax>();
                MonthlyIncomeTaxs.Add(new MonthlyIncomeTax()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    FixedFee = 1M,
                    LowerLimit = 21M,
                    Rate = -.0648M,
                    InstanceID = instanceId,
                    Description = "Some Fee",
                    CreationDate = DateTime.Now,
                    Name = "Nominas fee",
                    StatusID = 1,
                });

                var middlewareManager = new MiddlewareManager<MonthlyIncomeTax>(new BaseRecordManager<MonthlyIncomeTax>(), new MonthlyIncomeTaxValidator());

                //Act

                try
                {
                    await middlewareManager.CreateAsync(MonthlyIncomeTaxs, identityWorkId);
                }
                catch (CotorraException ex)
                {
                    Assert.Equal(4003, ex.ErrorCode);
                }
            }

        }

    }
}
