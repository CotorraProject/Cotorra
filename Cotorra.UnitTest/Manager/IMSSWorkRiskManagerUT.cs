using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.UnitTest
{
    public class IMSSWorkRiskManagerUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceID)
        {
            var IMSSWorkRiskTableLst = new List<IMSSWorkRisk>();
            IMSSWorkRiskTableLst.AddRange(new MemoryStorageContext().GetDefaultIMSSWorkRisk(identityWorkId, instanceID, Guid.Empty));

            var middlewareManager = new MiddlewareManager<IMSSWorkRisk>(new BaseRecordManager<IMSSWorkRisk>(), new IMSSWorkRiskValidator());
            await middlewareManager.CreateAsync(IMSSWorkRiskTableLst, identityWorkId);

            return IMSSWorkRiskTableLst as List<T>;
        }
    }
}
