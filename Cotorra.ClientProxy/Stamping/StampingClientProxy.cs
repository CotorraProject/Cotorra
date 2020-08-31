using Cotorra.Client;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class StampingClientProxy : IStampingClient
    {
        public async Task<PayrollStampingResult> PayrollStampingAsync(PayrollStampingParams payrollStampingParams)
        {
            throw new NotImplementedException();
        }

        public async Task<PayrollStampingResult> PayrollIndividualStampingAsync(PayrollIndividualStampingParams payrollIndividualStampingParams)
        {
            throw new NotImplementedException();
        }
    }
}
