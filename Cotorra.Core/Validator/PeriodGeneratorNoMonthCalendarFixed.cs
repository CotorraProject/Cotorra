using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cotorra.Schema;

namespace Cotorra.Core.Validator
{
    public class PeriodGeneratorNoMonthCalendarFixed : PeriodDetailGenerator
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


            var periodDays = periodType.PeriodTotalDays - 1;
            var paydayPosition = periodDays; //Aqui tomar en cuenta la posicion de la fecha de pago ya que la tengamos
            var periodNumber = 1;
            var nextPeriodNumber = 1;
            var paymentDays = periodType.PaymentDays;

            var periodInitialDate = periodType.InitialDate;
            var periodFinalDate = periodInitialDate.AddDays((double)periodDays);
            var periodPaydayDate = periodInitialDate.AddDays((double)paydayPosition);

            //En periodo que no ajustan al mes calendario la fecha de pago es importante para determinar a que mes pertenecen
            // y al mismo tiempo saber si sigue entrando en el año
            while (periodPaydayDate <= finalDateYear)
            {
                nextPeriodNumber++;
                var nextPeriodInitialDate = periodFinalDate.AddDays(1);
                var nextPeriodFinalDate = nextPeriodInitialDate.AddDays((double)periodDays);
                var nextPeriodPaydayDate = nextPeriodInitialDate.AddDays((double)paydayPosition);

                periods.Add(new PeriodDetail
                {
                    Number = periodNumber,
                    InitialDate = periodInitialDate,
                    FinalDate = periodFinalDate,
                    PaymentDays = paymentDays,
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
                periodInitialDate = nextPeriodInitialDate;
                periodFinalDate = nextPeriodFinalDate;
                periodPaydayDate = nextPeriodPaydayDate;
            }

            return periods;
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
            if (evaluatedPeriod.Number == FIRST_NUMBER_PERIOD)
            {
                return PeriodMonth.Initial;
            }
            else if (evaluatedPeriod.Number == lastPeriodOfYear.Number)
            {
                return PeriodMonth.Final;
            }
            else if (evaluatedPeriod.Month < nextPeriod.Month)
            {
                return PeriodMonth.Final;
            }
            else if (evaluatedPeriod.Month > previousPeriod.Month)
            {
                return PeriodMonth.Initial;
            }
            else
            {
                return PeriodMonth.None;
            }
        }
    }
}
