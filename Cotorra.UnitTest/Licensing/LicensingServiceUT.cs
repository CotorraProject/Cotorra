using Cotorra.Core;
using System;
using Xunit;
using System.Threading.Tasks;
using CotorraNode.Common.Config;
using CotorraNode.Security.ACS.Client.Schema.Entities;
using CotorraNode.Common.Library.Public;
using System.Net;
using CotorraNode.Common.Proxy;
using CotorraNube.CommonApp.RestClient;
using System.Linq;

namespace Cotorra.UnitTest
{
    public class LicensingServiceUT
    {


        public class CotorraLicensimgProvider
        {
            Guid CotorriaAppID = Guid.Parse("09bf71d1-aab0-42e5-b983-8208265b5930");
            Guid EmployeesFeature = Guid.Parse("2C038713-7C50-4378-BAC1-F1F2F985730F");

            [Fact]
            public async Task Should_Get_Cotorria_License_Then_Consume_Employeee_Then_Unconsume_Employee()
            {
                var service = new LicensingService(LicensingServiceProviderFactory.GetProvider());
                var LoginServiceURL = ConfigManager.GetValue("LoginServiceHost");

                var token = await LicensingServiceCotorraProviderUT.LoginAsync("luis.tejeda@cotorrai.com", "Xq63zqw@", LoginServiceURL + "api/Auth/LoginUser");
                //Get license info
                var licenseinfo = await service.GetLicensesUserByAppAsync(token, CotorriaAppID);
                Assert.NotEmpty(licenseinfo);
                var license = licenseinfo.FirstOrDefault();
                var CotorriaApp = license.Apps.Where(x => x.AppID == CotorriaAppID).FirstOrDefault();
                var licenseServiceID = CotorriaApp.LicenseServiceID;
                // get feature info
                var employeeFeature = CotorriaApp.Features.FirstOrDefault(x => x.ID == EmployeesFeature);
                var initialEmployeesApplied = employeeFeature.Applied;

                //Act
                 await service.ConsumeEmployeeLicense(license.LicenseID, licenseServiceID);

                //Get For Assert
                licenseinfo = await service.GetLicensesUserByAppAsync(token, CotorriaAppID);
                Assert.NotEmpty(licenseinfo);
                license = licenseinfo.FirstOrDefault();
                CotorriaApp = license.Apps.Where(x => x.AppID == CotorriaAppID).FirstOrDefault();
                licenseServiceID = CotorriaApp.LicenseServiceID;
                employeeFeature = CotorriaApp.Features.FirstOrDefault(x => x.ID == EmployeesFeature);
                var actualEmployees = employeeFeature.Applied;

                //Asert
                Assert.Equal(initialEmployeesApplied + 1, actualEmployees);

                //Act Again
                await service.AumentEmployeeLicense(license.LicenseID, licenseServiceID);


                //Get For Assert again
                licenseinfo = await service.GetLicensesUserByAppAsync(token, CotorriaAppID);
                Assert.NotEmpty(licenseinfo);
                license = licenseinfo.FirstOrDefault();
                CotorriaApp = license.Apps.Where(x => x.AppID == CotorriaAppID).FirstOrDefault();
                licenseServiceID = CotorriaApp.LicenseServiceID;
                employeeFeature = CotorriaApp.Features.FirstOrDefault(x => x.ID == EmployeesFeature);
                actualEmployees = employeeFeature.Applied;

                Assert.Equal(initialEmployeesApplied, actualEmployees);

            }


        }

    }

    public class LicensingServiceCotorraProviderUT
    {
        Guid CotorriaAppID = Guid.Parse("09bf71d1-aab0-42e5-b983-8208265b5930");
        Guid EmployeesFeature = Guid.Parse("2C038713-7C50-4378-BAC1-F1F2F985730F");

        [Fact]
        public void Should_Be_Logged_in()
        {
            var service = LicensingServiceProviderFactory.GetProvider();
            Assert.NotEmpty((service as LicensingServiceCotorraProvider).ServiceToken);
        }


