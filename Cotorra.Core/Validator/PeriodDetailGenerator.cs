using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public abstract class PeriodDetailGenerator
    {
        protected const int FIRST_NUMBER_PERIOD = 1;
        protected const int ODD_NUMBER = 1;
        protected const int EVEN_NUMBER = 0;
        protected Period periodType;
        protected Guid companyID;
        protected Guid instanceID;
        protected Guid userID;
        protected int currentPeriodNumber;

        protected PeriodStatus GetPeriodStatus(int evaluatedPeriodNumber)
        {
            if (currentPeriodNumber == evaluatedPeriodNumber)
            {
                return PeriodStatus.Calculating;
            }
            else if (evaluatedPeriodNumber < currentPeriodNumber)
            {
                return PeriodStatus.NA;
            }
            else
            {
                return PeriodStatus.Open;
            }
        }

        protected PeriodBimonthlyIMSS getPeriodBimonthlyIMSS(int month, PeriodMonth periodMonth)
        {
            if ((periodMonth == PeriodMonth.Initial || periodMonth == PeriodMonth.Both) && (month % 2 == ODD_NUMBER))
            {
                return PeriodBimonthlyIMSS.Initial;
            }
            else if ((periodMonth == PeriodMonth.Final || periodMonth == PeriodMonth.Both) && (month % 2 == EVEN_NUMBER))
            {
                return PeriodBimonthlyIMSS.Final;
            }
            else
            {
                return PeriodBimonthlyIMSS.None;
            }
        }
    }
}
