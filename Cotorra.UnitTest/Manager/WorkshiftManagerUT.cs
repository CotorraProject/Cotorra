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
    public class WorkshiftManagerUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceId) where T : BaseEntity
        {
            var Workshifts = new List<Workshift>();
            Workshifts.Add(new Workshift()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceId,
                Description = "Turno mixto",
                CreationDate = DateTime.Now,
                Name = "Mixto",
                StatusID = 1,
                Hours = 8
            });

            //Act
            var middlewareManager = new MiddlewareManager<Workshift>(new BaseRecordManager<Workshift>(), new WorkshiftValidator());
            await middlewareManager.CreateAsync(Workshifts, identityWorkId);

            return Workshifts as List<T>;
        }

        public class Create
        {
            [Fact]
            public async Task Should_Create_Workshift_And_Get_ToValidate_Finally_do_Delete()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var Workshifts = new List<Workshift>();
                var identityWorkId = Guid.NewGuid();
                Workshifts.Add(new Workshift()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = Guid.NewGuid(),
                    Description = "Turno mixto",
                    CreationDate = DateTime.Now,
                    Name = "Mixto",
                    StatusID = 1,
                    Hours = 8
                });

                //Act
                var middlewareManager = new MiddlewareManager<Workshift>(new BaseRecordManager<Workshift>(), new WorkshiftValidator());
                await middlewareManager.CreateAsync(Workshifts, identityWorkId);

                //Asserts
                //Get
                var result = await middlewareManager
                    .GetByIdsAsync(Workshifts.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());

                //Delete
                await middlewareManager.DeleteAsync(Workshifts.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.FirstOrDefault().ID == Workshifts.FirstOrDefault().ID);

                //Get it again to verify if the registry it was deleted
                var result2 = await middlewareManager
                    .GetByIdsAsync(Workshifts.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(!result2.Any());
            }

            [Fact]
            public async Task ShouldFailWhenNameIsLongerThanExpected()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var Workshifts = new List<Workshift>();
                var identityWorkId = Guid.NewGuid();
                Workshifts.Add(new Workshift()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = Guid.NewGuid(),
                    Description = "Turno mixto",
                    CreationDate = DateTime.Now,
                    Name = new string('a', 26),
                    StatusID = 1,
                    Hours = 8
                });

                //Act
                var middlewareManager = new MiddlewareManager<Workshift>(new BaseRecordManager<Workshift>(), new WorkshiftValidator());
                try
                {
                    await middlewareManager.CreateAsync(Workshifts, identityWorkId);
                }
                catch (CotorraException ex)
                {
                    var res = ex.ValidationInfo;
                    Assert.NotEmpty(res);
                    Assert.Equal(8001, ex.ErrorCode);
                }

            }

            [Fact]
            public async Task ShouldFailWhenHoursIsBiggerThanExpected()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var Workshifts = new List<Workshift>();
                var identityWorkId = Guid.NewGuid();
                Workshifts.Add(new Workshift()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = Guid.NewGuid(),
                    Description = "Turno mixto",
                    CreationDate = DateTime.Now,
                    Name = "Mixto",
                    StatusID = 1,
                    Hours = 49
                });

                //Act
                var middlewareManager = new MiddlewareManager<Workshift>(new BaseRecordManager<Workshift>(), new WorkshiftValidator());
                try
                {
                    await middlewareManager.CreateAsync(Workshifts, identityWorkId);
                }
                catch (CotorraException ex)
                {
                    var res = ex.ValidationInfo;
                    Assert.NotEmpty(res);
                    Assert.Equal(8002, ex.ErrorCode);
                }

            }

            [Fact]
            public async Task ShouldFailWhenHoursIsLowerThanExpected()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var Workshifts = new List<Workshift>();
                var identityWorkId = Guid.NewGuid();
                Workshifts.Add(new Workshift()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = Guid.NewGuid(),
                    Description = "Turno mixto",
                    CreationDate = DateTime.Now,
                    Name = "Mixto",
                    StatusID = 1,
                    Hours = -1
                });

                //Act
                var middlewareManager = new MiddlewareManager<Workshift>(new BaseRecordManager<Workshift>(), new WorkshiftValidator());
                try
                {
                    await middlewareManager.CreateAsync(Workshifts, identityWorkId);
                }
                catch (CotorraException ex)
                {
                    var res = ex.ValidationInfo;
                    Assert.NotEmpty(res);
                    Assert.Equal(8002, ex.ErrorCode);
                }

            }

            [Fact]
            public async Task ShouldFailWhenNameIsEmpty()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var Workshifts = new List<Workshift>();
                var identityWorkId = Guid.NewGuid();
                Workshifts.Add(new Workshift()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = Guid.NewGuid(),
                    Description = "Turno mixto",
                    CreationDate = DateTime.Now,
                    Name = "",
                    StatusID = 1,
                    Hours = 8
                }); ;

                //Act
                var middlewareManager = new MiddlewareManager<Workshift>(new BaseRecordManager<Workshift>(), new WorkshiftValidator());
                try
                {
                    await middlewareManager.CreateAsync(Workshifts, identityWorkId);
                }
                catch (CotorraException ex)
                {
                    var res = ex.ValidationInfo;
                    Assert.NotEmpty(res);
                    Assert.Equal(8001, ex.ErrorCode);
                }

            }

            [Fact]
            public async Task ShouldFailWhen2ValuesAreBad()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var Workshifts = new List<Workshift>();
                var identityWorkId = Guid.NewGuid();
                Workshifts.Add(new Workshift()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = Guid.NewGuid(),
                    Description = "Turno mixto",
                    CreationDate = DateTime.Now,
                    Name = "",
                    StatusID = 1,
                    Hours = 49
                });

                //Act
                var middlewareManager = new MiddlewareManager<Workshift>(new BaseRecordManager<Workshift>(), new WorkshiftValidator());
                try
                {
                    await middlewareManager.CreateAsync(Workshifts, identityWorkId);
                }
                catch (CotorraException ex)
                {
                    var res = ex.ValidationInfo;
                    Assert.NotEmpty(res);
                    Assert.Equal(8001, ex.ErrorCode);
                }

            }

            [Fact]
            public async Task ShouldFailWhen2ValuesAreBadSending2Objects()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var Workshifts = new List<Workshift>();
                var identityWorkId = Guid.NewGuid();
                Workshifts.Add(new Workshift()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = Guid.NewGuid(),
                    Description = "Turno mixto",
                    CreationDate = DateTime.Now,
                    Name = "",
                    StatusID = 1,
                    Hours = 49
                });

                Workshifts.Add(new Workshift()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = Guid.NewGuid(),
                    Description = "Turno mixto",
                    CreationDate = DateTime.Now,
                    Name = "",
                    StatusID = 1,
                    Hours = 49
                });

                //Act
                var middlewareManager = new MiddlewareManager<Workshift>(new BaseRecordManager<Workshift>(), new WorkshiftValidator());
                try
                {
                    await middlewareManager.CreateAsync(Workshifts, identityWorkId);
                }
                catch (CotorraException ex)
                {
                    var res = ex.ValidationInfo;
                    Assert.NotEmpty(res);
                    Assert.Equal(8001, ex.ErrorCode);
                }

            }
        }
    }
}
