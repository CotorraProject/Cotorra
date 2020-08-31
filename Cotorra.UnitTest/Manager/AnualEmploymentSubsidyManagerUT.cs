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
    public class AnualEmploymentSubsidyManagerUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceId) where T : BaseEntity
        {
            var AnualEmploymentSubsidies = new MemoryStorageContext().GetDefaultAnualEmploymentSubsidy(identityWorkId, instanceId, Guid.Empty);

            var middlewareManager = new MiddlewareManager<AnualEmploymentSubsidy>(new BaseRecordManager<AnualEmploymentSubsidy>(), new AnualEmploymentSubsidyValidator());
            await middlewareManager.CreateAsync(AnualEmploymentSubsidies, identityWorkId);

            return AnualEmploymentSubsidies as List<T>;
        }
    }
}
