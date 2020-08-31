using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Cotorra.UnitTest
{
    public class VacationManagerUT
    {
        public Vacation Build(Guid identityWorkID, Guid instanceID, Guid employeeID,
            DateTime initialDate, DateTime finalDate)
        {
            var vacation = new Vacation()
            {
                Active = true,
                company = identityWorkID,
                CreationDate = DateTime.UtcNow,
                DeleteDate = null,
                Description = "DEscripcion",
                EmployeeID = employeeID,
                InitialDate = initialDate,
                FinalDate = finalDate,
                ID = Guid.NewGuid(),
                InstanceID = instanceID,
                Name = "NameVacations",
                PaymentDate = DateTime.UtcNow,
                StatusID = 1,
                user = Guid.NewGuid(),
                VacationsBonusDays = 0,
                VacationsCaptureType = VacationsCaptureType.PayVacationsAndBonusInPeriod,
                VacationsDays = Convert.ToDecimal((finalDate - initialDate).TotalDays) + 1
            };
            return vacation;

        }

        public VacationDaysOff BuildVacationDaysOff(Guid identityWorkID, Guid instanceID, 
          DateTime Date)
        {
            var vacationDaysOff = new VacationDaysOff()
            {
                Active = true,
                company = identityWorkID, 
                DeleteDate = null, 
                ID = Guid.NewGuid(), 
                user = Guid.NewGuid(),
                Date = Date, 
                InstanceID = instanceID,
            };
            return vacationDaysOff;

        }


        public async Task<Vacation> CreateDefaultAsync(Guid identityWorkID, Guid instanceID, Guid employeeID,
            DateTime initialDate, DateTime finalDate, List<VacationDaysOff> daysOffs = null)
        { 
            var vacation = new Vacation()
            {
                Active = true,
                company = identityWorkID,
                CreationDate = DateTime.UtcNow,
                DeleteDate = null,
                Description = "DEscripcion",
                EmployeeID = employeeID,
                InitialDate = initialDate,
                FinalDate = finalDate,
                ID = Guid.NewGuid(),
                InstanceID = instanceID,
                Name = "NameVacations",
                PaymentDate = DateTime.UtcNow,
                StatusID = 1,
                user = Guid.NewGuid(),
                VacationsBonusDays = 2,
                VacationsCaptureType = VacationsCaptureType.PayVacationsAndBonusInPeriod,
                VacationsDays = Convert.ToDecimal((finalDate - initialDate).TotalDays) + 1
            };

            if (daysOffs != null)
            {
                vacation.VacationDaysOff = daysOffs;
            }

            var vacationsManager = new MiddlewareManager<Vacation>(new BaseRecordManager<Vacation>(), new VacationValidator());
            await vacationsManager.CreateAsync(new List<Vacation> { vacation }, identityWorkID);

            return vacation;
        }

        public class Create
        {
            [Fact]
            public async Task Should_Create_VacationNotDaysOff_Finally_do_Delete()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();

                var employee = (await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId, instanceId)).FirstOrDefault();

                var vacation = await new VacationManagerUT().CreateDefaultAsync(identityWorkId, instanceId, employee.ID, DateTime.Now, DateTime.Now.AddDays(1));

                //Act
                var middlewareManager = new MiddlewareManager<Vacation>(new BaseRecordManager<Vacation>(), new VacationValidator());

                //Asserts
                //Get
                var result = await middlewareManager
                    .GetByIdsAsync( new List<Guid>() { vacation.ID }, identityWorkId);
                Assert.True(result.Any());
                Assert.True(result.FirstOrDefault().ID == vacation.ID);

                //Delete
                await middlewareManager.DeleteAsync(new List<Guid>() { vacation.ID }, identityWorkId);

                //Get it again to verify if the registry it was deleted
                var result2 = await middlewareManager
                    .GetByIdsAsync(new List<Guid>() { vacation.ID }, identityWorkId);
                Assert.True(!result2.Any());
            }

            [Fact]
            public async Task Should_Create_VacationWithDaysOff_Finally_do_Delete()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();
                var initialDate = DateTime.Now;
                var finalDate = initialDate.AddDays(10);
                var dayOff = finalDate.AddDays(-2);
                var employee = (await new EmployeeManagerUT().CreateDefaultAsync<Employee>(identityWorkId, instanceId)).FirstOrDefault();
                var utManager = new VacationManagerUT();

                var daysoff = utManager.BuildVacationDaysOff(identityWorkId, instanceId, dayOff);

                var vacation = await utManager.CreateDefaultAsync(identityWorkId, instanceId, employee.ID, DateTime.Now, DateTime.Now.AddDays(1),
                   new List<VacationDaysOff>() { daysoff });

                //Act
                var middlewareManager = new MiddlewareManager<Vacation>(new BaseRecordManager<Vacation>(), new VacationValidator());
                var middlewareVacationDaysOffManager = new MiddlewareManager<VacationDaysOff>(new BaseRecordManager<VacationDaysOff>(), new VacationDaysOffValidator());

                //Asserts
                //Get
                var result = await middlewareManager
                    .GetByIdsAsync(new List<Guid>() { vacation.ID }, identityWorkId);
                Assert.True(result.Any());
                Assert.True(result.FirstOrDefault().ID == vacation.ID);

                //Delete
                await middlewareVacationDaysOffManager.DeleteAsync(new List<Guid>() { daysoff.ID }, identityWorkId);
                await middlewareManager.DeleteAsync(new List<Guid>() { vacation.ID }, identityWorkId);

                //Get it again to verify if the registry it was deleted
                var result2 = await middlewareManager
                    .GetByIdsAsync(new List<Guid>() { vacation.ID }, identityWorkId);
                Assert.True(!result2.Any());
            }
        }
    }
}
