using CotorraNode.Common.Base.Schema;
using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Cotorra.UnitTest.UtilityTest
{
    public class CreateEmployeesUT
    {

        public async Task<List<T>> CreateDefaultAsyncList<T>(Guid identityWorkId, Guid instanceID,
          string rfc = "RAQÑ7701212M3", string nss = "833531862",
          string curp = "KAHO641101HDFLKS06", string name = "Jaime",
          string firstLastName = "Duende",
          string secondLastName = "Gongora y Gongora",
          string code = "1",
          bool randomValues = false, int laps = 1) where T : BaseEntity
        {
            //Act Dependencies
            var deparments = Guid.Parse("1AB7ADFE-9E51-47A2-BF6E-38381415BFD1");
            var jobPositions = Guid.Parse("DC6FA1A7-59DD-4E29-BE13-1C97404EFBF8");
            var periodManager = new MiddlewareManager<Period>(new BaseRecordManager<Period>(), new PeriodValidator());
            var period = await periodManager.GetAllAsync(identityWorkId, instanceID);
            var periodType = new List<PeriodType>();
            if (!period.Any())
            {
                var fiscalYear = DateTime.Now.Year;
                var initialDate = new DateTime(fiscalYear, 1, 1);
                var finalDate = new DateTime(fiscalYear, 12, 31);

                var periodTypeManager = new MiddlewareManager<PeriodType>(new BaseRecordManager<PeriodType>(),
                    new PeriodTypeValidator());
                periodType = new MemoryStorageContext().GetDefaultPeriodType(identityWorkId, instanceID, Guid.NewGuid(), PaymentPeriodicity.Biweekly, 15M,
                    AdjustmentPay_16Days_Febrary.PayCalendarDays, forceUpdate: true);
                await periodTypeManager.CreateAsync(periodType, identityWorkId);
                period = new MemoryStorageContext().GetDefaultPeriod(identityWorkId, instanceID, Guid.NewGuid(), initialDate, finalDate, fiscalYear, periodType.FirstOrDefault(p => p.PaymentPeriodicity == PaymentPeriodicity.Biweekly));
                await periodManager.CreateAsync(period, identityWorkId);
            }

            var workshifts = await new WorkshiftManagerUT().CreateDefaultAsync<Workshift>(identityWorkId, instanceID);
            var emploRegID = Guid.Parse("AFBBA215-7B9F-4EE6-AE4E-12721485F7B1");
            //Arrange
            var employees = new List<Employee>();

            var rfcs = FiscalStringsUtils.GenerateRFCs(laps);
            var curps = FiscalStringsUtils.GenerateCURPs(laps);
            var NSSs = FiscalStringsUtils.GenerateNSS(laps);

            var actualCode = 101;
            for (int i = 0; i < laps; i++)
            {
                if (randomValues)
                {
                    Random _random = new Random();
                    var num = _random.Next(0, 90);
                    rfc = rfcs[i];
                    curp = curps[i];
                    nss = NSSs[i];
                    code = (actualCode++).ToString();
                    firstLastName = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 5);
                }

                employees.Add(new Employee()
                {
                    ID = Guid.NewGuid(),
                    Active = true,
                    company = identityWorkId,
                    RFC = rfc,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = instanceID,
                    user = Guid.NewGuid(),
                    BirthDate = DateTime.Now,
                    NSS = nss,
                    BornPlace = "Guadalajara",
                    CivilStatus = CivilStatus.Viudo,
                    Code = code,
                    CURP = curp,
                    DeleteDate = null,
                    Name = name,
                    FirstLastName = firstLastName,
                    SecondLastName = secondLastName,
                    Gender = Gender.Male,
                    EntryDate = new DateTime(DateTime.UtcNow.Year - 1, 1, 1),
                    JobPositionID = jobPositions,
                    DepartmentID = deparments,
                    PeriodTypeID = period.FirstOrDefault().PeriodTypeID,
                    WorkshiftID = workshifts.FirstOrDefault().ID,
                    EmployerRegistrationID = emploRegID,
                    BankAccount = "123456789145214587",
                    EmployeeTrustLevel = EmployeeTrustLevel.Trusted,
                    StatusID = 1,
                    Description = "desc",
                    RegimeType = EmployeeRegimeType.Salaries,
                    ContractType = ContractType.IndefiniteTermEmploymentContract,
                    DailySalary = 2150.45m,
                    ContributionBase = BaseQuotation.Fixed,
                    SalaryZone = SalaryZone.ZoneA,
                    PaymentBase = PaymentBase.SalaryComission,
                    SBCFixedPart = 2150.45m,
                    SBCMax25UMA = 2112.25m,
                    SBCVariablePart = 1.54m,
                    LocalStatus = CotorriaStatus.Active,
                    LastStatusChange = DateTime.Now
                });

            }
            //Act
            var middlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
            await middlewareManager.CreateAsync(employees, new LicenseParams() { IdentityWorkID = identityWorkId });

            return employees as List<T>;
        }

        [Fact]
        public async Task Should_Create_LotsOfEmployees()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            Guid identityWorkId = Guid.Parse("965572C8-BC76-4151-BE35-B70C9D257EC6");
            Guid instanceID = Guid.Parse("D368412B-AB81-4B65-ADEF-B654EE2C6CCA");
            var employee = await new CreateEmployeesUT().CreateDefaultAsyncList<Employee>(
             identityWorkId, instanceID, "1500", randomValues: true, laps: 80);
            scope.Complete();
        }
    }
}
