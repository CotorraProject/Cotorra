using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.UnitTest
{
    public class MinimunSalaryManagerUT
    {
        public async Task<List<MinimunSalary>> CreateAsync(Guid identityWorkId, Guid instanceID)
        {
            var middlewareManager = new MiddlewareManager<MinimunSalary>(new BaseRecordManager<MinimunSalary>(), new MinimunSalaryValidator());
            var lstMinimunSalary = new MemoryStorageContext().GetDefaultMinimunSalaries(identityWorkId, instanceID, Guid.NewGuid());

            await middlewareManager.CreateAsync(lstMinimunSalary, identityWorkId);

            return lstMinimunSalary;
        }
    }
}
