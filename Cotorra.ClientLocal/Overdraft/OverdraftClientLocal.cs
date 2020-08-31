using Cotorra.Core;
using Cotorra.Core.Managers;
using Cotorra.Core.Managers.Calculation;
using Cotorra.Schema;
using Cotorra.Schema.Calculation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class OverdraftClientLocal : IOverdraftClient
    {
        public OverdraftClientLocal(string authorizationHeader)
        {
        }

        public async Task<List<WorkPeriodResult>> GetWorkPeriodAsync(
            Guid companyID, Guid instanceID,
            Guid? periodDetailID = null, Guid? employeeID = null, Guid? overdraftID = null)
        {
            OverdraftManager overdraftManager = new OverdraftManager();
            return await overdraftManager.GetWorkPeriodAsync(
                companyID, instanceID,
                periodDetailID, employeeID, overdraftID);
        }
    }
}
