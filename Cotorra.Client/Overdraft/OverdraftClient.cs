using Cotorra.Schema;
using Cotorra.Schema.Calculation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class OverdraftClient : IOverdraftClient
    {
        IOverdraftClient _client = null;
        public OverdraftClient(string authorizationHeader, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        {
            _client = ClientAdapterFactory2.GetInstance<IOverdraftClient>(this.GetType(), authorizationHeader, clientadapter: clientadapter);
        }

        public Task<List<WorkPeriodResult>> GetWorkPeriodAsync(Guid companyID, Guid instanceID, Guid? periodDetailID = null, Guid? employeeID = null, Guid? overdraftID = null)
        {
            return _client.GetWorkPeriodAsync(companyID, instanceID, periodDetailID, employeeID, overdraftID);
        }
    }
}

