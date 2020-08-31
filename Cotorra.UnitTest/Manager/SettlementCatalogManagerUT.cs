using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.UnitTest
{
    public class SettlementCatalogManagerUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceID)
        {
            var settlementList = new List<SettlementCatalog>();
            settlementList.AddRange(new MemoryStorageContext().GetDefaultSettlementCatalogTable(identityWorkId, instanceID, Guid.Empty));

            var middlewareManager = new MiddlewareManager<SettlementCatalog>(new BaseRecordManager<SettlementCatalog>(), 
                new SettlementCatalogValidator());
            await middlewareManager.CreateAsync(settlementList, identityWorkId);

            return settlementList as List<T>;
        }
    }
}
