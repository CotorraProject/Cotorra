using Cotorra.Core;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Cotorra.Core.Extensions;
using System.Linq;
using System.Transactions;
using Cotorra.Core.Validator;
using CotorraNode.Common.Base.Schema;
using System.Threading.Tasks;

namespace Cotorra.UnitTest
{
    public class PeriodTypeUT
    {
        public async Task<List<T>> CreateDefaultAsync<T>(Guid identityWorkId, Guid instanceID, PaymentPeriodicity paymentPeriodicity) where T : BaseEntity
        {
            var periodTypes = new List<PeriodType>();
            var namePeriodicity = Enum.GetName(typeof(PaymentPeriodicity), paymentPeriodicity);
            var periodTotalDays = 0;
            var paymentDays = 0;
            var paymentDayPosition = 0;
            bool monthCalendarFixed = true;

            if (paymentPeriodicity == PaymentPeriodicity.Biweekly)
            {
                periodTotalDays = 15;
                paymentDays = 15;
                paymentDayPosition = 15;
                monthCalendarFixed = true;
            }
            else if (paymentPeriodicity == PaymentPeriodicity.Monthly)
            {
                periodTotalDays = 30;
                paymentDays = 30;
                paymentDayPosition = 30;
                monthCalendarFixed = true;
            }
            else if (paymentPeriodicity == PaymentPeriodicity.Weekly)
            {
                periodTotalDays = 7;
                paymentDays = 7;
                paymentDayPosition = 7;
                monthCalendarFixed = false;
            }

            periodTypes.Add(new PeriodType()
            {
                ID = Guid.NewGuid(),
                Active = true,
                company = identityWorkId,
                Timestamp = DateTime.UtcNow,
                InstanceID = instanceID,
                Description = $"Periodo de pago {namePeriodicity}",
                CreationDate = DateTime.Now,
                Name = namePeriodicity,
                StatusID = 1,
                PaymentPeriodicity = paymentPeriodicity,
                PeriodTotalDays = periodTotalDays,
                PaymentDays = paymentDays,
                ExtraordinaryPeriod = false,
                MonthCalendarFixed = monthCalendarFixed,
                FortnightPaymentDays = AdjustmentPay_16Days_Febrary.PayCalendarDays,
                PaymentDayPosition = paymentDayPosition
            });

            //Act
            var middlewareManager = new MiddlewareManager<PeriodType>(new BaseRecordManager<PeriodType>(), new PeriodTypeValidator());
            await middlewareManager.CreateAsync(periodTypes, identityWorkId);

            return periodTypes as List<T>;
        }

        public class Create
        {
            #region valid

            [Fact]
            public async Task Should_Create()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var identityWorkId = Guid.NewGuid();
                var instanceID = Guid.NewGuid();

                var periodTypes = await new PeriodTypeUT().CreateDefaultAsync<PeriodType>(identityWorkId, instanceID, PaymentPeriodicity.Biweekly);

                //Act
                var middlewareManager = new MiddlewareManager<PeriodType>(new BaseRecordManager<PeriodType>(),
                    new PeriodTypeValidator());

                //Assert
                var id = periodTypes.FirstOrDefault().ID;
                var found = await middlewareManager.GetByIdsAsync(new List<Guid>() { id }, identityWorkId);
                Assert.True(found.Any());
                var register = found.FirstOrDefault(x => x.ID == id);
                Assert.NotNull(register);

                await middlewareManager.DeleteAsync(new List<Guid>() { id }, identityWorkId);

                found = await middlewareManager.GetByIdsAsync(new List<Guid>() { id }, identityWorkId);
                Assert.True(!found.Any());
            }

            #endregion



            [Fact]
            public async Task Should_Fail_When_Payment_Periodicity_Is_BadValue()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var periodTypes = new List<PeriodType>();
                var identityWorkId = Guid.NewGuid();
                periodTypes.Add(PeriodTypeBuilder.Build(Guid.NewGuid(), identityWorkId, "Name"));

