using Cotorra.Schema;
using Cotorra.Schema.Calculation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public interface IOverdraftClient
    {
        Task<List<WorkPeriodResult>> GetWorkPeriodAsync(
            Guid companyID, Guid instanceID,
            Guid? periodDetailID = null, Guid? employeeID = null, Guid? overdraftID = null);
    }
}
