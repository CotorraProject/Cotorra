using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Core
{
    public class LicensingService : ILicensingServiceProvider
    {
        public ILicensingServiceProvider _serviceProvider;

        public LicensingService(ILicensingServiceProvider serviceProvider)  
        {
            _serviceProvider = serviceProvider;
        }

        public async Task  ConsumeEmployeeLicense(Guid LicenseID, Guid LicenseServiceID)
        {
              await _serviceProvider.ConsumeEmployeeLicense(LicenseID, LicenseServiceID);
        }

        public Task<List<LicenseInfoP>> GetLicensesUserAsync(string userToken)
        {
            return _serviceProvider.GetLicensesUserAsync(userToken);
        }

        public Task<List<LicenseInfoP>> GetLicensesUserByAppAsync(string userToken, Guid appID)
        {
            return _serviceProvider.GetLicensesUserByAppAsync(userToken, appID);
        }

        public async Task AumentEmployeeLicense(Guid LicenseID, Guid LicenseServiceID)
        {
              await _serviceProvider.AumentEmployeeLicense(LicenseID, LicenseServiceID);
        }
    }
}
