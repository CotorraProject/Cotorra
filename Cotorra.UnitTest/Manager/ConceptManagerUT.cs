using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Cotorra.Core.Extensions;
using System.Transactions;

namespace Cotorra.UnitTest
{
    public class ConceptManagerUT
    {
        public async Task<List<ConceptPayment>> CreateDefaultSalaryPaymentConceptsAsync(Guid identityWorkId, Guid instanceID)
        {
            var memoryStorageContext = new MemoryStorageContext();

            var middlewareAccumulates = new MiddlewareManager<AccumulatedType>(new BaseRecordManager<AccumulatedType>(), new AccumulatedTypeValidator());
            var accumulatesTypes = await middlewareAccumulates.GetAllAsync(identityWorkId, instanceID);

            if (!accumulatesTypes.Any())
            {
                accumulatesTypes = memoryStorageContext.GetDefaultAccumulatedType(identityWorkId, instanceID, Guid.NewGuid());
                await middlewareAccumulates.CreateAsync(accumulatesTypes, identityWorkId);
            }

            var resultTuple = memoryStorageContext.GetDefaultConcept<ConceptPayment>(identityWorkId, instanceID, Guid.NewGuid(), accumulatesTypes);
            var concepts = resultTuple.Item1
                .Cast<ConceptPayment>().ToList();

            var middlewareManagerConceptPaymentRelationship = new MiddlewareManager<ConceptPaymentRelationship>(new BaseRecordManager<ConceptPaymentRelationship>(),
             new ConceptPaymentRelationshipValidator());

            var middlewareConcept = new MiddlewareManager<ConceptPayment>(new BaseRecordManager<ConceptPayment>(), new ConceptPaymentValidator());
            await middlewareConcept.CreateAsync(concepts, identityWorkId);

            var relationships = resultTuple.Item2.Cast<ConceptPaymentRelationship>().ToList();
            await middlewareManagerConceptPaymentRelationship.CreateAsync(relationships, identityWorkId);

            return concepts;
        }

        //[Fact]
        //public async Task Should_Create_Relationship_FromConceptPayment_PreExistent()
        //{
        //    var txOptions = new System.Transactions.TransactionOptions();
        //    txOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
        //    using var scope = new TransactionScope(TransactionScopeOption.Required, txOptions, TransactionScopeAsyncFlowOption.Enabled);

        //    var middlewareAccumulates = new MiddlewareManager<AccumulatedType>(new BaseRecordManager<AccumulatedType>(), new AccumulatedTypeValidator());
        //    var accumulates = await middlewareAccumulates.FindByExpressionAsync(p => p.Active, Guid.Empty);
        //    var accumulatedTypesDistincts = accumulates.GroupBy(p => p.InstanceID).ToList();

        //    var middlewareManagerConceptPaymentRelationship = new MiddlewareManager<ConceptPaymentRelationship>(new BaseRecordManager<ConceptPaymentRelationship>(),
        //    new ConceptPaymentRelationshipValidator());
            
        //    accumulatedTypesDistincts.ForEach(p => {
        //        var memoryStorageContext = new MemoryStorageContext();
        //        var middlewareConcept = new MiddlewareManager<ConceptPayment>(new BaseRecordManager<ConceptPayment>(), new ConceptPaymentValidator());
        //        var allconcepts = middlewareConcept.FindByExpressionAsync(r => r.InstanceID == p.Key, p.FirstOrDefault().company).Result;
        //        var result = memoryStorageContext.GetDefaultConcept<ConceptPayment>(p.FirstOrDefault().company, p.Key, p.FirstOrDefault().user, p.ToList());

        //        var relationships = new List<ConceptPaymentRelationship>();
        //        result.Item2.ForEach(q =>
        //        {
        //            var conceptPaymentID = q.ConceptPaymentID;
        //            var conceptPrevious = result.Item1.Cast<ConceptPayment>().FirstOrDefault(o=> o.ID == conceptPaymentID);
        //            var databaseConcept = allconcepts.FirstOrDefault(item => item.Name == conceptPrevious.Name);

        //            var newRelationship = q;
        //            newRelationship.ConceptPaymentID = databaseConcept.ID;
        //            relationships.Add(newRelationship);
        //        });

        //        middlewareManagerConceptPaymentRelationship.CreateAsync(relationships, p.FirstOrDefault().company).Wait();
        //    });           

        //}

    }
}
