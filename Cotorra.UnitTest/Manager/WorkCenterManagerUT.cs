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
using CotorraNode.Common.ManageException;
using System.Threading.Tasks;

namespace Cotorra.UnitTest
{
    public class WorkCenterManagerUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceId) where T : BaseEntity
        {
            var WorkCenters = new List<WorkCenter>();
            WorkCenters.Add(BuildT<WorkCenter>( identityWorkId,  instanceId));

            var middlewareManager = new MiddlewareManager<WorkCenter>(new BaseRecordManager<WorkCenter>(), 
                new WorkCenterValidator());
            await middlewareManager.CreateAsync(WorkCenters, identityWorkId);

            return WorkCenters as List<T>;
        }

        public  T BuildT<T>(Guid identityWorkId, Guid instanceId) where T : BaseEntity, ICompanyData
        {
            return new WorkCenter()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceId,
                Description = "Un barco",
                CreationDate = DateTime.Now,
                Name = "La alta mar",
                Key = 1,
                StatusID = 1,
                Observations = "Some observations"
            } as  T ;
        }

        public class Create
        {
           
            [Fact]
            public async Task Should_Create_WorkCenter_And_Get_ToValidate_Finally_do_Delete ()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();

                var WorkCenters = await new WorkCenterManagerUT().CreateDefaultAsync<WorkCenter>(identityWorkId, instanceId);

                //Act
                var middlewareManager = new MiddlewareManager<WorkCenter>(new BaseRecordManager<WorkCenter>(), new WorkCenterValidator());

                //Asserts
                //Get
                var result = await middlewareManager
                    .GetByIdsAsync(WorkCenters.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());

                //Delete
                await middlewareManager.DeleteAsync(WorkCenters.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.FirstOrDefault().ID == WorkCenters.FirstOrDefault().ID);

                //Get it again to verify if the registry it was deleted
                var result2 = await middlewareManager
                    .GetByIdsAsync(WorkCenters.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(!result2.Any());
            }

            [Fact]
            public async Task Should_Not_Create_WorkCenter_When_Ley_Is_Duplicated()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();
                var builder = new WorkCenterManagerUT();
                var WorkCenters = await builder.CreateDefaultAsync<WorkCenter>(identityWorkId, instanceId);

                var repeatedWorkCenterByKey = builder.BuildT<WorkCenter>(identityWorkId, instanceId);

                //Act
                var middlewareManager = new MiddlewareManager<WorkCenter>(new BaseRecordManager<WorkCenter>(), new WorkCenterValidator());

                try
                {
                    await middlewareManager.CreateAsync(new List<WorkCenter>() { repeatedWorkCenterByKey }, identityWorkId);

                }catch(BaseException ex)
                {
                    Assert.Equal(6902, ex.ErrorCode);
                }
                
             
            }

        }
    }
}
