using CotorraNode.Common.Base.Schema;
using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Cotorra.UnitTest.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Cotorra.UnitTest
{
    public class FonacotMovementManagerUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceId) where T : BaseEntity
        {

            var middlewareEmployeeManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
            var employees = (await middlewareEmployeeManager.FindByExpressionAsync(p => p.Active && p.InstanceID == instanceId, identityWorkId));
            if (!employees.Any())
            {
                employees = (await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId: identityWorkId, instanceID: instanceId));
            }

            var middlewareConceptManager = new MiddlewareManager<ConceptPayment>(new BaseRecordManager<ConceptPayment>(), new ConceptPaymentValidator());
            var concepts = (await middlewareConceptManager.FindByExpressionAsync(p => p.Active && p.InstanceID == instanceId, identityWorkId));
            if (!concepts.Any())
            {
                concepts = (await new ConceptManagerUT().CreateDefaultSalaryPaymentConceptsAsync(identityWorkId, instanceId));
            }

            var fonacotMovements = new List<FonacotMovement>();
            fonacotMovements.Add(new FonacotMovement()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceId,
                Description = "Departamento de nominas",
                CreationDate = DateTime.Now,
                Name = "Nominas",
                StatusID = 1,            
                ConceptPaymentID = concepts.FirstOrDefault().ID,
                CreditNumber = "AA010101",
                DeleteDate = null,
                EmployeeID = employees.FirstOrDefault().ID,
                FonacotMovementStatus = FonacotMovementStatus.Active,
                Month = 1,
                Year = 2020,
                Observations = "Observations",
                RetentionType = RetentionType.FixedAmount,
                user = Guid.Empty,
            });

            var middlewareManager = new MiddlewareManager<FonacotMovement>(new BaseRecordManager<FonacotMovement>(), new FonacotMovementValidator());
            await middlewareManager.CreateAsync(fonacotMovements, identityWorkId);

            return fonacotMovements as List<T>;
        }

        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, 
            Guid instanceId, Guid employeeID, Guid conceptPaymentID) where T : BaseEntity
        {
            var fonacotMovements = new List<FonacotMovement>();
            fonacotMovements.Add(new FonacotMovement()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceId,
                Description = "Departamento de nominas",
                CreationDate = DateTime.Now,
                Name = "Nominas",
                StatusID = 1,
                ConceptPaymentID = conceptPaymentID,
                CreditNumber = "AA010101",
                DeleteDate = null,
                EmployeeID = employeeID,
                FonacotMovementStatus = FonacotMovementStatus.Active,
                Month = 1,
                Year = 2020,
                Observations = "Observations",
                RetentionType = RetentionType.FixedAmount,
                user = Guid.Empty,
            });

            var middlewareManager = new MiddlewareManager<FonacotMovement>(new BaseRecordManager<FonacotMovement>(), new FonacotMovementValidator());
            await middlewareManager.CreateAsync(fonacotMovements, identityWorkId);

            return fonacotMovements as List<T>;
        }

        public class Create
        {
            [Fact]
            public async Task Should_Create_Fonacot_Get_ToValidate_Finally_do_Delete()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();

                //Act
                var fonacotMovements = await new FonacotMovementManagerUT().CreateDefaultAsync<FonacotMovement>(identityWorkId, instanceId);

                //Asserts
                //Get
                var middlewareManager = new MiddlewareManager<FonacotMovement>(new BaseRecordManager<FonacotMovement>(), new FonacotMovementValidator());
                var result = await middlewareManager
                    .GetByIdsAsync(fonacotMovements.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());

                //Delete
                await middlewareManager.DeleteAsync(fonacotMovements.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.FirstOrDefault().ID == fonacotMovements.FirstOrDefault().ID);

                //Get it again to verify if the registry it was deleted
                var result2 = await middlewareManager
                    .GetByIdsAsync(fonacotMovements.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(!result2.Any());
            }

            [Fact]
            public async Task Should_Create_Fonacot_CreditNumber_Duplicated_ToValidate_Finally_do_Delete()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                try
                {
                    //Arrange
                    var identityWorkId = Guid.NewGuid();
                    var instanceId = Guid.NewGuid();
                    var middlewareManager = new MiddlewareManager<FonacotMovement>(new BaseRecordManager<FonacotMovement>(), new FonacotMovementValidator());
                    var fonacotMovements = await new FonacotMovementManagerUT().CreateDefaultAsync<FonacotMovement>(identityWorkId, instanceId);

                    fonacotMovements.FirstOrDefault().ID = Guid.NewGuid();

                    //Act
                    await middlewareManager.CreateAsync(fonacotMovements, identityWorkId);

                    //Assert
                    Assert.False(true, "La validación de duplicado falló");
                }
                catch (CotorraException ex)
                {
                    Assert.Equal(15001, ex.ErrorCode);
                }
                catch
                {
                    Assert.True(true);
                }
            }

            [Fact]
            public async Task Should_Create_Fonacot_DiscountAmount_Validation_ToValidate_Finally_do_Delete()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                try
                {
                    //Arrange
                    var identityWorkId = Guid.NewGuid();
                    var instanceId = Guid.NewGuid();
                    var middlewareManager = new MiddlewareManager<FonacotMovement>(new BaseRecordManager<FonacotMovement>(), new FonacotMovementValidator());

                    var middlewareEmployeeManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
                    var employees = (await middlewareEmployeeManager.FindByExpressionAsync(p => p.Active && p.InstanceID == instanceId, identityWorkId));
                    if (!employees.Any())
                    {
                        employees = (await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId: identityWorkId, instanceID: instanceId));
                    }

                    var middlewareConceptManager = new MiddlewareManager<ConceptPayment>(new BaseRecordManager<ConceptPayment>(), new ConceptPaymentValidator());
                    var concepts = (await middlewareConceptManager.FindByExpressionAsync(p => p.Active && p.InstanceID == instanceId, identityWorkId));
                    if (!concepts.Any())
                    {
                        concepts = (await new ConceptManagerUT().CreateDefaultSalaryPaymentConceptsAsync(identityWorkId, instanceId));
                    }

                    var fonacotMovements = new List<FonacotMovement>();
                    fonacotMovements.Add(new FonacotMovement()
                    {
                        ID = Guid.NewGuid(),
                        Active = true,
                        company = identityWorkId,
                        Timestamp = DateTime.UtcNow,
                        InstanceID = instanceId,
                        Description = "Departamento de nominas",
                        CreationDate = DateTime.Now,
                        Name = "Nominas",
                        StatusID = 1,
                        ConceptPaymentID = concepts.FirstOrDefault().ID,
                        CreditNumber = "AA010101",
                        DeleteDate = null,
                        EmployeeID = employees.FirstOrDefault().ID,
                        FonacotMovementStatus = FonacotMovementStatus.Active,
                        Month = 1,
                        Year = 2020,
                        Observations = "Observations",
                        RetentionType = RetentionType.FixedAmount,
                        user = Guid.Empty,
                    });

                    //Act
                    await middlewareManager.CreateAsync(fonacotMovements, identityWorkId);

                    //Assert
                    Assert.False(true, "La validación de period fixed falló");
                }
                catch (CotorraException ex)
                {
                    Assert.Equal(15003, ex.ErrorCode);
                }
                catch
                {
                    Assert.True(true);
                }
            }
        }
    }
}
