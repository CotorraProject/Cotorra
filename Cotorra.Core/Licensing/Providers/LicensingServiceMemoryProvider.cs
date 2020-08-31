using Cotorra.Schema;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Core
{
    public class LicensingServiceMemoryProvider : ILicensingServiceProvider
    {
        public async Task ConsumeEmployeeLicense(Guid LicenseID, Guid LicenseServiceID)
        {
            throw new NotImplementedException();
        }

        public Task<List<LicenseInfoP>> GetLicensesUserAsync(string userToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<LicenseInfoP>> GetLicensesUserByAppAsync(string userToken, Guid appID)
        {
            throw new NotImplementedException();
        }

        public async Task AumentEmployeeLicense(Guid LicenseID, Guid LicenseServiceID)
        {
            throw new NotImplementedException();
        }
    }
}