        [Fact]
        public async Task Should_Get_All_Licenses_For_Specific_User()
        {
            var service = LicensingServiceProviderFactory.GetProvider();

            var LoginServiceURL = ConfigManager.GetValue("LoginServiceHost");
            Assert.NotEmpty((service as LicensingServiceCotorraProvider).ServiceToken);
            var token = await LoginAsync("luis.tejeda@cotorrai.com", "Xq63zqw@", LoginServiceURL + "api/Auth/LoginUser");
            var licenseinfo = await service.GetLicensesUserAsync(token);

            Assert.NotEmpty(licenseinfo);
        }

        [Fact]
        public async Task Should_Get_Cotorria_License_For_Specific_User()
        {
            var service = LicensingServiceProviderFactory.GetProvider();

            var LoginServiceURL = ConfigManager.GetValue("LoginServiceHost");
            Assert.NotEmpty((service as LicensingServiceCotorraProvider).ServiceToken);

            var token = await LoginAsync("luis.tejeda@cotorrai.com", "Xq63zqw@", LoginServiceURL + "api/Auth/LoginUser");
            var licenseinfo = await service.GetLicensesUserByAppAsync(token, CotorriaAppID);

            Assert.NotEmpty(licenseinfo);
            Assert.DoesNotContain(licenseinfo.SelectMany(x => x.Apps), p => p.AppID != CotorriaAppID);
        }



        [Fact]
        public async Task Should_Get_Cotorria_License_Then_Consume_Employeee_Then_Unconsume_Employee()
        {
            var service = LicensingServiceProviderFactory.GetProvider();

            var LoginServiceURL = ConfigManager.GetValue("LoginServiceHost");
            Assert.NotEmpty((service as LicensingServiceCotorraProvider).ServiceToken);

            var token = await LoginAsync("luis.tejeda@cotorrai.com", "Xq63zqw@", LoginServiceURL + "api/Auth/LoginUser");
            //Get license info
            var licenseinfo = await service.GetLicensesUserByAppAsync(token, CotorriaAppID);
            Assert.NotEmpty(licenseinfo);
            var license = licenseinfo.FirstOrDefault();
            var CotorriaApp = license.Apps.Where(x => x.AppID == CotorriaAppID).FirstOrDefault();
            var licenseServiceID = CotorriaApp.LicenseServiceID;
            // get feature info
            var employeeFeature = CotorriaApp.Features.FirstOrDefault(x => x.ID == EmployeesFeature);
            var initialEmployeesApplied = employeeFeature.Applied;

            //Act
            await service.ConsumeEmployeeLicense(license.LicenseID, licenseServiceID);

            //Get For Assert
            licenseinfo = await service.GetLicensesUserByAppAsync(token, CotorriaAppID);
            Assert.NotEmpty(licenseinfo);
            license = licenseinfo.FirstOrDefault();
            CotorriaApp = license.Apps.Where(x => x.AppID == CotorriaAppID).FirstOrDefault();
            licenseServiceID = CotorriaApp.LicenseServiceID;
            employeeFeature = CotorriaApp.Features.FirstOrDefault(x => x.ID == EmployeesFeature);
            var actualEmployees = employeeFeature.Applied;

            //Asert
            Assert.Equal(initialEmployeesApplied + 1, actualEmployees);

            //Act Again
            await service.AumentEmployeeLicense(license.LicenseID, licenseServiceID);


            //Get For Assert again
            licenseinfo = await service.GetLicensesUserByAppAsync(token, CotorriaAppID);
            Assert.NotEmpty(licenseinfo);
            license = licenseinfo.FirstOrDefault();
            CotorriaApp = license.Apps.Where(x => x.AppID == CotorriaAppID).FirstOrDefault();
            licenseServiceID = CotorriaApp.LicenseServiceID;
            employeeFeature = CotorriaApp.Features.FirstOrDefault(x => x.ID == EmployeesFeature);
            actualEmployees = employeeFeature.Applied;

            Assert.Equal(initialEmployeesApplied, actualEmployees);

        }




        public static async Task<string> LoginAsync(string username, string password, string loginURI)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var userCredentials = new UserCredential() { Name = username, Password = password };
            var loginUserParams = new LoginUserParams();
            loginUserParams.UserCredential = userCredentials;
            var resultdeserealizedJson = await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, "", new Uri(loginURI), loginUserParams);
            var resultdeserealized = JsonSerializer.DeserializeObject<LoginResult>(resultdeserealizedJson);

            return resultdeserealized.AccessToken.AuthorizationHeader;
        }


    }
}
