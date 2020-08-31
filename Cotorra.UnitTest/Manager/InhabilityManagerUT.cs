using Cotorra.Core;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Cotorra.Core.Extensions;
using Cotorra.Core.Validator;
using CotorraNode.Common.Config;
using Microsoft.Extensions.Configuration;
using System.Transactions;
using CotorraNode.Common.Base.Schema;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Cotorra.UnitTest.Manager;

namespace Cotorra.UnitTest
{
    public class InhabilityManagerUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceID) where T : BaseEntity
        {
            //Act Dependencies          
            var employee = await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId, instanceID, randomValues: true);
            List<IncidentType> incidentTypes = await new IncidentTypeManagerUT().CreateDefaultAsync<IncidentType>(identityWorkId, instanceID);

            var middlewareManager = new MiddlewareManager<Inhability>(new BaseRecordManager<Inhability>(), new InhabilityValidator());
            List<Inhability> inhabilities = new List<Inhability>();

            inhabilities.Add(new Inhability()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceID,
                user = Guid.NewGuid(),
                CreationDate = DateTime.UtcNow,
                DeleteDate = null,
                Name = "_",
                StatusID = 1,
                Description = "desc",
                AuthorizedDays = 90,
                CategoryInsurance = CategoryInsurance.WorkRisk,
                Consequence = Consequence.Death,
                EmployeeID = employee.FirstOrDefault().ID,
                Folio = "A12345678",
                IncidentTypeID = incidentTypes.FirstOrDefault(p => p.ItConsiders == ItConsiders.Inhability).ID,
                InhabilityControl = InhabilityControl.DeathST3,
                InitialDate = new DateTime(DateTime.UtcNow.Year, 1, 1),
                Percentage = 25,
                RiskType = RiskType.WorkAccident
            });

            await middlewareManager.CreateAsync(inhabilities, identityWorkId);

            return inhabilities as List<T>;
        }

        public async Task<List<T>> CreateDefaultAsyncWithDate<T>(Guid identityWorkId, 
            Guid instanceID, Guid employeeID, DateTime inhabilityDate) where T : BaseEntity
        {
            //Act Dependencies          
            var middlewareManager = new MiddlewareManager<Inhability>(new BaseRecordManager<Inhability>(), new InhabilityValidator());
            List<Inhability> inhabilities = new List<Inhability>();

            //Act Dependencies
            var incidentTypes = await new IncidentTypeManagerUT().CreateDefaultAsync<IncidentType>(identityWorkId, instanceID);

            inhabilities.Add(new Inhability()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceID,
                user = Guid.NewGuid(),
                CreationDate = DateTime.UtcNow,
                DeleteDate = null,
                Name = "_",
                StatusID = 1,
                Description = "desc",
                AuthorizedDays = 90,
                CategoryInsurance = CategoryInsurance.WorkRisk,
                Consequence = Consequence.PostDischargeValuation,
                EmployeeID = employeeID,
                Folio = "A12345678",
                IncidentTypeID = incidentTypes.FirstOrDefault(p => p.ItConsiders == ItConsiders.Inhability).ID,
                InhabilityControl = InhabilityControl.Subsequent,
                InitialDate = inhabilityDate,
                Percentage = 25,
                RiskType = RiskType.WorkAccident
            });

            await middlewareManager.CreateAsync(inhabilities, identityWorkId);

            return inhabilities as List<T>;
        }


        private static List<Inhability> BuildEntities(Guid identityWorkId, Guid instanceID)
        {
            List<Inhability> inhabilities = new List<Inhability>();

            inhabilities.Add(new Inhability()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceID,
                user = Guid.NewGuid(),
                CreationDate = DateTime.UtcNow,
                DeleteDate = null,
                Name = "_",
                StatusID = 1,
                Description = "desc",
                AuthorizedDays = 90,
                CategoryInsurance = CategoryInsurance.WorkRisk,
                Consequence = Consequence.Death,
                EmployeeID = Guid.NewGuid(),
                Folio = "A12345678",
                IncidentTypeID = Guid.NewGuid(),
                InhabilityControl = InhabilityControl.DeathST3,
                InitialDate = new DateTime(DateTime.UtcNow.Year, 1, 1),
                Percentage = 25,
                RiskType = RiskType.WorkAccident
            });

            return inhabilities;
        }

        public class Create
        {
            [Fact]
            public async Task Should_Create_Inhability_And_Get_ToValidate_Finally_do_Delete()
            {
                var txOptions = new System.Transactions.TransactionOptions();
                txOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;

                using var scope = new TransactionScope(TransactionScopeOption.Required, txOptions, TransactionScopeAsyncFlowOption.Enabled);

                //Act
                Guid identityWorkId = Guid.NewGuid();//Guid.Parse("7535C4E6-4712-4DD6-955D-FCA86E054D49");
                Guid instanceId = Guid.NewGuid(); //Guid.Parse("33D7CA50-39E9-4B14-B482-5FCBEC07E8DB");
                var inhabilities = await new InhabilityManagerUT().CreateDefaultAsync<Inhability>(identityWorkId, instanceId);
                var middlewareManager = new MiddlewareManager<Inhability>(new BaseRecordManager<Inhability>(), new InhabilityValidator());

                //Asserts
                //Get
                var result = await middlewareManager
                    .GetByIdsAsync(inhabilities.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());

                //Delete
                await middlewareManager.DeleteAsync(inhabilities.Select(p => p.ID).ToList(), identityWorkId);

                //Get it again to verify if the registry it was deleted
                var result2 = await middlewareManager
                    .GetByIdsAsync(inhabilities.Select(p => p.ID).ToList(), identityWorkId);
                Assert.False(result2.Any());
            }

            [Fact]
            public async Task Should_Fall_Inhability_Duplicated_Folio()
            {
                var txOptions = new System.Transactions.TransactionOptions();
                txOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;

                using var scope = new TransactionScope(TransactionScopeOption.Required, txOptions, TransactionScopeAsyncFlowOption.Enabled);

                //Act
                Guid identityWorkId = Guid.NewGuid();//Guid.Parse("7535C4E6-4712-4DD6-955D-FCA86E054D49");
                var middlewareManager = new MiddlewareManager<Inhability>(new BaseRecordManager<Inhability>(), new InhabilityValidator());
                Guid id = Guid.Empty;

                try
                {
                    Guid instanceId = Guid.NewGuid(); //Guid.Parse("33D7CA50-39E9-4B14-B482-5FCBEC07E8DB");
                    var inhabilities = await new InhabilityManagerUT().CreateDefaultAsync<Inhability>(identityWorkId, instanceId);

                    id = inhabilities.FirstOrDefault().ID;
                    inhabilities.FirstOrDefault().ID = Guid.NewGuid();
                    await middlewareManager.CreateAsync(inhabilities, identityWorkId);

                    Assert.False(true);
                }
                catch (Exception ex)
                {
                    Assert.Contains("Existe 1 o más registros que ya existen en la base de datos con el campo: Folio", ex.Message);
                }
            }

            [Fact]
            public async Task Should_Fall_Inhability_Validate_Incidents()
            {
                var txOptions = new System.Transactions.TransactionOptions();
                txOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;

                using var scope = new TransactionScope(TransactionScopeOption.Required, txOptions, TransactionScopeAsyncFlowOption.Enabled);

                //Act
                Guid instanceId = Guid.NewGuid(); //Guid.Parse("33D7CA50-39E9-4B14-B482-5FCBEC07E8DB");
                Guid identityWorkId = Guid.NewGuid();//Guid.Parse("7535C4E6-4712-4DD6-955D-FCA86E054D49");
                var middlewareManager = new MiddlewareManager<Inhability>(new BaseRecordManager<Inhability>(), new InhabilityValidator());
                Guid id = Guid.Empty;

                try
                {
                    var inhabilities = await new InhabilityManagerUT().CreateDefaultAsync<Inhability>(identityWorkId, instanceId);
                    var employeeId = inhabilities.FirstOrDefault().EmployeeID;
                    var incidentTypeId = inhabilities.FirstOrDefault().IncidentTypeID;
                    var incidents = await new IncidentManagerUT().CreateDefaultAsync<Incident>(identityWorkId, instanceId, employeeId, incidentTypeId);


                    Assert.False(true);
                }
                catch (Exception ex)
                {
                    Assert.Contains("Existe 1 o más registros que ya existen en la base de datos con el campo: Folio", ex.Message);
                }
            }

            [Fact]
            public void Should_Fail_When_Description_is_larger_than_41()
            {
                //Assert
                Guid identityWorkId = Guid.NewGuid();
                Guid instanceId = Guid.NewGuid();
                var validator = new InhabilityValidator();
                var entities = BuildEntities(identityWorkId, instanceId);
                entities.FirstOrDefault().Description = new String('a', 42);
                //Act
                try
                {
                    validator.BeforeCreate(entities);
                    //Assert
                    Assert.False(true);

                }
                catch (CotorraException e)
                {
                    Assert.Equal(18001, e.ErrorCode);
                }
            }

            [Fact]
            public void Should_Fail_When_Description_is_minor_than_1()
            {
                //Assert
                Guid identityWorkId = Guid.NewGuid();
                Guid instanceId = Guid.NewGuid();
                var validator = new InhabilityValidator();
                var entities = BuildEntities(identityWorkId, instanceId);
                entities.FirstOrDefault().Description = String.Empty;
                //Act
                try
                {
                    validator.BeforeCreate(entities);
                    //Assert
                    Assert.False(true);

                }
                catch (CotorraException e)
                {
                    Assert.Equal(18001, e.ErrorCode);
                }
            }

            [Fact]
            public void Should_Fail_When_Folio_is_larger_than_9()
            {
                //Assert
                Guid identityWorkId = Guid.NewGuid();
                Guid instanceId = Guid.NewGuid();
                var validator = new InhabilityValidator();
                var entities = BuildEntities(identityWorkId, instanceId);
                entities.FirstOrDefault().Folio = new String('a', 10);
                //Act
                try
                {
                    validator.BeforeCreate(entities);
                    //Assert
                    Assert.False(true);

                }
                catch (CotorraException e)
                {
                    Assert.Equal(18002, e.ErrorCode);
                }
            }

            [Fact]
            public void Should_Fail_When_Folio_is_minor_than_1()
            {
                //Assert
                Guid identityWorkId = Guid.NewGuid();
                Guid instanceId = Guid.NewGuid();
                var validator = new InhabilityValidator();
                var entities = BuildEntities(identityWorkId, instanceId);
                entities.FirstOrDefault().Folio = null;
                //Act
                try
                {
                    validator.BeforeCreate(entities);
                    //Assert
                    Assert.False(true);

                }
                catch (CotorraException e)
                {
                    Assert.Equal(18002, e.ErrorCode);
                }
            }

            [Fact]
            public void Should_Fail_When_AuthorizedDays_is_larger_than_90()
            {
                //Assert
                Guid identityWorkId = Guid.NewGuid();
                Guid instanceId = Guid.NewGuid();
                var validator = new InhabilityValidator();
                var entities = BuildEntities(identityWorkId, instanceId);
                entities.FirstOrDefault().AuthorizedDays = 91;
                //Act
                try
                {
                    validator.BeforeCreate(entities);
                    //Assert
                    Assert.False(true);

                }
                catch (CotorraException e)
                {
                    Assert.Equal(18003, e.ErrorCode);
                }
            }

            [Fact]
            public void Should_Fail_When_AuthorizedDays_is_minor_than_1()
            {
                //Assert
                Guid identityWorkId = Guid.NewGuid();
                Guid instanceId = Guid.NewGuid();
                var validator = new InhabilityValidator();
                var entities = BuildEntities(identityWorkId, instanceId);
                entities.FirstOrDefault().AuthorizedDays = 0;
                //Act
                try
                {
                    validator.BeforeCreate(entities);
                    //Assert
                    Assert.False(true);

                }
                catch (CotorraException e)
                {
                    Assert.Equal(18003, e.ErrorCode);
                }
            }

            [Fact]
            public void Should_Fail_When_Percentage_is_larger_than_101()
            {
                //Assert
                Guid identityWorkId = Guid.NewGuid();
                Guid instanceId = Guid.NewGuid();
                var validator = new InhabilityValidator();
                var entities = BuildEntities(identityWorkId, instanceId);
                entities.FirstOrDefault().Percentage = 101;
                //Act
                try
                {
                    validator.BeforeCreate(entities);
                    //Assert
                    Assert.False(true);

                }
                catch (CotorraException e)
                {
                    Assert.Equal(18004, e.ErrorCode);
                }
            }

            [Fact]
            public void Should_Fail_When_Percentage_is_minor_than_1()
            {
                //Assert
                Guid identityWorkId = Guid.NewGuid();
                Guid instanceId = Guid.NewGuid();
                var validator = new InhabilityValidator();
                var entities = BuildEntities(identityWorkId, instanceId);
                entities.FirstOrDefault().Percentage = 0;
                //Act
                try
                {
                    validator.BeforeCreate(entities);
                    //Assert
                    Assert.False(true);

                }
                catch (CotorraException e)
                {
                    Assert.Equal(18004, e.ErrorCode);
                }
            }
        }
    }
}
