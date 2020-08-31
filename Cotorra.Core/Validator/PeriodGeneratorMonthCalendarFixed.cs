using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class PeriodGeneratorMonthCalendarFixed : PeriodDetailGenerator
    {
        
        public List<PeriodDetail> GeneratePeriods(Period periodType, int currentPeriod, Guid company, Guid instance, Guid user)
        {
            this.periodType = periodType;
            currentPeriodNumber = currentPeriod;
            companyID = company;
            instanceID = instance;
            userID = user;
            var periods = GeneratePeriodsDates();
            SetPeriodsFlags(periods);

            return periods;
        }

        private List<PeriodDetail> GeneratePeriodsDates()
        {
            List<PeriodDetail> periods = new List<PeriodDetail>();

            var year = (periodType.InitialDate.Month == 12) ? periodType.InitialDate.Year + 1 : periodType.InitialDate.Year;

            var finalDateYear = new DateTime(year, 12, 31);


            var periodDays = periodType.PaymentDays - 1;
            var paydayPosition = periodDays; //Aqui tomar en cuenta la posicion de la fecha de pago ya que la tengamos
            var periodNumber = 1;

            var periodInitialDate = periodType.InitialDate;
            var periodFinalDate = periodInitialDate.AddDays((double)periodDays);
            var periodPaydayDate = periodInitialDate.AddDays((double)paydayPosition);


            while (periodPaydayDate <= finalDateYear)
            {
                //En periodos ajuste a calendario un periodo no puede ir mas alla del día del fin de mes
                var nextPeriodInitial = periodInitialDate.AddDays((double)periodDays);
                var nextPeriodFinal = nextPeriodInitial.AddDays((double)periodDays);
                if (periodFinalDate.Month > periodInitialDate.Month)
                {
                    periodFinalDate = new DateTime(year, periodInitialDate.Month, DateTime.DaysInMonth(year, periodInitialDate.Month)).Date;
                }

                if (nextPeriodInitial.Month != nextPeriodFinal.Month &&
                    (DateTime.DaysInMonth(year, periodInitialDate.Month) - nextPeriodInitial.Day) < periodType.PaymentDays
                    && periodFinalDate.Month != 2)
                {
                    periodFinalDate = new DateTime(year, periodInitialDate.Month, DateTime.DaysInMonth(year, periodInitialDate.Month)).Date;
                }

                periods.Add(new PeriodDetail
                {
                    Number = periodNumber,
                    InitialDate = periodInitialDate,
                    FinalDate = periodFinalDate,
                    PaymentDays = GetPaymentDays(periodInitialDate, periodFinalDate),
                    Active = true,
                    company = companyID,
                    user = userID,
                    InstanceID = instanceID,
                    CreationDate = DateTime.UtcNow,
                    DeleteDate = null,
                    Name = $"Period {periodNumber}",
                    Description = $"{periodInitialDate.ToShortDateString()} to {periodFinalDate.ToShortDateString()}",
                    ID = Guid.NewGuid(),
                    SeventhDays = periodType.SeventhDays,
                    SeventhDayPosition = periodType.SeventhDayPosition,
                    PeriodID = periodType.ID,
                    StatusID = 1,
                    Timestamp = DateTime.UtcNow,
                    PeriodStatus = GetPeriodStatus(periodNumber),
                });

                periodNumber++;
                periodInitialDate = periodFinalDate.AddDays(1);
                periodFinalDate = periodInitialDate.AddDays((double)periodDays);
                periodPaydayDate = periodInitialDate.AddDays((double)paydayPosition);
            }

            return periods;
        }

        private decimal GetPaymentDays(DateTime initialDate, DateTime finalDate)
        {
            var periodDays = (finalDate.Date - initialDate.Date).Days + 1;
            if ((periodDays != periodType.PaymentDays) && 
                (periodType.FortnightPaymentDays == AdjustmentPay_16Days_Febrary.PayCalendarDays))
            {
                return periodDays;
            }
            else
            {
                return periodType.PaymentDays;
            }
        }

        private void SetPeriodsFlags(List<PeriodDetail> periods)
        {
            PeriodDetail previousPeriod = null;
            PeriodDetail nextPeriod = null;
            int posElement = 0;
            foreach (var period in periods)
            {
                nextPeriod = (posElement + 1 < periods.Count) ? periods[posElement + 1] : null;
                var periodFiscalYear = getPeriodFiscalYear(period, periods.LastOrDefault());
                var periodMonth = getPeriodMonth(period, previousPeriod, nextPeriod, periods.LastOrDefault());
                var periodBimonth = getPeriodBimonthlyIMSS(period.Month, periodMonth);

                period.PeriodFiscalYear = periodFiscalYear;
                period.PeriodMonth = periodMonth;
                period.PeriodBimonthlyIMSS = periodBimonth;

                previousPeriod = period;
                posElement++;
            }
        }

        private PeriodFiscalYear getPeriodFiscalYear(PeriodDetail evaluatedPeriod, PeriodDetail lastPeriodOfYear)
        {
            if (evaluatedPeriod.Number == FIRST_NUMBER_PERIOD)
            {
                return PeriodFiscalYear.Initial;
            }
            else if (evaluatedPeriod.Number == lastPeriodOfYear.Number)
            {
                return PeriodFiscalYear.Final;
            }
            else
            {
                return PeriodFiscalYear.None;
            }
        }

        private PeriodMonth getPeriodMonth(PeriodDetail evaluatedPeriod, PeriodDetail previousPeriod, PeriodDetail nextPeriod, PeriodDetail lastPeriodOfYear)
        {
            var firstPeriodOfMonth = false;
            var lastPeriodOfMonth = false;

            if ((evaluatedPeriod.Number == FIRST_NUMBER_PERIOD) || (evaluatedPeriod.Month != previousPeriod.Month))
            {
                firstPeriodOfMonth = true;
            }

            if((evaluatedPeriod.Number == lastPeriodOfYear.Number) || (evaluatedPeriod.Month != nextPeriod.Month))
            {
                lastPeriodOfMonth = true;
            }

            if(firstPeriodOfMonth && lastPeriodOfMonth)
            {
                return PeriodMonth.Both;
            }
            else if(firstPeriodOfMonth)
            {
                return PeriodMonth.Initial;
            }
            else if(lastPeriodOfMonth)
            {
                return PeriodMonth.Final;
            }
            else
            {
                return PeriodMonth.None;
            }
            
        }

    }
}
