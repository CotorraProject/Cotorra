using Cotorra.Schema;
using System;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public interface IInitializationClient
    {
        Task<InitializationResult> InitializeAsync(string authTkn, Guid licenseServiceID, string socialReason,
            string RFC, PayrollCompanyConfiguration payrollCompanyConfiguration, EmployerRegistration employerRegistration);

    }
}
