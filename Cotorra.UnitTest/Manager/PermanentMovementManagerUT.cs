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
    public class PermanentMovementManagerUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceID) where T : BaseEntity
        {
            var permanentMovements = new List<PermanentMovement>();
            List<ConceptPayment> conceptPayments = null;

            var employees = await (new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId: identityWorkId, instanceID: instanceID, randomValues: true));

            var middlewareConceptPaymentManager = new MiddlewareManager<ConceptPayment>(new BaseRecordManager<ConceptPayment>(), new ConceptPaymentValidator());
            conceptPayments = await middlewareConceptPaymentManager.FindByExpressionAsync(p => p.InstanceID == instanceID, identityWorkId);
            if (!conceptPayments.Any())
            {
                conceptPayments = await (new ConceptManagerUT().CreateDefaultSalaryPaymentConceptsAsync(identityWorkId, instanceID));
            }

            permanentMovements.Add(new PermanentMovement()
            {
                AccumulatedAmount = 0,
                Active = true,
                Amount = 500,
                company = identityWorkId,
                ConceptPaymentID = conceptPayments.FirstOrDefault(p => p.ConceptType == ConceptType.SalaryPayment && p.Name == "Sueldo").ID,
                EmployeeID = employees.FirstOrDefault().ID,
                ControlNumber = 0,
                CreationDate = DateTime.UtcNow,
                DeleteDate = null,
                Description = "",
                ID = Guid.NewGuid(),
                InitialApplicationDate = DateTime.UtcNow,
                LimitAmount = 0,
                Name = "Movimiento permamente sueldito",
                PermanentMovementStatus = PermanentMovementStatus.Active,
                PermanentMovementType = PermanentMovementType.Amount,
                RegistryDate = DateTime.UtcNow,
                StatusID = 1,
                TimesApplied = 0,
                TimesToApply = 5,
                user = Guid.NewGuid(),
                InstanceID = instanceID,
                Timestamp = DateTime.UtcNow
            });

            var middlewareManager = new MiddlewareManager<PermanentMovement>(new BaseRecordManager<PermanentMovement>(), new PermanentMovementValidator());
            await middlewareManager.CreateAsync(permanentMovements, identityWorkId);

            return permanentMovements as List<T>;
        }

        [Fact]
        public async Task Should_Create_PermanentMovement_And_Get_ToValidate_Finally_do_Delete()
        {
            var txOptions = new System.Transactions.TransactionOptions();
            txOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;

            using var scope = new TransactionScope(TransactionScopeOption.Required, txOptions, TransactionScopeAsyncFlowOption.Enabled);

            //Act
            Guid identityWorkId = Guid.NewGuid();
            Guid instanceId = Guid.NewGuid();
            var permanentMovements = await new PermanentMovementManagerUT().CreateDefaultAsync<PermanentMovement>(identityWorkId, instanceId);
            var middlewareManager = new MiddlewareManager<PermanentMovement>(new BaseRecordManager<PermanentMovement>(), new PermanentMovementValidator());

            //Asserts
            //Get
            var result = await middlewareManager
                .GetByIdsAsync(permanentMovements.Select(p => p.ID).ToList(), identityWorkId);
            Assert.True(result.Any());

            //Delete
            await middlewareManager.DeleteAsync(permanentMovements.Select(p => p.ID).ToList(), identityWorkId);
            Assert.True(result.FirstOrDefault().ID == permanentMovements.FirstOrDefault().ID);

            //Get it again to verify if the registry it was deleted
            var result2 = await middlewareManager
                .GetByIdsAsync(permanentMovements.Select(p => p.ID).ToList(), identityWorkId);
            Assert.False(result2.Any());
        }
    }
}
