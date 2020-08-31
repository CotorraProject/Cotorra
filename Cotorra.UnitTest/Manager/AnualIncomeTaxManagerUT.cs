using CotorraNode.Common.Base.Schema;
using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.UnitTest
{
    public class AnualIncomeTaxManagerUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceId) where T : BaseEntity
        {
            var AnualIncomeTaxs = new MemoryStorageContext().GetDefaultAnualIncomeTax(identityWorkId, instanceId, Guid.Empty);

            var middlewareManager = new MiddlewareManager<AnualIncomeTax>(new BaseRecordManager<AnualIncomeTax>(),
                new AnualIncomeTaxValidator());
            await middlewareManager.CreateAsync(AnualIncomeTaxs, identityWorkId);

            return AnualIncomeTaxs as List<T>;
        }
    }
}
