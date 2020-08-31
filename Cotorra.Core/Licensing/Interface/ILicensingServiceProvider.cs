using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Core
{
    public interface ILicensingServiceProvider
    {

        Task ConsumeEmployeeLicense(Guid LicenseID, Guid LicenseServiceID);
        Task AumentEmployeeLicense(Guid LicenseID, Guid LicenseServiceID);
        Task<List<LicenseInfoP>> GetLicensesUserAsync(string userToken);
        Task<List<LicenseInfoP>> GetLicensesUserByAppAsync(string userToken, Guid appID);

    }
}
