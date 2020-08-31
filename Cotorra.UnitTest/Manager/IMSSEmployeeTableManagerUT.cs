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
    public class IMSSEmployeeTableManagerUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceID)
        {
            var IMSSEmployeeTableLst = new List<IMSSEmployeeTable>();
            IMSSEmployeeTableLst.AddRange(new MemoryStorageContext().GetDefaultIMSSEmployeeTable(identityWorkId, instanceID, Guid.Empty));

            var middlewareManager = new MiddlewareManager<IMSSEmployeeTable>(new BaseRecordManager<IMSSEmployeeTable>(), new IMSSEmployeeTableValidator());
            await middlewareManager.CreateAsync(IMSSEmployeeTableLst, identityWorkId);

            return IMSSEmployeeTableLst as List<T>;
        }

    }
}
