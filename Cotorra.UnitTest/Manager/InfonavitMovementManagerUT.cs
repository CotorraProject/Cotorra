using CotorraNode.Common.Base.Schema;
using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Cotorra.UnitTest
{
    public class InfonavitMovementManagerUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceId) where T : BaseEntity
        {
            var employees = await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId, instanceId, randomValues: true);

            var infonavitMovements = new List<InfonavitMovement>();
            infonavitMovements.Add(new InfonavitMovement()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceId,
                Description = $"Credito Infonavit",
                CreationDate = DateTime.Now,
                Name = $"{DateTime.Now.Year}",
                user = Guid.NewGuid(),
                StatusID = 1,
                AccumulatedAmount = 0,
                AppliedTimes = 0,
                CreditNumber = "aaa1452",
                EmployeeID = employees.FirstOrDefault().ID,
                MonthlyFactor = 20,
                IncludeInsurancePayment_D14 = true,
                InfonavitCreditType = InfonavitCreditType.DiscountFactor_D15,
                InfonavitStatus = true,
                InitialApplicationDate = DateTime.UtcNow,
                RegisterDate = DateTime.UtcNow,
                DeleteDate = null
            });

            var middlewareManager = new MiddlewareManager<InfonavitMovement>(new BaseRecordManager<InfonavitMovement>(), 
                new InfonavitMovementValidator());
            await middlewareManager.CreateAsync(infonavitMovements, identityWorkId);

            return infonavitMovements as List<T>;
        }

        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceId, 
            Guid employeeID, decimal monthlyFactor, DateTime initialApplicationDate) where T : BaseEntity
        {
            var infonavitMovements = new List<InfonavitMovement>();
            infonavitMovements.Add(new InfonavitMovement()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceId,
                Description = $"Credito Infonavit",
                CreationDate = DateTime.Now,
                Name = $"{DateTime.Now.Year}",
                user = Guid.NewGuid(),
                StatusID = 1,
                AccumulatedAmount = 0,
                AppliedTimes = 0,
                CreditNumber = "aaa1452",
                EmployeeID = employeeID,
                MonthlyFactor = 20,
                IncludeInsurancePayment_D14 = true,
                InfonavitCreditType = InfonavitCreditType.DiscountFactor_D15,
                InfonavitStatus = true,
                InitialApplicationDate = initialApplicationDate,
                RegisterDate = DateTime.UtcNow,
                DeleteDate = null
            });

            var middlewareManager = new MiddlewareManager<InfonavitMovement>(new BaseRecordManager<InfonavitMovement>(),
                new InfonavitMovementValidator());
            await middlewareManager.CreateAsync(infonavitMovements, identityWorkId);

            return infonavitMovements as List<T>;
        }


        [Fact]
        public async Task Should_Create_InfonavitMovement()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            //Arrange
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();

            //Act
            var infonavitMovement = await CreateDefaultAsync<InfonavitMovement>(identityWorkId, instanceId);

            //Assert
            var middlewareManager = new MiddlewareManager<InfonavitMovement>(new BaseRecordManager<InfonavitMovement>(), 
                new InfonavitMovementValidator());
            var found = await middlewareManager.GetByIdsAsync(new List<Guid>() { infonavitMovement.FirstOrDefault().ID }, 
                identityWorkId, null);
            Assert.True(found.Any());

            //Delete test
            await middlewareManager.DeleteAsync(new List<Guid>() { found.FirstOrDefault().ID }, identityWorkId);
        }
    }
}
