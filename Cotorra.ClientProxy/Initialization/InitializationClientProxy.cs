using CotorraNode.Common.Base.Schema;
using CotorraNode.Common.Config;
using CotorraNode.Common.Proxy;
using Cotorra.Client;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Threading.Tasks; 
using Newtonsoft.Json;
using Cotorra.ClientProxy;

namespace Cotorra.Client

{
    public class InitializationClientProxy : IInitializationClient
    {
        private string _authorizationHeader;
        private static string _cotorraUri;

        static InitializationClientProxy()
        {
            _cotorraUri = $"{ConfigManager.GetValue("CotorraService")}api/initialization";
        }


        public InitializationClientProxy(string authorizationHeader)
        {           
            _authorizationHeader = authorizationHeader;
        }

        public async Task<InitializationResult> InitializeAsync(string authTkn, Guid licenseServiceID, string socialReason,
            string RFC,  PayrollCompanyConfiguration payrollCompanyConfiguration, EmployerRegistration employerRegistration)
        {
            var createParmas = new InitializationParams()
            {
                AuthTkn = authTkn,
                LicenseServiceID = licenseServiceID,
                SocialReason = socialReason,
                RFC = RFC,
                PayrollCompanyConfiguration = payrollCompanyConfiguration,
                EmployerRegistration = employerRegistration
            };

            string serializedResult = await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, _authorizationHeader,
                    new Uri($"{_cotorraUri}"), new object[] { createParmas });
            if (!String.IsNullOrEmpty(serializedResult))
            {
                return JsonConvert.DeserializeObject<InitializationResult>(serializedResult);
            }
            return default;
        }
    }
}
