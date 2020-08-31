using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cotorra.UnitTest
{
    public class HistoricEmployeeSalaryAdjustmentManagerUT
    {
        public async Task<HistoricEmployeeSalaryAdjustment> UpdateModificationDateAsync(Guid employeeID, DateTime modifiedDate,
            Guid identityWorkID)
        {
            var middlewareHistoricAdjustmentManager = new MiddlewareManager<HistoricEmployeeSalaryAdjustment>
                   (new BaseRecordManager<HistoricEmployeeSalaryAdjustment>(), new HistoricEmployeeSalaryAdjustmentValidator());
            var historics = await middlewareHistoricAdjustmentManager.FindByExpressionAsync(p => p.EmployeeID == employeeID, identityWorkID);
            historics.FirstOrDefault().ModificationDate = modifiedDate;
            await middlewareHistoricAdjustmentManager.UpdateAsync(historics, identityWorkID);

            return historics.FirstOrDefault();
        }
    }
}
