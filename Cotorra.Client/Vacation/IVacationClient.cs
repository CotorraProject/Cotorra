using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public interface IVacationClient
    {
        Task<List<Vacation>> BrekeOrNotAsync(Vacation vacation, Guid identityWorkID, Guid instanceID,
              HolidayPaymentConfiguration holidayPaymentConfiguration);
    }
}
