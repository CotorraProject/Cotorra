
using CotorraNode.Common.Config;
using Cotorra.ClientProxy;
using Cotorra.Schema;
using Cotorra.Schema.Calculation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class VacationClientProxy : IVacationClient
    {
        private readonly string _authorizationHeader;
        private readonly string _cotorraUri;

        public VacationClientProxy(string authorizationHeader)
        {
            _cotorraUri = $"{ConfigManager.GetValue("CotorraService")}api/Vacation";
            _authorizationHeader = authorizationHeader;
        }

        

        public async Task<List<Vacation>> BrekeOrNotAsync(Vacation vacation, Guid identityWorkID, Guid instanceID, HolidayPaymentConfiguration holidayPaymentConfiguration)
        {

            throw new NotSupportedException("Try local");

        }

    }
}
