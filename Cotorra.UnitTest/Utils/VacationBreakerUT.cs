using Cotorra.Core;
using Xunit;
using System;
using Cotorra.Schema;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Cotorra.UnitTest.Util
{

    public class VacationBreakerUT
    {


        [Fact]
        public async Task NotBreakeVacationCardPayVacationInitiallyAsync()
        {
            Vacation vacationCard = new VacationManagerUT().Build(new Guid(), new Guid(), Guid.Parse("F81867AA-4A4F-4FFE-9510-61A7E72C4177"),
                DateTime.Now, DateTime.Now.AddDays(10));
            VacationCardManager manager = new VacationCardManager();

            var res = await manager.BrekeOrNotAsync(vacationCard, Guid.Parse("0C08DAA6-F775-42A8-B75E-1B9B685B7977"), Guid.Parse("1E84AB62-574E-4373-A8D7-80910BD5D607"), HolidayPaymentConfiguration.PayVacationInitially);

            Assert.NotEmpty(res);
            Assert.Single(res);

        }


        //periodo semanal
        [Fact]
        public async Task BreakeVacationCardPayVacationInPeriodAsync()
        {
            var initialDateVacation = new DateTime(2020, 01, 01);
            var finalDateVacation = new DateTime(2020, 01, 11);
            Vacation vacationCard = new VacationManagerUT().Build(new Guid(), new Guid(), Guid.Parse("F81867AA-4A4F-4FFE-9510-61A7E72C4177"),
                initialDateVacation, finalDateVacation);
            VacationCardManager manager = new VacationCardManager();

            var res = await manager.BrekeOrNotAsync(vacationCard, Guid.Parse("0C08DAA6-F775-42A8-B75E-1B9B685B7977"), Guid.Parse("1E84AB62-574E-4373-A8D7-80910BD5D607"), HolidayPaymentConfiguration.PayVacationInPeriod);

            Assert.NotEmpty(res);
            Assert.Equal(2, res.Count);

            var card1 = res[0];
            var card2 = res[1];

            Assert.Equal(initialDateVacation, card1.InitialDate);
            Assert.Equal(initialDateVacation.AddDays(6), card1.FinalDate);

            Assert.Equal(initialDateVacation.AddDays(7), card2.InitialDate);
            Assert.Equal(finalDateVacation, card2.FinalDate);

        }

        //periodo semanal
        [Fact]
        public async Task BreakeVacationCardPayVacationInPeriodAsyncThreePeriods()
        {
            var initialDateVacation = new DateTime(2020, 01, 02);
            var finalDateVacation = new DateTime(2020, 01, 20);
            Vacation vacationCard = new VacationManagerUT().Build(new Guid(), new Guid(), Guid.Parse("F81867AA-4A4F-4FFE-9510-61A7E72C4177"),
                initialDateVacation, finalDateVacation);
            VacationCardManager manager = new VacationCardManager();

            var res = await manager.BrekeOrNotAsync(vacationCard, Guid.Parse("0C08DAA6-F775-42A8-B75E-1B9B685B7977"), Guid.Parse("1E84AB62-574E-4373-A8D7-80910BD5D607"), HolidayPaymentConfiguration.PayVacationInPeriod);

            Assert.NotEmpty(res);
            Assert.Equal(3, res.Count);

            var card1 = res[0];
            var card2 = res[1];
            var card3 = res[2];

            Assert.Equal(initialDateVacation, card1.InitialDate);
            Assert.Equal(initialDateVacation.AddDays(5), card1.FinalDate);

            Assert.Equal(new DateTime(2020, 01, 08), card2.InitialDate);
            Assert.Equal(new DateTime(2020, 01, 14), card2.FinalDate);

            Assert.Equal(new DateTime(2020, 01, 15), card3.InitialDate);
            Assert.Equal(new DateTime(2020, 01, 20), card3.FinalDate);

        }

        //periodo semanal
        [Fact]
        public async Task BreakeVacationCardPayVacationInPeriodAsyncOnePeriod()
        {
            var initialDateVacation = new DateTime(2020, 01, 01);
            var finalDateVacation = new DateTime(2020, 01, 07);
            Vacation vacationCard = new VacationManagerUT().Build(new Guid(), new Guid(), Guid.Parse("F81867AA-4A4F-4FFE-9510-61A7E72C4177"),
                initialDateVacation, finalDateVacation);
            VacationCardManager manager = new VacationCardManager();

            var res = await manager.BrekeOrNotAsync(vacationCard, Guid.Parse("0C08DAA6-F775-42A8-B75E-1B9B685B7977"), Guid.Parse("1E84AB62-574E-4373-A8D7-80910BD5D607"), HolidayPaymentConfiguration.PayVacationInPeriod);

            Assert.NotEmpty(res);
            Assert.Single(res);

            var card1 = res[0];

            Assert.Equal(new DateTime(2020, 01, 01), card1.InitialDate);
            Assert.Equal(new DateTime(2020, 01, 07), card1.FinalDate);

        }


        //periodo semanal
        [Fact]
        public async Task BreakeVacationCardPayVacationInPeriodWithDaysOffAsync()
        {
            var initialDateVacation = new DateTime(2020, 01, 01);
            var finalDateVacation = new DateTime(2020, 01, 11);
            Vacation vacationCard = new VacationManagerUT().Build(new Guid(), new Guid(), Guid.Parse("F81867AA-4A4F-4FFE-9510-61A7E72C4177"),
                initialDateVacation, finalDateVacation);
            VacationCardManager manager = new VacationCardManager();

            List<VacationDaysOff> VacationDaysOff = new List<VacationDaysOff>() {
                new VacationDaysOff()
                {
                    Active = true,
                    company = new Guid(),
                    DeleteDate = null,
                    ID = Guid.NewGuid(),
                    user = Guid.NewGuid(),
                    Date = new DateTime(2020, 01, 10),
                    VacationID = vacationCard.ID
                }
            };

            vacationCard.VacationDaysOff = VacationDaysOff;

            var res = await manager.BrekeOrNotAsync(vacationCard, Guid.Parse("0C08DAA6-F775-42A8-B75E-1B9B685B7977"), Guid.Parse("1E84AB62-574E-4373-A8D7-80910BD5D607"), HolidayPaymentConfiguration.PayVacationInPeriod);

            Assert.NotEmpty(res);
            Assert.Equal(2, res.Count);

            var card1 = res[0];
            var card2 = res[1];

            Assert.Equal(initialDateVacation, card1.InitialDate);
            Assert.Equal(initialDateVacation.AddDays(6), card1.FinalDate);

            Assert.Equal(initialDateVacation.AddDays(7), card2.InitialDate);
            Assert.Equal(finalDateVacation, card2.FinalDate);
            Assert.NotNull(card2.VacationDaysOff);
            Assert.Null(card1.VacationDaysOff);
            Assert.Equal(7,card1.VacationsDays);
            Assert.Equal(0m, card1.VacationsBonusDays);
            Assert.Equal(3, card2.VacationsDays);
            Assert.Equal(0m, card2.VacationsBonusDays);

        }

        //periodo semanal
        [Fact]
        public async Task BreakeVacationCardPayVacationInPeriodWithDaysOffAndBonusAsync()
        {
            var initialDateVacation = new DateTime(2020, 01, 01);
            var finalDateVacation = new DateTime(2020, 01, 11);
            Vacation vacationCard = new VacationManagerUT().Build(new Guid(), new Guid(), Guid.Parse("F81867AA-4A4F-4FFE-9510-61A7E72C4177"),
                initialDateVacation, finalDateVacation);
            vacationCard.VacationsBonusPercentage = 25;
            VacationCardManager manager = new VacationCardManager();

            List<VacationDaysOff> VacationDaysOff = new List<VacationDaysOff>() {
                new VacationDaysOff()
                {
                    Active = true,
                    company = new Guid(),
                    DeleteDate = null,
                    ID = Guid.NewGuid(),
                    user = Guid.NewGuid(),
                    Date = new DateTime(2020, 01, 10),
                    VacationID = vacationCard.ID
                }
            };

            vacationCard.VacationDaysOff = VacationDaysOff;

            var res = await manager.BrekeOrNotAsync(vacationCard, Guid.Parse("0C08DAA6-F775-42A8-B75E-1B9B685B7977"), Guid.Parse("1E84AB62-574E-4373-A8D7-80910BD5D607"), HolidayPaymentConfiguration.PayVacationsAndBonusInPeriod);

            Assert.NotEmpty(res);
            Assert.Equal(2, res.Count);

            var card1 = res[0];
            var card2 = res[1];

            Assert.Equal(initialDateVacation, card1.InitialDate);
            Assert.Equal(initialDateVacation.AddDays(6), card1.FinalDate);

            Assert.Equal(initialDateVacation.AddDays(7), card2.InitialDate);
            Assert.Equal(finalDateVacation, card2.FinalDate);
            Assert.NotNull(card2.VacationDaysOff);
            Assert.Null(card1.VacationDaysOff);
            Assert.Equal(7, card1.VacationsDays);
            Assert.Equal(1.75m, card1.VacationsBonusDays);
            Assert.Equal(3, card2.VacationsDays);
            Assert.Equal(.75m, card2.VacationsBonusDays);

        }


        //periodo semanal
        [Fact]
        public async Task BreakeVacationCardPayVacationInPeriodAsyncinJuly()
        {
            var initialDateVacation = new DateTime(2020, 07, 08);
            var finalDateVacation = new DateTime(2020, 07, 16);
            Vacation vacationCard = new VacationManagerUT().Build(new Guid(), new Guid(), Guid.Parse("F81867AA-4A4F-4FFE-9510-61A7E72C4177"),
                initialDateVacation, finalDateVacation);
            VacationCardManager manager = new VacationCardManager();

            var res = await manager.BrekeOrNotAsync(vacationCard, Guid.Parse("0C08DAA6-F775-42A8-B75E-1B9B685B7977"), Guid.Parse("1E84AB62-574E-4373-A8D7-80910BD5D607"), HolidayPaymentConfiguration.PayVacationInPeriod);

            Assert.NotEmpty(res);
            Assert.Equal(2, res.Count);

            var card1 = res[0];
            var card2 = res[1];

            Assert.Equal(initialDateVacation, card1.InitialDate);
            Assert.Equal(initialDateVacation.AddDays(6), card1.FinalDate);

            Assert.Equal(initialDateVacation.AddDays(7), card2.InitialDate);
            Assert.Equal(finalDateVacation, card2.FinalDate);

        }

        //periodo semanal
        [Fact]
        public async Task BreakeVacationCardPayVacationInPeriodWithDaysOffAsyncCheckDays()
        {
            var initialDateVacation = new DateTime(2020, 01, 10);
            var finalDateVacation = new DateTime(2020, 01, 21);
            Vacation vacationCard = new VacationManagerUT().Build(new Guid(), new Guid(), Guid.Parse("F81867AA-4A4F-4FFE-9510-61A7E72C4177"),
                initialDateVacation, finalDateVacation);
            VacationCardManager manager = new VacationCardManager();

            List<VacationDaysOff> VacationDaysOff = new List<VacationDaysOff>() {
                new VacationDaysOff()
                {
                    Active = true,
                    company = new Guid(),
                    DeleteDate = null,
                    ID = Guid.NewGuid(),
                    user = Guid.NewGuid(),
                    Date = new DateTime(2020, 01, 11),
                    VacationID = vacationCard.ID
                },
                 new VacationDaysOff()
                {
                    Active = true,
                    company = new Guid(),
                    DeleteDate = null,
                    ID = Guid.NewGuid(),
                    user = Guid.NewGuid(),
                    Date = new DateTime(2020, 01, 12),
                    VacationID = vacationCard.ID
                },
                new VacationDaysOff()
                {
                    Active = true,
                    company = new Guid(),
                    DeleteDate = null,
                    ID = Guid.NewGuid(),
                    user = Guid.NewGuid(),
                    Date = new DateTime(2020, 01, 18),
                    VacationID = vacationCard.ID
                },
                new VacationDaysOff()
                {
                    Active = true,
                    company = new Guid(),
                    DeleteDate = null,
                    ID = Guid.NewGuid(),
                    user = Guid.NewGuid(),
                    Date = new DateTime(2020, 01, 19),
                    VacationID = vacationCard.ID
                },

            };

            vacationCard.VacationDaysOff = VacationDaysOff;

            var res = await manager.BrekeOrNotAsync(vacationCard, Guid.Parse("0C08DAA6-F775-42A8-B75E-1B9B685B7977"), Guid.Parse("1E84AB62-574E-4373-A8D7-80910BD5D607"), HolidayPaymentConfiguration.PayVacationInPeriod);

            Assert.NotEmpty(res);
            Assert.Equal(2, res.Count);

            var card1 = res[0];
            var card2 = res[1];

            Assert.Equal(initialDateVacation, card1.InitialDate);
            Assert.Equal(initialDateVacation.AddDays(6), card1.FinalDate);

            Assert.Equal(initialDateVacation.AddDays(7), card2.InitialDate);
            Assert.Equal(finalDateVacation, card2.FinalDate);
            Assert.NotNull(card2.VacationDaysOff);
            Assert.Null(card1.VacationDaysOff);
            Assert.Equal(7, card1.VacationsDays);
            Assert.Equal(0m, card1.VacationsBonusDays);
            Assert.Equal(3, card2.VacationsDays);
            Assert.Equal(0m, card2.VacationsBonusDays);

        }
    }
}
