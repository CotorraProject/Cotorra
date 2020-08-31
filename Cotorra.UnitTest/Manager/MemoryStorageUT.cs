using Cotorra.Core;
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
    public class MemoryStorageUT
    {
        [Fact]
        public void MemoryTest()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var memoryTest = new MemoryStorageContext();
            var result = memoryTest.GetDefaultMinimunSalaries(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            Assert.True(result.Any());
        }

        [Fact]
        public void GetDefaultMonthlyIncomeTax()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var memoryTest = new MemoryStorageContext();
            var result = memoryTest.GetDefaultMonthlyIncomeTax(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            Assert.True(result.Any());
        }

        [Fact]
        public async Task MemoryTest_BenefitTypeDefault()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var memoryTest = new MemoryStorageContext();
            var company = Guid.Parse("7535c4e6-4712-4dd6-955d-fca86e054d49");
            var instance = Guid.Parse("52a251f8-8fe5-4613-8e6a-181a09f431a1");
            var user = Guid.Parse("82ced4f6-a6cf-27e7-5683-95b6891b0b10");
            var result = memoryTest.GetDefaultBenefitType(company, instance, user);

            //var middlewareManager = new MiddlewareManager<BenefitType>(new BaseRecordManager<BenefitType>(),
            //   new BenefitTypeValidator());
            //await middlewareManager.CreateAsync(result, company);
            //scope.Complete();
            Assert.True(result.Any());
        }

        [Fact]
        public void MemoryTest_FunctionsDefault()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var memoryTest = new MemoryStorageContext();
        }

        [Fact]
        public void MemoryTest_ConceptDefault()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var company = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var user = Guid.NewGuid();
            var memoryTest = new MemoryStorageContext();
            var accumulates = memoryTest.GetDefaultAccumulatedType(company, instanceId, user);

            var resultSalaryPayment = memoryTest.GetDefaultConcept<ConceptPayment>(company, instanceId, user, accumulates);
            var relationships = memoryTest.GetDefaultConceptPaymentRelationship(resultSalaryPayment.Item1, accumulates);

            var accumulateType = accumulates.FirstOrDefault(p => p.Name.ToLower().Contains("directo"));
            var found = relationships.FirstOrDefault(p => p.AccumulatedTypeID == accumulateType.ID);
        }


        [Fact]
        public void MemoryTest_IncidentTypeAndAccumulatedDefault()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var memoryTest = new MemoryStorageContext();
            var resultAccumulatedTypes = memoryTest.GetDefaultAccumulatedType(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var resultIncidentTypes = memoryTest.GetDefaultIncidentType(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), resultAccumulatedTypes);
            var resultIncidentTypeRelationship = memoryTest.GetDefaultIncidentTypeRelationship(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), resultIncidentTypes);

            Assert.True(resultAccumulatedTypes.Any());
            Assert.True(resultIncidentTypes.Any());
        }
    }
}
