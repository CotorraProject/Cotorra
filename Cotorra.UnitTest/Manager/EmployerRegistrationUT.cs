using CotorraNode.Common.Base.Schema;
using Cotorra.Client;
using Cotorra.Core;
using Cotorra.Core.Utils;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Cotorra.UnitTest
{
    public class EmployerRegistrationUT
    {

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceID) where T : BaseEntity
        {
            var employerRegistrations = new List<EmployerRegistration>();
            var addressID = Guid.NewGuid();
            employerRegistrations.Add(new EmployerRegistration()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceID,
                Description = "OSCAR KALA HAAK",              
                CreationDate = DateTime.Now,
                user = Guid.NewGuid(),
                Name = "EmplorerTest",
                StatusID = 1,

                Code = "R1723289120",              
                RiskClass = "Risk",
                RiskClassFraction = 0.20m,    
                AddressID = addressID,

                Address = new Schema.Address()
                {
                    ID = addressID,
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = instanceID,
                    Description = "Direccion Registro Patronal",
                    CreationDate = DateTime.Now,
                    user = Guid.NewGuid(),
                    Name = "Direccion Registro Patronal",
                    StatusID = 1,
                    ExteriorNumber = "123",
                    FederalEntity = "Jalisco",
                    InteriorNumber = "145",
                    Municipality = "Guadalajara",
                    Reference = "Reference",
                    Street = "Albert",
                    Suburb = "Colomos",
                    ZipCode = "44660",
                }
            });


            var middlewareManager = new MiddlewareManager<EmployerRegistration>(new BaseRecordManager<EmployerRegistration>(), new EmployerRegistrationValidator());
            await middlewareManager.CreateAsync(employerRegistrations, identityWorkId);

            var addressID1 = Guid.NewGuid();
            var payrollCompanyConfiguration = new List<PayrollCompanyConfiguration>();
            payrollCompanyConfiguration.Add(new PayrollCompanyConfiguration()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceID,
                user = Guid.NewGuid(),
                Name = "Company Config",
                Description = "Configuration",
                StatusID = 1,
                AdjustmentPay = AdjustmentPay_16Days_Febrary.PayCalendarDays,
                CreationDate = DateTime.UtcNow,
                CurrentExerciseYear = DateTime.UtcNow.Year,
                CurrencyID = Guid.Parse("57B07610-ED2A-40F5-8093-FA3EAA38B41D"), //pesos
                DeleteDate = null,
                PaymentDays = 16m,
                PaymentPeriodicity = PaymentPeriodicity.Biweekly,
                PeriodInitialDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),
                StartDate = new DateTime(DateTime.UtcNow.Year, 1, 1),
                RFC = "KAHO641101B39",
                SocialReason = "OSCAR KALA HAAK",
                CURP = "KAHO641101HDFLKS06",
                FiscalRegime = FiscalRegime.FiscalIncorporationPerson,
                SalaryZone = SalaryZone.ZoneA,
                NonDeducibleFactor = 0.25M,
                CurrentPeriod = 1,
                AddressID = addressID1,
                Address = new Schema.Address()
                {
                    ID = addressID1,
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = instanceID,
                    Description = "Direccion Registro Patronal",
                    CreationDate = DateTime.Now,
                    user = Guid.NewGuid(),
                    Name = "Direccion Registro Patronal",
                    StatusID = 1,
                    ExteriorNumber = "123",
                    FederalEntity = "Jalisco",
                    InteriorNumber = "145",
                    Municipality = "Guadalajara",
                    Reference = "Reference",
                    Street = "Albert",
                    Suburb = "Colomos",
                    ZipCode = "44660",
                }
            });

            var middlewarePayrollCompanyConfigurationManager = new MiddlewareManager<PayrollCompanyConfiguration>(new BaseRecordManager<PayrollCompanyConfiguration>(),
                new PayrollCompanyConfigurationValidator());
            await middlewarePayrollCompanyConfigurationManager.CreateAsync(payrollCompanyConfiguration, identityWorkId);


            var fileCER = File.ReadAllBytes(Path.Combine(AssemblyDirectory, "fiscal\\cfdi33nom12\\KAHO641101B39.cer"));
            var fileKEY = File.ReadAllBytes(Path.Combine(AssemblyDirectory, "fiscal\\cfdi33nom12\\KAHO641101B39.key"));

            var digitalSign = new DigitalSign.DigitalSign();
            var employerFiscalInformations = new List<EmployerFiscalInformation>();

            var clsCryptoToCreate = new ClsCrypto(
                        identityWorkId.ToString().ToLower().Replace("-", ""),
                        instanceID.ToString().ToLower().Replace("-", ""),
                        instanceID.ToString().ToLower().Replace("-", "").Substring(0, 19));

            var fileCerEncrypted = clsCryptoToCreate.Encrypt(Convert.ToBase64String(fileCER));
            var fileKeyEncrypted = clsCryptoToCreate.Encrypt(Convert.ToBase64String(fileKEY));
            var passEncrypted = StringCipher.Encrypt("12345678a");

            employerFiscalInformations.Add(new EmployerFiscalInformation()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceID,
                Description = "Emplorer FiscalInfoTest",
                CreationDate = DateTime.Now,
                user = Guid.NewGuid(),
                Name = "EmplorerFiscalInfoTest",
                RFC= payrollCompanyConfiguration.FirstOrDefault().RFC,
                StatusID = 1,
                CertificateCER = fileCerEncrypted,
                CertificateKEY = fileKeyEncrypted,
                CertificatePwd = passEncrypted,
                CertificateNumber = digitalSign.GetCertificateNumber(fileCER),
                StartDate = digitalSign.GetExpirationDate(fileCER).Item1,
                EndDate = digitalSign.GetExpirationDate(fileCER).Item2                
            }); 

            var middlewareManagerFiscalInfo = new MiddlewareManager<EmployerFiscalInformation>(new BaseRecordManager<EmployerFiscalInformation>(),
                new EmployerFiscalInformationValidator());
            await middlewareManagerFiscalInfo.CreateAsync(employerFiscalInformations, identityWorkId);


            return employerRegistrations as List<T>;
        }


        public class Create
        {
            [Fact]
            public async Task Client_Should_Create()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();
                var employerRegistrations = await new EmployerRegistrationUT().CreateDefaultAsync<EmployerRegistration>(identityWorkId, instanceId);

                var client = new Client<EmployerRegistration>("", ClientConfiguration.ClientAdapter.Local);

                //Asserts
                //Get
                var result = await client
                    .GetByIdsAsync(employerRegistrations.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());

                //Delete
                await client.DeleteAsync(employerRegistrations.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.FirstOrDefault().ID == employerRegistrations.FirstOrDefault().ID);

                //Get it again to verify if the registry it was deleted
                var result2 = await client
                    .GetByIdsAsync(employerRegistrations.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(!result2.Any());

            }
        }
    }
}