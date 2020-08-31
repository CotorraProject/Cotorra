using Cotorra.Client;
using Cotorra.Core;
using Cotorra.Core.Managers;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Cotorra.UnitTest
{
    public class AuthorizationManagerUT
    {
        [Fact]
        public async Task Authorization_Should_Authorize()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var identityWorkId = Guid.NewGuid();
            var instanceID = Guid.NewGuid();

            var employee = await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId, instanceID);

            var middlewareManager = new MiddlewareManager<OverdraftDetail>(new BaseRecordManager<OverdraftDetail>(), new OverdraftDetailValidator());
            var overdraftDetails = await middlewareManager.FindByExpressionAsync(p => p.InstanceID == instanceID && p.ConceptPayment.Name == "Sueldo", identityWorkId,
                new string[] { "Overdraft" });

            var randomValue = new Random(15000);
            for (int i = 0; i < overdraftDetails.Count; i++)
            {
                overdraftDetails[i].Amount = Convert.ToDecimal(randomValue.NextDouble());
                overdraftDetails[i].Taxed = Convert.ToDecimal(randomValue.NextDouble());
                overdraftDetails[i].Exempt = Convert.ToDecimal(randomValue.NextDouble());
                overdraftDetails[i].IMSSTaxed = Convert.ToDecimal(randomValue.NextDouble());
                overdraftDetails[i].IMSSExempt = Convert.ToDecimal(randomValue.NextDouble());
            }

            var middlewareDetailManager = new MiddlewareManager<OverdraftDetail>(new BaseRecordManager<OverdraftDetail>(), new OverdraftDetailValidator());
            await middlewareDetailManager.UpdateAsync(overdraftDetails, identityWorkId);

            var authManager = new AuthorizationManager();
            var authParams = new AuthorizationParams()
            {
                IdentityWorkID = identityWorkId,
                InstanceID = instanceID,
                PeriodDetailIDToAuthorize = overdraftDetails.FirstOrDefault().Overdraft.PeriodDetailID
            };

            await authManager.AuthorizationAsync(authParams);

            var middlewareHistoricAccumulatedEmployeeManager = new MiddlewareManager<HistoricAccumulatedEmployee>(new BaseRecordManager<HistoricAccumulatedEmployee>(),
                new HistoricAccumulatedEmployeeValidator());

            //Total de acumulados por empleado
            var resultHistoric = await middlewareHistoricAccumulatedEmployeeManager.GetAllAsync(identityWorkId, instanceID, new string[] { "AccumulatedType" });
            Assert.True(resultHistoric.Count == 33);

            //Overdraft after authorization
            var overdraftDetailsAfter = await middlewareManager.FindByExpressionAsync(p => p.Overdraft.EmployeeID == employee.FirstOrDefault().ID,
                identityWorkId, new string[] { "Overdraft" });

            Assert.True(overdraftDetailsAfter.FirstOrDefault().Overdraft.PeriodDetailID != overdraftDetails.FirstOrDefault().Overdraft.PeriodDetailID);

        }

        [Fact]
        public async Task Authorization_Should_Authorize_David()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var instanceID = Guid.Parse("5A5368BD-66D5-4E31-A7D1-77C206468D72");

            var middlewareManager = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), new PeriodDetailValidator());
            var periodDetails = await middlewareManager.FindByExpressionAsync(p => p.InstanceID == instanceID
                && p.PeriodStatus == PeriodStatus.Calculating, Guid.Empty,
                null);

            var authManager = new AuthorizationManager();
            var authParams = new AuthorizationParams()
            {
                IdentityWorkID = periodDetails.FirstOrDefault().company,
                InstanceID = instanceID,
                PeriodDetailIDToAuthorize = periodDetails.FirstOrDefault().ID
            };

            var historics = await authManager.AuthorizationAsync(authParams);

            Assert.True(historics.Any());

        }
    }
}
