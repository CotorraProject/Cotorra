using Cotorra.Core;
using Cotorra.Schema;
using System;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class InitializationClientLocal: IInitializationClient
    {

        public InitializationClientLocal(string authorizationHeader)
        {
           
        }

        public async Task<InitializationResult> InitializeAsync(string authTkn, Guid licenseServiceID, string socialReason, string RFC,
           PayrollCompanyConfiguration payrollCompanyConfiguration, EmployerRegistration employerRegistration)

        {
            var mgr = new InitializationManager();
            return await  mgr.InitializeAsync(new InitializationParams
            {
                AuthTkn = authTkn,
                LicenseServiceID = licenseServiceID,
                SocialReason = socialReason,
                RFC = RFC,
                PayrollCompanyConfiguration = payrollCompanyConfiguration,
                EmployerRegistration = employerRegistration
            });
        } 
    }
}
