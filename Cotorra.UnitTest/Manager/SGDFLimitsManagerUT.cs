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
    public class SGDFLimitsManagerUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceID)
        {
            var SGDFLimitsLst = new List<SGDFLimits>();
            SGDFLimitsLst.AddRange(new MemoryStorageContext().GetDefaultSGDFLimits(identityWorkId, instanceID, Guid.Empty));

            var middlewareManager = new MiddlewareManager<SGDFLimits>(new BaseRecordManager<SGDFLimits>(), new SGDFLimitsValidator());
            await middlewareManager.CreateAsync(SGDFLimitsLst, identityWorkId);

            return SGDFLimitsLst as List<T>;
        }

    }
}
