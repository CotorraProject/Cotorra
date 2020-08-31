using CotorraNode.Identity.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public class PeriodSimulator
    {
        private PeriodDetail CreateDefaultPeriodDetail(Period period, Guid company, Guid instance, Guid user, int number,
           DateTime initialDate, DateTime finalDate,
           bool initialBimonthlyIMSS, bool finalBimonthlyIMSS, bool initialMonth, bool finalMonth,
           bool initialPayrollYear, bool finalPayrollYear, decimal paymentDays,
           DateTime? seventhDay = null)
        {
            var periodDetail = new PeriodDetail();
            periodDetail.Active = true;
            periodDetail.company = company;
            periodDetail.user = user;
            periodDetail.InstanceID = instance;
            periodDetail.CreationDate = DateTime.UtcNow;
            periodDetail.DeleteDate = null;
            periodDetail.Name = $"Period {number}";
            periodDetail.Description = $"{initialDate.ToShortDateString()} to {finalDate.ToShortDateString()}";
            periodDetail.ID = Guid.NewGuid();

            periodDetail.InitialDate = initialDate;
            periodDetail.FinalDate = finalDate;
            //BimonthleImss
            periodDetail.PeriodBimonthlyIMSS = initialBimonthlyIMSS ? PeriodBimonthlyIMSS.Initial : PeriodBimonthlyIMSS.None;
            if (periodDetail.PeriodBimonthlyIMSS == PeriodBimonthlyIMSS.None)
            {
                periodDetail.PeriodBimonthlyIMSS = finalBimonthlyIMSS ? PeriodBimonthlyIMSS.Final : PeriodBimonthlyIMSS.None;
            }
            //Month
            periodDetail.PeriodMonth = initialMonth ? PeriodMonth.Initial : PeriodMonth.None;
            if (periodDetail.PeriodMonth == PeriodMonth.None)
            {
                periodDetail.PeriodMonth = finalMonth ? PeriodMonth.Final : PeriodMonth.None;
            }
            periodDetail.PeriodID = period.ID;
            periodDetail.Number = number;
            //FiscalYear
            periodDetail.PeriodFiscalYear = initialPayrollYear ? PeriodFiscalYear.Initial : PeriodFiscalYear.None;
            if (periodDetail.PeriodFiscalYear == PeriodFiscalYear.None)
            {
                periodDetail.PeriodFiscalYear = finalPayrollYear ? PeriodFiscalYear.Final : PeriodFiscalYear.None;
            }
            periodDetail.PaymentDays = paymentDays;
            periodDetail.StatusID = 1;
            periodDetail.Timestamp = DateTime.UtcNow;

            return periodDetail;
        }

        private int lastDayOfMonth(int year, int month)
        {
            return DateTime.DaysInMonth(year, month);
        }


        public List<PeriodDetail> GetPeriodDetails(PaymentPeriodicity paymentPeriodicity, DateTime initialDate,
            int periodTotalDays, bool fixToMonthCalendar)
        {
            var lstPeriodDetail = new List<PeriodDetail>();
            var finalDate = initialDate.AddYears(1);

            //calculates the difference betweent initial and final date to the proximous final year
            //si la fecha inicial es más próxima al final del año inicial que el inicio del siguiente año, tomamos el año final
            if ((new DateTime(initialDate.Year, 12, 31) - initialDate) <= (finalDate - new DateTime(finalDate.Year, 1, 1)))
            {
                finalDate = new DateTime(finalDate.Year, 12, 31);
            }
            else
            {
                finalDate = new DateTime(initialDate.Year, 12, 31);
            }

            if (paymentPeriodicity == PaymentPeriodicity.Biweekly
                || paymentPeriodicity == PaymentPeriodicity.Monthly
                || paymentPeriodicity == PaymentPeriodicity.Weekly)
            {
                var paymentDays = periodTotalDays - 1;
                var actualNumber = 0;

                var initialDay = initialDate.Date;
                var finalDay = finalDate.Date;
                var isFirstPeriodDetail = true;

                var company = Guid.NewGuid();
                var instanceId = Guid.NewGuid();
                var userId = Guid.NewGuid();

                var period = new Period() { 
                    Active = true,
                    company = company,
                    CreationDate = DateTime.Now,
                    DeleteDate = null,
                    Description = "Periodo dumyy",
                    ExtraordinaryPeriod = false,
                    FinalDate = finalDate,
                    FiscalYear = finalDate.Year,
                    FortnightPaymentDays = AdjustmentPay_16Days_Febrary.PayCalendarDays,
                    ID = Guid.NewGuid(),
                    IdentityID = userId,
                    InstanceID = instanceId,
                    InitialDate = initialDate,
                    IsActualFiscalYear = true,
                    IsFiscalYearClosed = false,
                    MonthCalendarFixed = fixToMonthCalendar,
                    PaymentPeriodicity = paymentPeriodicity,
                    StatusID = 1,
                    PeriodTotalDays = periodTotalDays,
                };
                
                while (initialDay < finalDay)
                {
                    var finalTempDate = initialDay.AddDays((double)paymentDays);
                    int previousBiweeklyMonth = 0;
                    bool isInitialMonth = false;
                    bool isFinalMonth = false;

                    var nextPeriodInitial = initialDay.AddDays((double)paymentDays);
                    var nextPeriodFinal = nextPeriodInitial.AddDays((double)paymentDays);

                    //if the last period is greater than final day of exercise
                    if (finalTempDate > finalDay)
                    {
                        break;
                    }

                    //MonthCalendarFixed
                    if (fixToMonthCalendar)
                    {
                        if (finalTempDate.Month > initialDay.Month)
                        {
                            finalTempDate = new DateTime(initialDay.Year, initialDay.Month, lastDayOfMonth(initialDay.Year, initialDay.Month)).Date;
                        }

                        if (nextPeriodInitial.Month != nextPeriodFinal.Month &&
                            (lastDayOfMonth(initialDay.Year, initialDay.Month) - nextPeriodInitial.Day) < paymentDays
                            && finalTempDate.Month != 2)
                        {
                            finalTempDate = new DateTime(initialDay.Year, initialDay.Month, lastDayOfMonth(initialDay.Year, initialDay.Month)).Date;
                        }

                        previousBiweeklyMonth = initialDay.AddDays(-1 * (double)paymentDays).Month;
                        isInitialMonth = initialDay.Month != previousBiweeklyMonth;
                        isFinalMonth = new DateTime(initialDay.Year, initialDay.Month, lastDayOfMonth(initialDay.Year, initialDay.Month)).Date == finalTempDate.Date;
                    }
                    //When not used MonthFixed
                    else
                    {
                        previousBiweeklyMonth = initialDay.AddDays(-1 * (double)paymentDays).Month;
                        isInitialMonth = initialDay.Month != previousBiweeklyMonth;
                        isFinalMonth = nextPeriodInitial.Month != initialDay.Month;
                    }

                    var defaultPeriodDetail = CreateDefaultPeriodDetail(period, company, instanceId, userId,
                            number: ++actualNumber,
                            initialDate: initialDay, finalDate: finalTempDate,
                            initialBimonthlyIMSS: initialDay.Month % 2 != 0 && isInitialMonth,
                            finalBimonthlyIMSS: initialDay.Month % 2 == 0 && isFinalMonth,
                            initialMonth: isInitialMonth, finalMonth: isFinalMonth,
                            initialPayrollYear: isInitialMonth && initialDay.Month == 1,
                            finalPayrollYear: isFinalMonth && initialDay.Month == 12,
                            paymentDays: paymentDays, null);

                    if (isFirstPeriodDetail)
                    {
                        defaultPeriodDetail.PeriodStatus = PeriodStatus.Calculating;
                        isFirstPeriodDetail = false;
                    }
                    else
                    {
                        defaultPeriodDetail.PeriodStatus = PeriodStatus.Open;
                    }

                    lstPeriodDetail.Add(defaultPeriodDetail);

                    initialDay = finalTempDate.AddDays(1);
                }

            }

            return lstPeriodDetail;
        }
    }
}
