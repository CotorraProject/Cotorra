using CotorraNode.Common.Config;
using Cotorra.Core.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Sql;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using org.mariuszgromada.math.mxparser;
using Cotorra.Schema;
using System.Linq.Expressions;
using Cotorra.Core;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace Cotorra.UnitTest.UpdateDB
{
    public class UpdateConceptsDB
    {
        [Fact]
        public async Task TM_UpdateConcepts()
        {
            var connectionString = ConfigManager.GetValue("ConfigConnectionString");
            using var context = new CotorraContext(connectionString);
            Expression<Func<ConceptPayment, bool>> expression = exp => exp.Active;
            var conceptPayments = await context.ConceptPayment.Where(expression).ToListAsync();

            var memoryStorageContext = new MemoryStorageContext();
            var accumulateds = memoryStorageContext.GetDefaultAccumulatedType(Guid.Empty, Guid.Empty, Guid.Empty);
            (var conceptPaymentsMemory, var pr) = memoryStorageContext.GetDefaultConcept<ConceptPayment>(Guid.Empty, Guid.Empty, Guid.Empty, accumulateds);

            conceptPayments.ForEach(conceptInDB =>
            {
                var conceptFoundInMemory = conceptPaymentsMemory
                .FirstOrDefault(p => p.Code == conceptInDB.Code && p.ConceptType == conceptInDB.ConceptType);
                if (null != conceptFoundInMemory)
                {
                    conceptInDB.Formula = conceptFoundInMemory.Formula;
                    conceptInDB.Formula1 = conceptFoundInMemory.Formula1;
                    conceptInDB.Formula2 = conceptFoundInMemory.Formula2;
                    conceptInDB.Formula3 = conceptFoundInMemory.Formula3;
                    conceptInDB.Formula4 = conceptFoundInMemory.Formula4;
                }
            });

            var concept22 = conceptPayments.AsParallel().Where(p => p.Code == 22 && p.ConceptType == ConceptType.SalaryPayment);

            context.UpdateRange(conceptPayments);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task TM_UpdateConcepts_SATGroup()
        {
            var connectionString = ConfigManager.GetValue("ConfigConnectionStringTest");
            using var context = new CotorraContext(connectionString);
            Expression<Func<ConceptPayment, bool>> expression = exp => exp.Active;
            var conceptPayments = await context.ConceptPayment.Where(expression).ToListAsync();

            var memoryStorageContext = new MemoryStorageContext();
            var accumulateds = memoryStorageContext.GetDefaultAccumulatedType(Guid.Empty, Guid.Empty, Guid.Empty);
            (var conceptPaymentsMemory, var pr) = memoryStorageContext.GetDefaultConcept<ConceptPayment>(Guid.Empty, Guid.Empty, Guid.Empty, accumulateds);

            conceptPayments.ForEach(conceptInDB =>
            {
                var conceptFoundInMemory = conceptPaymentsMemory
                .FirstOrDefault(p => p.Code == conceptInDB.Code && p.ConceptType == conceptInDB.ConceptType);
                if (null != conceptFoundInMemory)
                {
                    conceptInDB.SATGroupCode = conceptFoundInMemory.SATGroupCode;
                }
            });

            var concept22 = conceptPayments.AsParallel().Where(p => p.Code == 22 && p.ConceptType == ConceptType.SalaryPayment);

            context.UpdateRange(conceptPayments);
            await context.SaveChangesAsync();
        }
    }
}
