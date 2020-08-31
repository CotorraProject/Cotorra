using AutoMapper;
using CotorraNode.Common.Config;
using CotorraNode.Common.Library.Public;
using CotorraNode.Common.Proxy;
using CotorraNode.Layer2.LicensingValidatorClient;
using CotorraNode.Layer2.Users.Schema.DTO;
using CotorraNode.Security.ACS.Client.Schema.Entities;
using CotorraNube.CommonApp.RestClient;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Cotorra.Core
{
    public class LicensingServiceCotorraProvider : ILicensingServiceProvider
    {
        public string LicensingServicesURL { get; set; }
        public string LicensingValidatorURL { get; set; }
        public string ServiceToken { get; } = "";
        public string LicensingUser { get; set; }
        public string LicensingPassword { get; set; }
        public string LoginServiceURL { get; set; }
        public Uri UriUserServices { get; set; }

        IMapper mapperForLicense;
        private Guid EmployeesFeature = Guid.Parse("2C038713-7C50-4378-BAC1-F1F2F985730F");

        internal LicensingServiceCotorraProvider()
        {

            LicensingUser = ConfigManager.GetValue("LicensingUser");
            LicensingPassword = ConfigManager.GetValue("LicensingPassword");
            LicensingServicesURL = ConfigManager.GetValue("LicensingServices");
            LicensingValidatorURL = ConfigManager.GetValue("licensingvalidatorhost");
            LoginServiceURL = ConfigManager.GetValue("LoginServiceHost");
            UriUserServices = new Uri(ConfigManager.GetValue("userhost"));
            ServiceToken = LoginAsync(LicensingUser, LicensingPassword, LoginServiceURL + "api/Auth/LoginService").Result;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<LicensesInfoDTO, LicenseInfoP>();
                cfg.CreateMap<LicenseAppsDTO, LicenseApps>();
                cfg.CreateMap<FeaturesDTO, LicensingFeatures>();
            });

            mapperForLicense = config.CreateMapper();
        }

        public async Task ConsumeEmployeeLicense(Guid LicenseID, Guid LicenseServiceID)
        {
            var licensingClient = new LicensingValidatorClient(ServiceToken,
              LicensingServicesURL, LicensingValidatorURL);
            try
            {
                licensingClient.UpdateFeature(LicenseServiceID, EmployeesFeature, 1);
            }
            catch (Exception ex)
            {
                throw new CotorraException(9991, "9991", "Haz llegado al limite máximo de colaboradores en tu licencia, te recomendamos incrementar tu plan con más colaboradores adicionales o depurar tu lista de colaboradores", ex);
            }
        }

        public async Task AumentEmployeeLicense(Guid LicenseID, Guid LicenseServiceID)
        {
            var licensingClient = new LicensingValidatorClient(ServiceToken,
              LicensingServicesURL, LicensingValidatorURL);
            licensingClient.UpdateFeature(LicenseServiceID, EmployeesFeature, -1);
        }


        public async Task<List<LicenseInfoP>> GetLicensesUserAsync(string userToken)
        {
            List<LicenseInfoP> licenses = new List<LicenseInfoP>();
            var strResult = await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.GET, userToken,
                new Uri(UriUserServices + "api/UserLicenses"));
            var result = JsonSerializer.DeserializeObject<List<LicensesInfoDTO>>(strResult);
            var dest = mapperForLicense.Map<List<LicensesInfoDTO>, List<LicenseInfoP>>(result);
            return dest;
        }

        public async Task<List<LicenseInfoP>> GetLicensesUserByAppAsync(string userToken, Guid appID)
        {
            List<LicenseInfoP> licenses = new List<LicenseInfoP>();
            var strResult = await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.GET, userToken,
                new Uri(UriUserServices + "api/UserLicenses"));
            var result = JsonSerializer.DeserializeObject<List<LicensesInfoDTO>>(strResult);

            result.ForEach(license =>
            {
                license.Apps = license.Apps.Where(x => x.AppID == appID).ToList();
            });

            var dest = mapperForLicense.Map<List<LicensesInfoDTO>, List<LicenseInfoP>>(result);
            return dest;

        }

        public static async Task<string> LoginAsync(string username, string password, string loginURI)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var credential = new ServiceCredential() { Name = username, Password = password };
            var loginParams = new LoginServiceParams();
            loginParams.ServiceCredential = credential;
            var resultdeserealizedJson = await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, "", new Uri(loginURI), loginParams);
            var resultdeserealized = JsonSerializer.DeserializeObject<LoginResult>(resultdeserealizedJson);

            return resultdeserealized.AccessToken.AuthorizationHeader;
        }


    }


}
