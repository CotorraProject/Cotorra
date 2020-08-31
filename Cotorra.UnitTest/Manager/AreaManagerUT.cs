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
using System.Threading.Tasks;

namespace Cotorra.UnitTest
{
    public class AreaManagerUT
    {
        private List<T> Build<T>(Guid identityWorkId, Guid instanceId) where T : BaseEntity
        {
            var Areas = new List<Area>
            {
                new Area()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = instanceId,
                    Description = "Area de nominas",
                    CreationDate = DateTime.Now,
                    Name = "Nominas",
                    Number = 1,
                    StatusID = 1,
                }
            };

            return Areas as List<T>;
        }

        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceId) where T : BaseEntity
        {
            var Areas = Build<T>(identityWorkId, instanceId) as  List<Area>;

            var middlewareManager = new MiddlewareManager<Area>(new BaseRecordManager<Area>(), new AreaValidator());
            await middlewareManager.CreateAsync(Areas, identityWorkId);

            return Areas as List<T>;
        }

        public class  CreateOrUpdate
        {
            [Fact]
            public async Task Should_Create_Area_And_Upsert_Finally_do_Delete ()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();

                var Areas = await new AreaManagerUT().CreateDefaultAsync<Area>(identityWorkId, instanceId);

                //Act
                var middlewareManager = new MiddlewareManager<Area>(new BaseRecordManager<Area>(), new AreaValidator());

                //Asserts
                //Get
                var result = await middlewareManager
                    .GetByIdsAsync(Areas.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());

                var AreasToInsert = new AreaManagerUT().Build<Area>(identityWorkId, instanceId);

                AreasToInsert.AddRange(result);

                //upsert

                await middlewareManager.CreateorUpdateAsync(AreasToInsert, identityWorkId);

                result = await middlewareManager
                  .GetByIdsAsync(AreasToInsert.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Count() == 2);

                //Delete
                await middlewareManager.DeleteAsync(AreasToInsert.Select(p => p.ID).ToList(), identityWorkId);
               

                //Get it again to verify if the registry it was deleted
                var result2 = await middlewareManager
                    .GetByIdsAsync(AreasToInsert.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(!result2.Any());
            }
        }

        public class Create
        {
            [Fact]
            public async Task Should_Create_Area_And_Get_ToValidate_Finally_do_Delete()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();

                var Areas = await new AreaManagerUT().CreateDefaultAsync<Area>(identityWorkId, instanceId);

                //Act
                var middlewareManager = new MiddlewareManager<Area>(new BaseRecordManager<Area>(), new AreaValidator());

                //Asserts
                //Get
                var result = await middlewareManager
                    .GetByIdsAsync(Areas.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());




                //Delete
                await middlewareManager.DeleteAsync(Areas.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.FirstOrDefault().ID == Areas.FirstOrDefault().ID);

                //Get it again to verify if the registry it was deleted
                var result2 = await middlewareManager
                    .GetByIdsAsync(Areas.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(!result2.Any());
            }
        }
    }
}
