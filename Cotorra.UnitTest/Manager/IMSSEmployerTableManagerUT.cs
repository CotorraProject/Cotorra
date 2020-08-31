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
    public class IMSSEmployerTableManagerUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceID)
        {
            var IMSSEmployerTableLst = new List<IMSSEmployerTable>();
            IMSSEmployerTableLst.AddRange(new MemoryStorageContext().GetDefaultIMSSEmployerTable(identityWorkId, instanceID, Guid.Empty));

            var middlewareManager = new MiddlewareManager<IMSSEmployerTable>(new BaseRecordManager<IMSSEmployerTable>(), new IMSSEmployerTableValidator());
            await middlewareManager.CreateAsync(IMSSEmployerTableLst, identityWorkId);

            return IMSSEmployerTableLst as List<T>;
        }

    }
}
