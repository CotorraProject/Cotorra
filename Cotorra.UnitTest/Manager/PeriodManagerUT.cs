using CotorraNode.Common.Base.Schema;
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
    public class PeriodManagerUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceId, DateTime initialDate, DateTime finalDate, PaymentPeriodicity paymentPeriodicity) where T : BaseEntity
        {
            var periodType = (await new PeriodTypeUT().CreateDefaultAsync<PeriodType>
                (identityWorkId, instanceId, paymentPeriodicity)).FirstOrDefault();

            var Periods = new List<Period>();
            Periods.Add(new Period()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceId,
                Description = $"Periodo del Ejercicio {DateTime.Now.Year}",
                CreationDate = DateTime.Now,
                Name = $"{DateTime.Now.Year}",
                InitialDate = initialDate,
                FinalDate = initialDate.AddYears(1),
                FiscalYear = DateTime.Now.Year,
                IsActualFiscalYear = true,
                IsFiscalYearClosed = false,
                PeriodTypeID = periodType.ID,
                ExtraordinaryPeriod = periodType.ExtraordinaryPeriod,
                FortnightPaymentDays = periodType.FortnightPaymentDays,
                MonthCalendarFixed = periodType.MonthCalendarFixed,
                PaymentDayPosition = periodType.PaymentDayPosition,
                PaymentDays = periodType.PaymentDays,
                PaymentPeriodicity = periodType.PaymentPeriodicity,
                PeriodTotalDays = periodType.PeriodTotalDays,
                StatusID = 1
            });

            var middlewareManager = new MiddlewareManager<Period>(new BaseRecordManager<Period>(), new PeriodValidator());
            await middlewareManager.CreateAsync(Periods, identityWorkId);

            return Periods as List<T>;
        }

        [Fact]
        public async Task Should_Create_Biweekly()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            //Arrange
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();

            //Act
            var initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            var finalDate = new DateTime(DateTime.Now.Year, 12, 31);
            var paymentPeriodicity = PaymentPeriodicity.Biweekly;
            var period = await CreateDefaultAsync<Period>(identityWorkId, instanceId, initialDate, finalDate, paymentPeriodicity);

            //Assert
            var middlewareManager = new MiddlewareManager<Period>(new BaseRecordManager<Period>(), new PeriodValidator());
            var found = await middlewareManager.GetByIdsAsync(new List<Guid>() { period.FirstOrDefault().ID }, identityWorkId, new string[] { "PeriodDetails" });
            Assert.True(found.Any());

            var periodDetails = found.SelectMany(p => p.PeriodDetails).OrderBy(p => p.Number);
            Assert.True(periodDetails.Any());
            Assert.True(found.SelectMany(p => p.PeriodDetails).Count() == 24);

            //Delete test
            var middlewareManagerPeriodDetails = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), new PeriodDetailValidator());
            var periodDetailsIds = found.SelectMany(p => p.PeriodDetails).Select(p => p.ID).ToList();
            //details
            await middlewareManagerPeriodDetails.DeleteAsync(periodDetailsIds, identityWorkId);
            //periods
            await middlewareManager.DeleteAsync(new List<Guid>() { found.FirstOrDefault().ID }, identityWorkId);
        }

        [Fact]
        public async Task Should_Create_Biweekly_DeletePeriodDetail_Fail_FuturePeriodDetails()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();

                //Act
                var initialDate = new DateTime(DateTime.Now.Year, 1, 1);
                var finalDate = new DateTime(DateTime.Now.Year, 12, 31);
                var paymentPeriodicity = PaymentPeriodicity.Biweekly;
                var period = await CreateDefaultAsync<Period>(identityWorkId, instanceId, initialDate, finalDate, paymentPeriodicity);

                //Assert
                var middlewareManager = new MiddlewareManager<Period>(new BaseRecordManager<Period>(), new PeriodValidator());
                var found = await middlewareManager.GetByIdsAsync(new List<Guid>() { period.FirstOrDefault().ID }, identityWorkId, new string[] { "PeriodDetails" });
                Assert.True(found.Any());

                var periodDetails = found.SelectMany(p => p.PeriodDetails).OrderBy(p => p.Number);
                Assert.True(periodDetails.Any());
                Assert.True(found.SelectMany(p => p.PeriodDetails).Count() == 24);

                var middlewareManagerPeriodDetails = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), new PeriodDetailValidator());
                var periodDetailToDelete = periodDetails.FirstOrDefault(p => p.Number == 1);
                await middlewareManagerPeriodDetails.DeleteAsync(new List<Guid> { periodDetailToDelete.ID }, identityWorkId);

                Assert.True(false);
            }
            catch (Exception ex)
            {
                Assert.True(ex is CotorraException);
                Assert.True((ex as CotorraException).ErrorCode.Equals(8007));
            }
        }

        [Fact]
        public async Task Should_Create_Biweekly_DeletePeriodDetail_Pass_FuturePeriodDetails_All()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();

                //Act
                var initialDate = new DateTime(DateTime.Now.Year, 1, 1);
                var finalDate = new DateTime(DateTime.Now.Year, 12, 31);
                var paymentPeriodicity = PaymentPeriodicity.Biweekly;
                var period = await CreateDefaultAsync<Period>(identityWorkId, instanceId, initialDate, finalDate, paymentPeriodicity);

                //Assert
                var middlewareManager = new MiddlewareManager<Period>(new BaseRecordManager<Period>(), new PeriodValidator());
                var found = await middlewareManager.GetByIdsAsync(new List<Guid>() { period.FirstOrDefault().ID }, identityWorkId, new string[] { "PeriodDetails" });
                Assert.True(found.Any());

                var periodDetails = found.SelectMany(p => p.PeriodDetails).OrderBy(p => p.Number);
                Assert.True(periodDetails.Any());
                Assert.True(found.SelectMany(p => p.PeriodDetails).Count() == 24);

                var middlewareManagerPeriodDetails = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), new PeriodDetailValidator());
                // //Delete test
                var periodDetailsIds = found.SelectMany(p => p.PeriodDetails).Select(p => p.ID).ToList();
                //details
                await middlewareManagerPeriodDetails.DeleteAsync(periodDetailsIds, identityWorkId);
                //periods
                await middlewareManager.DeleteAsync(new List<Guid>() { found.FirstOrDefault().ID }, identityWorkId);
            }
            catch (Exception ex)
            {
                Assert.True(false, ex.Message);
            }           
        }

        [Fact]
        public async Task Should_Create_Biweekly_UpdatePeriodDetail_Fail_FuturePeriodDetails_All()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();

                //Act
                var initialDate = new DateTime(DateTime.Now.Year, 1, 1);
                var finalDate = new DateTime(DateTime.Now.Year, 12, 31);
                var paymentPeriodicity = PaymentPeriodicity.Biweekly;
                var period = await CreateDefaultAsync<Period>(identityWorkId, instanceId, initialDate, finalDate, paymentPeriodicity);

                //Assert
                var middlewareManager = new MiddlewareManager<Period>(new BaseRecordManager<Period>(), new PeriodValidator());
                var found = await middlewareManager.GetByIdsAsync(new List<Guid>() { period.FirstOrDefault().ID }, identityWorkId, new string[] { "PeriodDetails" });
                Assert.True(found.Any());

                var periodDetails = found.SelectMany(p => p.PeriodDetails).OrderBy(p => p.Number);
                Assert.True(periodDetails.Any());
                Assert.True(found.SelectMany(p => p.PeriodDetails).Count() == 24);

                var middlewareManagerPeriodDetails = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), new PeriodDetailValidator());

                //Update
                var finalDateToUpdate = periodDetails.FirstOrDefault(p => p.Number == 1).FinalDate;
                periodDetails.FirstOrDefault(p => p.Number == 2).InitialDate = finalDateToUpdate;
                await middlewareManagerPeriodDetails.UpdateAsync(periodDetails.ToList(), identityWorkId);

                Assert.True(false, "Se espera que no se pueda actualizar");
            }
            catch (Exception ex)
            {
                Assert.True(ex is CotorraException);
                Assert.True((ex as CotorraException).ErrorCode.Equals(8008));
            }
        }

        [Fact]
        public async Task Should_Create_Biweekly_UpdatePeriodDetail_Pass_FuturePeriodDetails_All()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceId = Guid.NewGuid();

                //Act
                var initialDate = new DateTime(DateTime.Now.Year, 1, 1);
                var finalDate = new DateTime(DateTime.Now.Year, 12, 31);
                var paymentPeriodicity = PaymentPeriodicity.Biweekly;
                var period = await CreateDefaultAsync<Period>(identityWorkId, instanceId, initialDate, finalDate, paymentPeriodicity);

                //Assert
                var middlewareManager = new MiddlewareManager<Period>(new BaseRecordManager<Period>(), new PeriodValidator());
                var found = await middlewareManager.GetByIdsAsync(new List<Guid>() { period.FirstOrDefault().ID }, identityWorkId, new string[] { "PeriodDetails" });
                Assert.True(found.Any());

                var periodDetails = found.SelectMany(p => p.PeriodDetails).OrderBy(p => p.Number);
                Assert.True(periodDetails.Any());
                Assert.True(found.SelectMany(p => p.PeriodDetails).Count() == 24);

                var middlewareManagerPeriodDetails = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), new PeriodDetailValidator());

                //Update
                var finalDateToUpdate = periodDetails.FirstOrDefault(p => p.Number == 1).FinalDate;
                periodDetails.FirstOrDefault(p => p.Number == 2).InitialDate = finalDateToUpdate.AddDays(3);
                await middlewareManagerPeriodDetails.UpdateAsync(periodDetails.ToList(), identityWorkId);
            }
            catch (Exception ex)
            {
                Assert.True(false, $"Se espera que se pueda actualizar {ex.Message}");
            }
        }


        [Fact]
        public async Task Should_Create_Monthly()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            //Arrange
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();

            //Act
            var initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            var finalDate = new DateTime(DateTime.Now.Year, 12, 31);
            var paymentPeriodicity = PaymentPeriodicity.Monthly;
            var period = await CreateDefaultAsync<Period>(identityWorkId, instanceId, initialDate, finalDate, paymentPeriodicity);

            //Assert
            var middlewareManager = new MiddlewareManager<Period>(new BaseRecordManager<Period>(), new PeriodValidator());
            var found = await middlewareManager.GetByIdsAsync(new List<Guid>() { period.FirstOrDefault().ID }, identityWorkId, new string[] { "PeriodDetails" });
            Assert.True(found.Any());

            var periodDetails = found.SelectMany(p => p.PeriodDetails).OrderBy(p => p.Number);
            Assert.True(periodDetails.Any());
            Assert.True(found.SelectMany(p => p.PeriodDetails).Count() == 12);

            //Delete test
            var middlewareManagerPeriodDetails = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), new PeriodDetailValidator());
            var periodDetailsIds = found.SelectMany(p => p.PeriodDetails).Select(p => p.ID).ToList();
            //details
            await middlewareManagerPeriodDetails.DeleteAsync(periodDetailsIds, identityWorkId);
            //periods
            await middlewareManager.DeleteAsync(new List<Guid>() { found.FirstOrDefault().ID }, identityWorkId);
        }

        [Fact]
        public async Task Should_Create_Weekly()
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            //Arrange
            var identityWorkId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();

            //Act
            var initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            var finalDate = new DateTime(DateTime.Now.Year, 12, 31);
            var paymentPeriodicity = PaymentPeriodicity.Weekly;
            var period = await CreateDefaultAsync<Period>(identityWorkId, instanceId, initialDate, finalDate, paymentPeriodicity);

            //Assert
            var middlewareManager = new MiddlewareManager<Period>(new BaseRecordManager<Period>(), new PeriodValidator());
            var found = await middlewareManager.GetByIdsAsync(new List<Guid>() { period.FirstOrDefault().ID }, identityWorkId, new string[] { "PeriodDetails" });
            Assert.True(found.Any());

            var periodDetails = found.SelectMany(p => p.PeriodDetails).OrderBy(p => p.Number);
            Assert.True(periodDetails.Any());
            Assert.True(found.SelectMany(p => p.PeriodDetails).Count() == 53 || found.SelectMany(p => p.PeriodDetails).Count() == 52);

            //Delete test
            var middlewareManagerPeriodDetails = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), new PeriodDetailValidator());
            var periodDetailsIds = found.SelectMany(p => p.PeriodDetails).Select(p => p.ID).ToList();
            //details
            await middlewareManagerPeriodDetails.DeleteAsync(periodDetailsIds, identityWorkId);
            //periods
            await middlewareManager.DeleteAsync(new List<Guid>() { found.FirstOrDefault().ID }, identityWorkId);
        }
    }
}
