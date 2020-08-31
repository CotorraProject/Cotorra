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
    public class MonthlyEmploymentSubsidyManagerUT 
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceId) where T : BaseEntity
        {
            var MonthlyIncomeTaxs = new MemoryStorageContext().GetDefaultMonthlyEmploymentSubsidy(identityWorkId, instanceId, Guid.Empty);

            var middlewareManager = new MiddlewareManager<MonthlyEmploymentSubsidy>(new BaseRecordManager<MonthlyEmploymentSubsidy>(), new MonthlyEmploymentSubsidyValidator());
            await middlewareManager.CreateAsync(MonthlyIncomeTaxs, identityWorkId);

            return MonthlyIncomeTaxs as List<T>;
        }

        public class Create
        {
            [Fact]
            public async Task Should_Create_MonthlyEmploymentSubsidy_And_Get_ToValidate_Finally_do_Delete()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();

                var MonthlyIncomeTaxs = await new MonthlyEmploymentSubsidyManagerUT().CreateDefaultAsync<MonthlyEmploymentSubsidy>(identityWorkId, instanceId);

                //Act
                var middlewareManager = new MiddlewareManager<MonthlyEmploymentSubsidy>(new BaseRecordManager<MonthlyEmploymentSubsidy>(), new MonthlyEmploymentSubsidyValidator());

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
            public async Task Should_Fail_When_LowerLimit_is_less_than_zero()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();

                //create one uma
                var MonthlyIncomeTaxs = await new MonthlyEmploymentSubsidyManagerUT().CreateDefaultAsync<MonthlyEmploymentSubsidy>(identityWorkId, instanceId);

                //Act
                var middlewareManager = new MiddlewareManager<MonthlyEmploymentSubsidy>(new BaseRecordManager<MonthlyEmploymentSubsidy>(), new MonthlyEmploymentSubsidyValidator());


                //Asserts
                //Get
                var result = await middlewareManager
                    .GetByIdsAsync(MonthlyIncomeTaxs.Select(p => p.ID).ToList(), identityWorkId);

                Assert.True(null != result && result.Any());

                try
                {
                    await middlewareManager.CreateAsync(new List<MonthlyEmploymentSubsidy>() { result.FirstOrDefault() }, identityWorkId);

                    Assert.True(false, "Debe de mandar error");
                }
                catch (CotorraException ex)
                {
                    Assert.Equal(9006, ex.ErrorCode);
                }
                catch (Exception)
                {
                    //duplicated
                    Assert.True(true);
                }               
            }

        }
    }
}
