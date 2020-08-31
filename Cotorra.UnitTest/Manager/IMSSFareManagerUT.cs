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
    public class IMSSFareManagerUT
    {
        [Fact]
        public async Task Client_Should_Call_Dummy_Async()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var iMSSFare = new List<IMSSFare>();
            var companyId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            iMSSFare = new MemoryStorageContext().GetDefaultIMSSFare(companyId, instanceId, userId);

            var middlewareManager = new MiddlewareManager<IMSSFare>(new BaseRecordManager<IMSSFare>(), new IMSSFareValidator());

            //Act
            await middlewareManager.CreateAsync(iMSSFare, companyId);

            //Asserts
            //Get
            var result = await middlewareManager
                .GetByIdsAsync(iMSSFare.Select(p => p.ID).ToList(), companyId);
            Assert.True(result.Any());

            //Delete
            await middlewareManager.DeleteAsync(iMSSFare.Select(p => p.ID).ToList(), companyId);

            //Get it again to verify if the registry it was deleted
            var result2 = await middlewareManager
                .GetByIdsAsync(iMSSFare.Select(p => p.ID).ToList(), companyId);
            Assert.True(!result2.Any());

        }
    }
}
