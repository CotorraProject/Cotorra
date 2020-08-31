using Cotorra.Core;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class VacationClientLocal : IVacationClient 
    {
        public VacationClientLocal(string authorizationHeader)
        {
        }

        public async Task<List<Vacation>> BrekeOrNotAsync(Vacation vacation, Guid identityWorkID, Guid instanceID,  HolidayPaymentConfiguration holidayPaymentConfiguration)
        {
            VacationCardManager manager = new VacationCardManager();

            return await manager.BrekeOrNotAsync(vacation, identityWorkID, instanceID,
                holidayPaymentConfiguration);
        }
         
    }
}
