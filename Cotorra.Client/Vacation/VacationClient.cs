using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class  VacationClient : IVacationClient
    {
        private readonly IVacationClient _client;

        public VacationClient(string authorizationHeader, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        {
            _client = ClientAdapterFactory2.GetInstance<IVacationClient>(this.GetType(), authorizationHeader, clientadapter: clientadapter);
        }

        public Task<List<Vacation>> BrekeOrNotAsync(Vacation vacation, Guid identityWorkID, Guid instanceID, HolidayPaymentConfiguration holidayPaymentConfiguration)
        {
            return _client.BrekeOrNotAsync(vacation, identityWorkID, instanceID,  holidayPaymentConfiguration);
        }

       
    }
}

