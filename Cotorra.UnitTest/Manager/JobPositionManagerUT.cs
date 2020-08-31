using Cotorra.Core;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Cotorra.Core.Extensions;
using Cotorra.Core.Validator;
using Cotorra.Client;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Serialize.Linq.Serializers;
using Serialize.Linq;
using System.Reflection;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Extensions;
using Serialize.Linq.Nodes;
using System.Transactions;
using System.Threading.Tasks;
using CotorraNode.Common.Base.Schema;

namespace Cotorra.UnitTest
{
    public class JobPositionManagerUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceID) where T : BaseEntity
        {
            var JobPositions = new List<JobPosition>();
            JobPositions.Add(new JobPosition()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceID,
                Description = "Supervisor de ventas general",
                CreationDate = DateTime.Now,
                Name = "Supervisor de Ventas",
                JobPositionRiskType = JobPositionRiskType.Class_I,
                StatusID = 1,
            });

            var middlewareManager = new MiddlewareManager<JobPosition>(new BaseRecordManager<JobPosition>(), new JobPositionValidator());
            await middlewareManager.CreateAsync(JobPositions, identityWorkId);

            return JobPositions as List<T>;
        }

        public class Create
        {
            [Fact]
            public async Task Client_Should_Create()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                var identityWorkId = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                var client = new Client<JobPosition>("", ClientConfiguration.ClientAdapter.Local);

                var JobPositions = await new JobPositionManagerUT().CreateDefaultAsync<JobPosition>(identityWorkId, instanceID);

                //Asserts
                //Get
                var result = await client
                    .GetByIdsAsync(JobPositions.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());

                //Delete
                await client.DeleteAsync(JobPositions.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.FirstOrDefault().ID == JobPositions.FirstOrDefault().ID);

                //Get it again to verify if the registry it was deleted
                var result2 = await client
                    .GetByIdsAsync(JobPositions.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(!result2.Any());
            }

            [Fact]
            public async Task Should_Not_Create_Duplicate_List_JobPosition_And_Get_ToValidate_Finally_do_Delete()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                try
                {
                    //Arrange
                    var JobPositions = new List<JobPosition>();
                    var identityWorkId = Guid.NewGuid();
                    JobPositions.Add(new JobPosition()
                    {
                        ID = Guid.NewGuid(),
                        Active = true,
                        company = identityWorkId,
                        Timestamp = DateTime.UtcNow,
                        InstanceID = identityWorkId,
                        Description = "Supervisor de ventas general",
                        CreationDate = DateTime.Now,
                        Name = "Supervisor de Ventas",
                        StatusID = 1,
                    });
                    JobPositions.Add(new JobPosition()
                    {
                        ID = Guid.NewGuid(),
                        Active = true,
                        company = identityWorkId,
                        Timestamp = DateTime.UtcNow,
                        InstanceID = identityWorkId,
                        Description = "Supervisor de ventas general",
                        CreationDate = DateTime.Now,
                        Name = "Supervisor de Ventas",
                        StatusID = 1,
                    });

                    //Act
                    var middlewareManager = new MiddlewareManager<JobPosition>(new BaseRecordManager<JobPosition>(), new JobPositionValidator());
                    await middlewareManager.CreateAsync(JobPositions, identityWorkId);
                    Assert.True(false);
                }
                catch (Exception ex)
                {
                    Assert.True(ex is CotorraException);
                    Assert.True((ex as CotorraException).ErrorCode.Equals(1002));
                }
            }

            [Fact]
            public async Task Should_Not_Create_Duplicate_DB_JobPosition_And_Get_ToValidate_Finally_do_Delete()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                var middlewareManager = new MiddlewareManager<JobPosition>(new BaseRecordManager<JobPosition>(), new JobPositionValidator());
                List<Guid> lstIds = new List<Guid>();
                var identityWorkId = Guid.NewGuid();
                try
                {
                    //Arrange
                    var JobPositions = new List<JobPosition>();
                    JobPositions.Add(new JobPosition()
                    {
                        ID = Guid.NewGuid(),
                        Active = true,
                        company = identityWorkId,
                        Timestamp = DateTime.UtcNow,
                        InstanceID = identityWorkId,
                        Description = "Supervisor de ventas general",
                        CreationDate = DateTime.Now,
                        Name = "Supervisor de Ventas",
                        StatusID = 1,
                    });
                    lstIds.Add(JobPositions.FirstOrDefault().ID);

                    //Act
                    await middlewareManager.CreateAsync(JobPositions, identityWorkId);

                    //Asserts
                    //Get
                    var result = await middlewareManager
                        .GetByIdsAsync(JobPositions.Select(p => p.ID).ToList(), identityWorkId);
                    Assert.True(result.Any());

                    //Act
                    await middlewareManager.CreateAsync(JobPositions, identityWorkId);
                    Assert.True(false, "No debió de pasar");
                }
                catch (Exception ex)
                {
                    Assert.True(ex is CotorraException);
                    Assert.True((ex as CotorraException).ErrorCode.Equals(1002));
                }               
            }
        }
    }
}
