using Cotorra.Core.Utils;
using System;
using Xunit;


namespace Cotorra.UnitTest.Util
{
   
    public class DateTimeUtilUT
    {
         

        [Fact]
        public void ShouldBeZeroYearsZeroMonthsZeroDayForSameDate()
        {
            DateTime now = DateTime.Now;
            CalculateDateDifference calculateDateDifference = new CalculateDateDifference(now, now);
           var res =  calculateDateDifference.ToString();            
            Assert.NotEmpty(res);
            Assert.Equal("0 Year(s), 0 month(s), 0 day(s)", res);
        }

        [Fact]
        public void ShouldBeZeroYearsZeroMonths15DayForAdd15()
        {
            DateTime now = DateTime.Now;
            DateTime nowPlusFifteen = DateTime.Now.AddDays(15);
            CalculateDateDifference calculateDateDifference = new CalculateDateDifference(now, nowPlusFifteen);
            var res = calculateDateDifference.ToString();
            Assert.NotEmpty(res);
            Assert.Equal("0 Year(s), 0 month(s), 15 day(s)", res);
        }


        [Fact]
        public void ShouldBeZeroYearsOneMonths0DaysForAdd30()
        {
            DateTime now = DateTime.Now;
            DateTime nowPlusFifteen = DateTime.Now.AddDays(30);
            CalculateDateDifference calculateDateDifference = new CalculateDateDifference(now, nowPlusFifteen);
            var res = calculateDateDifference.ToString();
            Assert.NotEmpty(res);
            Assert.Equal("0 Year(s), 1 month(s), 0 day(s)", res);
        }

        [Fact]
        public void ShouldBeOneYears0Months0DaysForAdd365()
        {
            DateTime now = DateTime.Now;
            DateTime nowPlusFifteen = DateTime.Now.AddDays(365);
            CalculateDateDifference calculateDateDifference = new CalculateDateDifference(now, nowPlusFifteen);
            var res = calculateDateDifference.ToString();
            Assert.NotEmpty(res);
            Assert.Equal("1 Year(s), 0 month(s), 0 day(s)", res);
        }


        [Fact]
        public void ShouldBeOneYearBetwwn2019_01_01and_2020_01_01()
        {
            DateTime now = new DateTime(2019, 1, 1);
            DateTime noworNever = new DateTime(2020, 1, 1);
            CalculateDateDifference calculateDateDifference = new CalculateDateDifference(now, noworNever);
            var res = calculateDateDifference.ToString();
            Assert.NotEmpty(res);
            Assert.Equal("1 Year(s), 0 month(s), 0 day(s)", res);
        }


        [Fact]
        public void ShouldBeTenYearElevenMonthsBetween2019_01_01and_2029_11_01()
        {
            DateTime now = new DateTime(2019, 1, 1);
            DateTime noworNever = new DateTime(2029, 12, 1);
            CalculateDateDifference calculateDateDifference = new CalculateDateDifference(now, noworNever);
            var res = calculateDateDifference.ToString();
            Assert.NotEmpty(res);
            Assert.Equal("10 Year(s), 11 month(s), 0 day(s)", res);
        }

        [Fact]
        public void ShouldBeTenYearFourMonthsBetween2019_01_01and_2029_05_10()
        {
            DateTime now = new DateTime(2019, 1, 1);
            DateTime noworNever = new DateTime(2029, 5, 10);
            CalculateDateDifference calculateDateDifference = new CalculateDateDifference(now, noworNever);
            var res = calculateDateDifference.ToString();
            Assert.NotEmpty(res);
            Assert.Equal("10 Year(s), 4 month(s), 9 day(s)", res);
        }

        [Fact]
        public void ShouldBe0Year0Months28DaysBetween2020_02_01_and_2020_02_29()
        {
            DateTime now = new DateTime(2020, 2, 1);
            DateTime noworNever = new DateTime(2020, 2, 29);
            CalculateDateDifference calculateDateDifference = new CalculateDateDifference(now, noworNever);
            var res = calculateDateDifference.ToString();
            Assert.NotEmpty(res);
            Assert.Equal("0 Year(s), 0 month(s), 28 day(s)", res);
        }

        [Fact]
        public void ShouldBeZeroYearsZeroMonthsZeroDayForSameDateUsingProperty()
        {
            DateTime now = DateTime.Now;
            CalculateDateDifference calculateDateDifference = new CalculateDateDifference( );
            var res = calculateDateDifference.Calculate(now, now).ToString();
            
            Assert.NotEmpty(res);
            Assert.Equal("0 Year(s), 0 month(s), 0 day(s)", res);
        }

    }
}