                //Act
                var middlewareManager = new MiddlewareManager<PeriodType>(new BaseRecordManager<PeriodType>(), new PeriodTypeValidator());
                try
                {
                    await middlewareManager.CreateAsync(periodTypes, identityWorkId);
                }
                catch (CotorraException ex)
                {
                    //Assert
                    var res = ex.ValidationInfo;
                    Assert.NotEmpty(res);
                    Assert.Equal(9003, ex.ErrorCode);
                }

            }


            [Fact]
            public async Task Should_Fail_When_Name_Is_TooLong()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var periodTypes = new List<PeriodType>();
                var identityWorkId = Guid.NewGuid();
                periodTypes.Add(PeriodTypeBuilder.Build(Guid.NewGuid(), identityWorkId, new string('a', 51)));


                //Act
                var middlewareManager = new MiddlewareManager<PeriodType>(new BaseRecordManager<PeriodType>(), new PeriodTypeValidator());
                try
                {
                    await middlewareManager.CreateAsync(periodTypes, identityWorkId);
                }
                catch (CotorraException ex)
                {
                    //Assert
                    var res = ex.ValidationInfo;
                    Assert.NotEmpty(res);
                    Assert.Equal(9001, ex.ErrorCode);
                }

            }

            [Fact]
            public async Task Should_Fail_When_Period_Total_Days_Is_TooBig()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var periodTypes = new List<PeriodType>();
                var identityWorkId = Guid.NewGuid();
                var periodType = PeriodTypeBuilder.Build(Guid.NewGuid(), identityWorkId, "name");

                periodType.PeriodTotalDays = 366;

                periodTypes.Add(periodType);

                //Act
                var middlewareManager = new MiddlewareManager<PeriodType>(new BaseRecordManager<PeriodType>(), new PeriodTypeValidator());
                try
                {
                    await middlewareManager.CreateAsync(periodTypes, identityWorkId);
                }
                catch (CotorraException ex)
                {
                    //Assert
                    var res = ex.ValidationInfo;
                    Assert.NotEmpty(res);
                    Assert.Equal(9002, ex.ErrorCode);
                }

            }

        }

        public class Update
        {
            [Fact]
            public async Task Should_Fail_When_Payment_Periodicity_Is_BadValue()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var periodTypes = new List<PeriodType>();
                var identityWorkId = Guid.NewGuid();
                var id = Guid.NewGuid();
                var originalName = "some";

                var periodType = PeriodTypeBuilder.Build(id, identityWorkId, originalName);

                periodTypes.Add(periodType);


                var middlewareManager = new MiddlewareManager<PeriodType>(new BaseRecordManager<PeriodType>(), new PeriodTypeValidator());

                await middlewareManager.CreateAsync(periodTypes, identityWorkId);

                var found = await middlewareManager.GetByIdsAsync(new List<Guid>() { id }, identityWorkId);
                Assert.True(found.Any());
                var register = found.FirstOrDefault(x => x.ID == id);
                Assert.NotNull(register);
                Assert.Equal(register.Name, originalName);

                register.PaymentPeriodicity = (PaymentPeriodicity)11;


                try
                {
                    await middlewareManager.UpdateAsync(new List<PeriodType>() { register }, identityWorkId);
                }
                catch (CotorraException ex)
                {
                    //Assert
                    var res = ex.ValidationInfo;
                    Assert.NotEmpty(res);
                    Assert.Equal(9003, ex.ErrorCode);
                }
                finally
                {
                    await middlewareManager.DeleteAsync(new List<Guid>() { id }, identityWorkId);
                }

            }


            [Fact]
            public async Task Should_Fail_When_Name_Is_TooLong()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                var periodTypes = new List<PeriodType>();
                var identityWorkId = Guid.NewGuid();
                var id = Guid.NewGuid();
                var originalName = "some";
                var periodType = PeriodTypeBuilder.Build(id, identityWorkId, originalName);

                periodTypes.Add(periodType);


                var middlewareManager = new MiddlewareManager<PeriodType>(new BaseRecordManager<PeriodType>(), new PeriodTypeValidator());

                await middlewareManager.CreateAsync(periodTypes, identityWorkId);

                var found = await middlewareManager.GetByIdsAsync(new List<Guid>() { id }, identityWorkId);
                Assert.True(found.Any());
                var register = found.FirstOrDefault(x => x.ID == id);
                Assert.NotNull(register);
                Assert.Equal(register.Name, originalName);

                register.Name = new string('a', 251);

                try
                {
                    await middlewareManager.UpdateAsync(new List<PeriodType>() { register }, identityWorkId);

                    Assert.True(false, "Debe de mandar error");
                }
                catch (CotorraException ex)
                {
                    //Assert
                    var res = ex.ValidationInfo;
                    Assert.NotEmpty(res);
                    Assert.Equal(9001, ex.ErrorCode);
                }
            }

            [Fact]
            public async Task Should_Fail_When_Period_Total_Days_Is_TooBig()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                var periodTypes = new List<PeriodType>();
                var identityWorkId = Guid.NewGuid();
                var id = Guid.NewGuid();
                var originalName = "some";

                var periodType = PeriodTypeBuilder.Build(id, identityWorkId, originalName);

                periodTypes.Add(periodType);
                var middlewareManager = new MiddlewareManager<PeriodType>(new BaseRecordManager<PeriodType>(), new PeriodTypeValidator());

                await middlewareManager.CreateAsync(periodTypes, identityWorkId);

                var found = await middlewareManager.GetByIdsAsync(new List<Guid>() { id }, identityWorkId);
                Assert.True(found.Any());
                var register = found.FirstOrDefault(x => x.ID == id);
                Assert.NotNull(register);
                Assert.Equal(register.Name, originalName);

                register.PeriodTotalDays = 390;

                try
                {
                    await middlewareManager.UpdateAsync(new List<PeriodType>() { register }, identityWorkId);

                    Assert.True(false, "Debe de mandar error");
                }
                catch (CotorraException ex)
                {
                    //Assert
                    var res = ex.ValidationInfo;
                    Assert.NotEmpty(res);
                    Assert.Equal(9002, ex.ErrorCode);
                }               
            }
        }

        public class Delete
        {
            [Fact]
            public async Task Should_Delete()
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                //Arrange
                var periodTypes = new List<PeriodType>();
                var identityWorkId = Guid.NewGuid();
                var id = Guid.NewGuid();

                var periodType = PeriodTypeBuilder.Build(id, identityWorkId, "Quincenal");

                periodTypes.Add(periodType);

                //Act
                var middlewareManager = new MiddlewareManager<PeriodType>(new BaseRecordManager<PeriodType>(), new PeriodTypeValidator());

                await middlewareManager.CreateAsync(periodTypes, identityWorkId);

                //Assert
                var found = await middlewareManager.GetByIdsAsync(new List<Guid>() { id }, identityWorkId);
                Assert.True(found.Any());
                var register = found.FirstOrDefault(x => x.ID == id);
                Assert.NotNull(register);

                await middlewareManager.DeleteAsync(new List<Guid>() { id }, identityWorkId);
                found = await middlewareManager.GetByIdsAsync(new List<Guid>() { id }, identityWorkId);
                register = found.FirstOrDefault(x => x.ID == id);
                Assert.Null(register);

            }
        }

        public class PeriodTypeBuilder
        {

            public static PeriodType Build(Guid id, Guid identityWorkId, string originalName)
            {
                return new PeriodType()
                {
                    ID = id,
                    Active = true,
                    company = identityWorkId,
                    Timestamp = DateTime.UtcNow,
                    InstanceID = Guid.NewGuid(),
                    Description = "Turno mixto",
                    CreationDate = DateTime.Now,
                    Name = originalName,
                    StatusID = 1,
                    PaymentPeriodicity = (PaymentPeriodicity)10,
                    PeriodTotalDays = 15,
                    PaymentDays = 10,
                    ExtraordinaryPeriod = false,
                    MonthCalendarFixed = false,
                    FortnightPaymentDays = (AdjustmentPay_16Days_Febrary)15,
                    PaymentDayPosition = 14
                };
            }
        }
    }
}
