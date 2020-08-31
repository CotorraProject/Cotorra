using Cotorra.Core;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Cotorra.Core.Extensions;
using Cotorra.Core.Validator;
using CotorraNode.Common.Config;
using Microsoft.Extensions.Configuration;
using System.Transactions;
using CotorraNode.Common.Base.Schema;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Cotorra.UnitTest.Manager;
using Cotorra.Client;

namespace Cotorra.UnitTest
{
    public class EmployeeManagerUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceID,
            string rfc = "RAQÑ7701212M3", string nss = "833531862",
            string curp = "KAHO641101HDFLKS06", string name = "Jaime",
            string firstLastName = "Duende",
            string secondLastName = "Gongora y Gongora",
            string code = "1",
            bool randomValues = false) where T : BaseEntity
        {
            //Act Dependencies
            var deparments = await new DepartmentManagerUT().CreateDefaultAsync<Department>(identityWorkId, instanceID);
            var jobPositions = await new JobPositionManagerUT().CreateDefaultAsync<JobPosition>(identityWorkId, instanceID);
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
            var emploRegID = await new EmployerRegistrationUT().CreateDefaultAsync<EmployerRegistration>(identityWorkId, instanceID);
            //Arrange
            var employees = new List<Employee>();

            if (randomValues)
            {
                Random _random = new Random();
                var num = _random.Next(0, 26); // Zero to 25
                char let = (char)('a' + num);
                rfc = rfc.Replace("A", let.ToString());
                num = _random.Next(0, 26); // Zero to 25
                let = (char)('a' + num);
                curp = curp.Replace("A", let.ToString());
                num = _random.Next(0, 9); // Zero to 25
                nss = nss.Replace("3", num.ToString());
                num = _random.Next(10, 100); // Zero to 25
                code = num.ToString();
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
                JobPositionID = jobPositions.FirstOrDefault().ID,
                DepartmentID = deparments.FirstOrDefault().ID,
                PeriodTypeID = period.FirstOrDefault().PeriodTypeID,
                WorkshiftID = workshifts.FirstOrDefault().ID,
                EmployerRegistrationID = emploRegID.FirstOrDefault().ID,
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

            //needed to create overdraft

            var conceptPayments = await (new ConceptManagerUT().CreateDefaultSalaryPaymentConceptsAsync(identityWorkId, instanceID));

            //Act
            var middlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
            await middlewareManager.CreateAsync(employees, identityWorkId);

            return employees as List<T>;
        }

        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceID,
           decimal diarySalary,
           decimal sbcFixed,
           string rfc = "RAQÑ7701212M3", string nss = "833531862",
           string curp = "KAHO641101HDFLKS06", string name = "Jaime",
           string firstLastName = "Duende",
           string secondLastName = "Gongora y Gongora",
           string code = "1",
           bool randomValues = false) where T : BaseEntity
        {
            //Act Dependencies
            var deparments = await new DepartmentManagerUT().CreateDefaultAsync<Department>(identityWorkId, instanceID);
            var jobPositions = await new JobPositionManagerUT().CreateDefaultAsync<JobPosition>(identityWorkId, instanceID);
            var periodManager = new MiddlewareManager<Period>(new BaseRecordManager<Period>(), new PeriodValidator());
            var period = await periodManager.GetAllAsync(identityWorkId, instanceID);
            if (!period.Any())
            {
                period = await (new PeriodManagerUT().CreateDefaultAsync<Period>(identityWorkId, instanceID, new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1), new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1).AddDays(-1), PaymentPeriodicity.Biweekly));
            }

            var workshifts = await new WorkshiftManagerUT().CreateDefaultAsync<Workshift>(identityWorkId, instanceID);
            var emploRegID = await new EmployerRegistrationUT().CreateDefaultAsync<EmployerRegistration>(identityWorkId, instanceID);
            //Arrange
            var employees = new List<Employee>();

            if (randomValues)
            {
                Random _random = new Random();
                var num = _random.Next(0, 26); // Zero to 25
                char let = (char)('a' + num);
                rfc = rfc.Replace("A", let.ToString());
                num = _random.Next(0, 26); // Zero to 25
                let = (char)('a' + num);
                curp = curp.Replace("A", let.ToString());
                num = _random.Next(0, 9); // Zero to 25
                nss = nss.Replace("3", num.ToString());
                num = _random.Next(10, 100); // Zero to 25
                code = num.ToString();
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
                JobPositionID = jobPositions.FirstOrDefault().ID,
                DepartmentID = deparments.FirstOrDefault().ID,
                PeriodTypeID = period.FirstOrDefault().PeriodTypeID,
                WorkshiftID = workshifts.FirstOrDefault().ID,
                EmployerRegistrationID = emploRegID.FirstOrDefault().ID,
                BankAccount = "123456789145214587",
                EmployeeTrustLevel = EmployeeTrustLevel.Trusted,
                StatusID = 1,
                Description = "desc",
                RegimeType = EmployeeRegimeType.Salaries,
                ContractType = ContractType.IndefiniteTermEmploymentContract,
                DailySalary = diarySalary,
                ContributionBase = BaseQuotation.Fixed,
                SalaryZone = SalaryZone.ZoneA,
                PaymentBase = PaymentBase.SalaryComission,
                SBCFixedPart = sbcFixed,
                SBCMax25UMA = 0m,
                SBCVariablePart = 0m
            });

            //needed to create overdraft

            var conceptPayments = await (new ConceptManagerUT().CreateDefaultSalaryPaymentConceptsAsync(identityWorkId, instanceID));

            //Act
            var middlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
            await middlewareManager.CreateAsync(employees, identityWorkId);

            return employees as List<T>;
        }

        public async Task<List<T>> CreateDefaultSpecialAsync<T>(Guid identityWorkId, Guid instanceID,
          string rfc = "RAQÑ7701212M3", string nss = "833531862",
          string curp = "KAHO641101HDFLKS06", string name = "Jaime",
          string firstLastName = "Duende",
          string secondLastName = "Gongora y Gongora",
          string code = "1",
          bool randomValues = false) where T : BaseEntity
        {
            //Act Dependencies
            var deparments = await new MiddlewareManager<Department>(new BaseRecordManager<Department>(), new DepartmentValidator()).FindByExpressionAsync(p
                => p.InstanceID == instanceID, identityWorkId);
            if (!deparments.Any())
            {
                deparments = await new DepartmentManagerUT().CreateDefaultAsync<Department>(identityWorkId, instanceID);
            }

            var jobPositions = await new MiddlewareManager<JobPosition>(new BaseRecordManager<JobPosition>(), new JobPositionValidator()).FindByExpressionAsync(p
                => p.InstanceID == instanceID, identityWorkId);
            if (!jobPositions.Any())
            {
                jobPositions = await new JobPositionManagerUT().CreateDefaultAsync<JobPosition>(identityWorkId, instanceID);
            }

            var period = await new MiddlewareManager<Period>(new BaseRecordManager<Period>(), new PeriodValidator()).FindByExpressionAsync(p
                => p.InstanceID == instanceID, identityWorkId);
            var workshifts = await new MiddlewareManager<Workshift>(new BaseRecordManager<Workshift>(), new WorkshiftValidator()).FindByExpressionAsync(p
                => p.InstanceID == instanceID, identityWorkId);
            var emploRegID = await new MiddlewareManager<EmployerRegistration>(new BaseRecordManager<EmployerRegistration>(), new EmployerRegistrationValidator()).FindByExpressionAsync(p
                => p.InstanceID == instanceID, identityWorkId);

            //Arrange
            var employees = new List<Employee>();

            if (randomValues)
            {
                Random _random = new Random();
                var num = _random.Next(0, 26); // Zero to 25
                char let = (char)('a' + num);
                rfc = rfc.Replace("A", let.ToString());
                num = _random.Next(0, 26); // Zero to 25
                let = (char)('a' + num);
                curp = curp.Replace("A", let.ToString());
                num = _random.Next(0, 9); // Zero to 25
                nss = nss.Replace("3", num.ToString());
                num = _random.Next(10, 100); // Zero to 25
                code = num.ToString();
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
                BankAccount = "123456789145214587",
                FirstLastName = firstLastName,
                SecondLastName = secondLastName,
                Gender = Gender.Male,
                EntryDate = new DateTime(DateTime.UtcNow.Year - 1, 1, 1),
                JobPositionID = jobPositions.FirstOrDefault().ID,
                DepartmentID = deparments.FirstOrDefault().ID,
                PeriodTypeID = period.FirstOrDefault().PeriodTypeID,
                WorkshiftID = workshifts.FirstOrDefault().ID,
                EmployerRegistrationID = emploRegID.FirstOrDefault().ID,
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
                SBCVariablePart = 1.54m
            });

            //Act
            var middlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
            await middlewareManager.CreateAsync(employees, identityWorkId);

            return employees as List<T>;
        }

        public async Task<Employee> UpdateEmployeeDailySalary(Employee employee, decimal amount, Guid identityWorkId)
        {
            employee.DailySalary = amount;
            var middlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
            await middlewareManager.UpdateAsync(new List<Employee> { employee }, identityWorkId);

            return employee;
        }

        public async Task<Employee> UpdateEmployeeDailySalaryAndSBC(Employee employee, decimal amount, decimal sbc, Guid identityWorkId)
        {
            employee.DailySalary = amount;
            employee.SBCMax25UMA = sbc;
            var middlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
            await middlewareManager.UpdateAsync(new List<Employee> { employee }, identityWorkId);

            return employee;
        }

        public class Create
        {
            [Fact]
            public async Task Should_Create_Employee_And_Get_ToValidate_Finally_do_Delete()
            {
                var txOptions = new System.Transactions.TransactionOptions();
                txOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;

                using var scope = new TransactionScope(TransactionScopeOption.Required, txOptions, TransactionScopeAsyncFlowOption.Enabled);

                //Act
                Guid identityWorkId = Guid.NewGuid();//Guid.Parse("7535C4E6-4712-4DD6-955D-FCA86E054D49");
                Guid instanceId = Guid.NewGuid(); //Guid.Parse("33D7CA50-39E9-4B14-B482-5FCBEC07E8DB");
                var employees = await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId, instanceId, randomValues: true);
                var middlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());

                //Asserts
                //Get
                var result = await middlewareManager
                    .GetByIdsAsync(employees.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());

                var middlewareManagerOverdraft = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var resultOverdraft = await middlewareManagerOverdraft.FindByExpressionAsync(p => p.InstanceID == instanceId && p.EmployeeID == employees.FirstOrDefault().ID, identityWorkId);
                Assert.True(resultOverdraft.Any());

                //Delete
                await middlewareManager.DeleteAsync(employees.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.FirstOrDefault().ID == employees.FirstOrDefault().ID);

                //Get it again to verify if the registry it was deleted
                var result2 = await middlewareManager
                    .GetByIdsAsync(employees.Select(p => p.ID).ToList(), identityWorkId);
                Assert.False(result2.Any());
            }

            [Fact]
            public async Task Should_NotCreate_Employee_Duplicate_Name()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                try
                {
                    //Act
                    Guid identityWorkId = Guid.NewGuid();
                    Guid instanceId = Guid.NewGuid();
                    var employees = await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId, instanceId, name: "Jimmy");

                    //Modify ToDuplicate
                    employees.FirstOrDefault().ID = Guid.NewGuid();
                    employees.FirstOrDefault().Code = "2";
                    employees.FirstOrDefault().CURP = "asdasdas";
                    employees.FirstOrDefault().RFC = "AAA010102AAB";

                    var middlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
                    await middlewareManager.CreateAsync(employees, identityWorkId);

                    Assert.False(true, "No se permite Nombre duplicado");
                }
                catch (Exception ex)
                {
                    Assert.True(ex is CotorraException);
                    Assert.True((ex as CotorraException).ErrorCode.Equals(4003));
                }
            }

            [Fact]
            public async Task Should_NotCreate_Employee_Duplicate_Code()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                try
                {
                    //Act
                    Guid identityWorkId = Guid.NewGuid();
                    Guid instanceId = Guid.NewGuid();
                    var employees = await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId, instanceId, code: "3", randomValues: true);

                    //Modify ToDuplicate
                    employees.FirstOrDefault().ID = Guid.NewGuid();
                    employees.FirstOrDefault().CURP = "asdasdas";
                    employees.FirstOrDefault().RFC = "AAA010102AAB";
                    employees.FirstOrDefault().Name = "Oscar";

                    var middlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
                    middlewareManager.Create(employees, identityWorkId);

                    Assert.False(true, "No se permite Código duplicado");
                }
                catch (Exception ex)
                {
                    Assert.True(ex is CotorraException);
                    Assert.True((ex as CotorraException).ErrorCode.Equals(4004));
                }
            }

            [Fact]
            public async Task Should_NotCreate_Employee_Duplicate_RFC()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                try
                {
                    //Act
                    Guid identityWorkId = Guid.NewGuid();
                    Guid instanceId = Guid.NewGuid();
                    var employees = await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId, instanceId, rfc: "FBR010101IAC", randomValues: false);

                    //Modify ToDuplicate
                    employees.FirstOrDefault().ID = Guid.NewGuid();
                    employees.FirstOrDefault().Code = "32562";
                    employees.FirstOrDefault().CURP = "asdasdas7";
                    employees.FirstOrDefault().Name = "Oscar26";

                    var middlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
                    middlewareManager.Create(employees, identityWorkId);

                    Assert.False(true, "No se permite RFC duplicado");
                }
                catch (Exception ex)
                {
                    Assert.True(ex is CotorraException);
                    Assert.True((ex as CotorraException).ErrorCode.Equals(4005));
                }
            }

            [Fact]
            public async Task Should_NotCreate_Employee_Duplicate_NSS()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                try
                {
                    //Act
                    Guid identityWorkId = Guid.NewGuid();
                    Guid instanceId = Guid.NewGuid();
                    var employees = await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId, instanceId, nss: "833531877", randomValues: false);

                    //Modify ToDuplicate
                    employees.FirstOrDefault().ID = Guid.NewGuid();
                    employees.FirstOrDefault().RFC = "CCC010201CAB";
                    employees.FirstOrDefault().Code = "2025";
                    employees.FirstOrDefault().CURP = "asdasdas2";
                    employees.FirstOrDefault().Name = "Oscar23";

                    var middlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
                    middlewareManager.Create(employees, identityWorkId);

                    Assert.False(true, "No se permite NSS duplicado");
                }
                catch (Exception ex)
                {
                    Assert.True(ex is CotorraException);
                    Assert.True((ex as CotorraException).ErrorCode.Equals(4006));
                }
            }

            [Fact]
            public async Task Should_NotCreate_Employee_Duplicate_CURP()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                try
                {
                    //Act
                    Guid identityWorkId = Guid.NewGuid();
                    Guid instanceId = Guid.NewGuid();
                    var employees = await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId, instanceId, curp: "BBSDASDBASDASR", randomValues: true);

                    //Modify ToDuplicate
                    employees.FirstOrDefault().ID = Guid.NewGuid();
                    employees.FirstOrDefault().RFC = "CCC010101CCC";
                    employees.FirstOrDefault().NSS = "084521754";
                    employees.FirstOrDefault().Code = "2";
                    employees.FirstOrDefault().Name = "Oscar";

                    var middlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
                    middlewareManager.Create(employees, identityWorkId);

                    Assert.False(true, "No se permite CURP duplicado");
                }
                catch (Exception ex)
                {
                    Assert.True(ex is CotorraException);
                    Assert.True((ex as CotorraException).ErrorCode.Equals(4007));
                }
            }

            [Fact]
            public async Task Should_NotCreate_Employee_ForegignKey_JobPosition()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                try
                {
                    //Act
                    Guid identityWorkId = Guid.NewGuid();
                    Guid instanceId = Guid.NewGuid();
                    var employees = await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId, instanceId, randomValues: true);
                    employees.FirstOrDefault().JobPositionID = Guid.NewGuid();
                    employees.FirstOrDefault().Code = "7";
                    employees.FirstOrDefault().Name = "Nombre";
                    employees.FirstOrDefault().NSS = "833541869";
                    employees.FirstOrDefault().CURP = "AASDASDBASDATY";
                    employees.FirstOrDefault().RFC = "CCC020202CCC";

                    var middlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
                    middlewareManager.Create(employees, identityWorkId);

                    Assert.False(true, "No se permite posición no existe");
                }
                catch (Exception ex)
                {
                    Assert.True(ex is CotorraException);
                    Assert.True((ex as CotorraException).ErrorCode.Equals(4009));
                }
            }
        }

        public class Update
        {
            [Fact]
            public async Task Should_Update_Employee_And_Get_ToValidate_Finally_do_Delete()
            {
                var txOptions = new System.Transactions.TransactionOptions();
                txOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;

                using var scope = new TransactionScope(TransactionScopeOption.Required, txOptions, TransactionScopeAsyncFlowOption.Enabled);

                //Act
                Guid identityWorkId = Guid.NewGuid();//Guid.Parse("7535C4E6-4712-4DD6-955D-FCA86E054D49");
                Guid instanceId = Guid.NewGuid(); //Guid.Parse("33D7CA50-39E9-4B14-B482-5FCBEC07E8DB");
                var employees = await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId, instanceId, randomValues: true);
                var middlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());

                //Asserts
                //Get
                var result = await middlewareManager
                    .GetByIdsAsync(employees.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());

                var middlewareManagerOverdraft = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var resultOverdraft = await middlewareManagerOverdraft.FindByExpressionAsync(p => p.InstanceID == instanceId && p.EmployeeID == employees.FirstOrDefault().ID, identityWorkId);
                Assert.True(resultOverdraft.Any());

                var employeeId = employees.FirstOrDefault().ID;
                var newAmount = employees.FirstOrDefault().DailySalary;
                employees.FirstOrDefault().DailySalary = 5478;
                await middlewareManager.UpdateAsync(employees, identityWorkId);

                var middlewareHistoricEmployeeSalaryAdjustmentManager = new MiddlewareManager<HistoricEmployeeSalaryAdjustment>(new
                    BaseRecordManager<HistoricEmployeeSalaryAdjustment>(), new HistoricEmployeeSalaryAdjustmentValidator());
                var historicEmployee = await middlewareHistoricEmployeeSalaryAdjustmentManager.FindByExpressionAsync(p => p.EmployeeID == employeeId, identityWorkId); ;
                Assert.True(historicEmployee.FirstOrDefault().DailySalary == newAmount);

            }

        }

        public class ChangeStatus
        {
            [Fact]
            public async Task Should_Change_Status_To_Inactive()
            {
                var txOptions = new System.Transactions.TransactionOptions();
                txOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;

                using var scope = new TransactionScope(TransactionScopeOption.Required, txOptions, TransactionScopeAsyncFlowOption.Enabled);

                //Act
                Guid identityWorkId = Guid.NewGuid();//Guid.Parse("7535C4E6-4712-4DD6-955D-FCA86E054D49");
                Guid instanceId = Guid.NewGuid(); //Guid.Parse("33D7CA50-39E9-4B14-B482-5FCBEC07E8DB");
                var employees = await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId, instanceId, randomValues: true);
                var middlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());

                //Asserts

                //Get
                var result = await middlewareManager
                    .GetByIdsAsync(employees.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());
                var employeeFromDB = result.FirstOrDefault();
                Assert.Equal(CotorriaStatus.Active, employeeFromDB.LocalStatus);
                Assert.Equal(DateTime.Now.Date, employeeFromDB.LastStatusChange.Date);


                //Set inactive
                var employeeId = employeeFromDB.ID;
                var statusManager = new StatusManager<Employee>(new EmployeeValidator());
                await statusManager.SetInactive(new List<Guid>() { employeeId }, identityWorkId);

                //Asserts

                //GetAggain
                result = await middlewareManager
                  .GetByIdsAsync(employees.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());
                employeeFromDB = result.FirstOrDefault();

                //Asserts

                Assert.Equal(CotorriaStatus.Inactive, employeeFromDB.LocalStatus);
                Assert.Equal(DateTime.Now.Date, employeeFromDB.LastStatusChange.Date);
            }

            [Fact]
            public async Task Should_Change_Status_To_Unregistered()
            {
                var txOptions = new System.Transactions.TransactionOptions();
                txOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;

                using var scope = new TransactionScope(TransactionScopeOption.Required, txOptions, TransactionScopeAsyncFlowOption.Enabled);

                //Act
                Guid identityWorkId = Guid.NewGuid();//Guid.Parse("7535C4E6-4712-4DD6-955D-FCA86E054D49");
                Guid instanceId = Guid.NewGuid(); //Guid.Parse("33D7CA50-39E9-4B14-B482-5FCBEC07E8DB");
                var employees = await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId, instanceId, randomValues: true);
                var middlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());

                //Asserts

                //Get
                var result = await middlewareManager
                    .GetByIdsAsync(employees.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());
                var employeeFromDB = result.FirstOrDefault();
                Assert.Equal(CotorriaStatus.Active, employeeFromDB.LocalStatus);
                Assert.Equal(DateTime.Now.Date, employeeFromDB.LastStatusChange.Date);


                //Set inactive
                DateTime? unregisteredDate = DateTime.Now.AddDays(1);
                var employeeId = employeeFromDB.ID;
                var statusManager = new StatusManager<Employee>(new EmployeeValidator());
                await statusManager.SetUnregistered(new List<Guid>() { employeeId }, identityWorkId, unregisteredDate);

                //Asserts

                //GetAggain
                result = await middlewareManager
                  .GetByIdsAsync(employees.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());
                employeeFromDB = result.FirstOrDefault();

                //Asserts

                Assert.Equal(CotorriaStatus.Unregistered, employeeFromDB.LocalStatus);
                Assert.Equal(DateTime.Now.Date, employeeFromDB.LastStatusChange.Date);
                Assert.Equal(unregisteredDate.GetValueOrDefault().Date, employeeFromDB.UnregisteredDate.GetValueOrDefault().Date);
            }

            [Fact]
            public async Task Should_Change_Status_To_UnregisteredUsingClient()
            {
              
                var txOptions = new System.Transactions.TransactionOptions();
                txOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;

                using var scope = new TransactionScope(TransactionScopeOption.Required, txOptions, TransactionScopeAsyncFlowOption.Enabled);

                //Act
                Guid identityWorkId = Guid.NewGuid();//Guid.Parse("7535C4E6-4712-4DD6-955D-FCA86E054D49");
                Guid instanceId = Guid.NewGuid(); //Guid.Parse("33D7CA50-39E9-4B14-B482-5FCBEC07E8DB");
                var employees = await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId, instanceId, randomValues: true);
                var middlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());

                //Asserts

                //Get
                var result = await middlewareManager
                    .GetByIdsAsync(employees.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());
                var employeeFromDB = result.FirstOrDefault();
                Assert.Equal(CotorriaStatus.Active, employeeFromDB.LocalStatus);
                Assert.Equal(DateTime.Now.Date, employeeFromDB.LastStatusChange.Date);


                //Set inactive
                DateTime? unregisteredDate = DateTime.Now.AddDays(1);
                var employeeId = employeeFromDB.ID;

                var statusClient = new StatusClient<Employee>("", ClientConfiguration.ClientAdapter.Local);
                await statusClient.SetUnregistered(new List<Guid>() { employeeId }, identityWorkId, unregisteredDate);

                //Asserts

                //GetAggain
                result = await middlewareManager
                  .GetByIdsAsync(employees.Select(p => p.ID).ToList(), identityWorkId);
                Assert.True(result.Any());
                employeeFromDB = result.FirstOrDefault();

                //Asserts

                Assert.Equal(CotorriaStatus.Unregistered, employeeFromDB.LocalStatus);
                Assert.Equal(DateTime.Now.Date, employeeFromDB.LastStatusChange.Date);
                Assert.Equal(unregisteredDate.GetValueOrDefault().Date, employeeFromDB.UnregisteredDate.GetValueOrDefault().Date);
            }
        }
    }
}
