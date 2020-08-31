using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public abstract class AccumulatedBase
    {
        public decimal CalculateAccumulated(DateTime periodFinalDate,
            IEnumerable<AccumulatedEmployee> accumulatedEmployee,
            IEnumerable<HistoricAccumulatedEmployee> historicAccumulatedEmployees,
            bool IsMonthly = false)
        {
            var result = 0M;
            if (periodFinalDate.Month == 1)
            {
                var accumulatedPerMonth = 0M;
                if (IsMonthly)
                {
                    accumulatedPerMonth = historicAccumulatedEmployees.Sum(p => p.January);
                    result = accumulatedPerMonth;
                }
                else
                {
                    result = accumulatedEmployee.Sum(p => p.January) + accumulatedPerMonth;
                }
               
            }
            else if (periodFinalDate.Month == 2)
            {
                var accumulatedPerMonth = 0M;
                if (IsMonthly)
                {
                    accumulatedPerMonth = historicAccumulatedEmployees.Sum(p => p.February);
                    result = accumulatedPerMonth;
                }
                else
                {
                    result = accumulatedEmployee.Sum(p => p.February) + accumulatedPerMonth;
                }
            }
            else if (periodFinalDate.Month == 3)
            {
                var accumulatedPerMonth = 0M;
                if (IsMonthly)
                {
                    accumulatedPerMonth = historicAccumulatedEmployees.Sum(p => p.March);
                    result = accumulatedPerMonth;
                }
                else
                {
                    result = accumulatedEmployee.Sum(p => p.March) + accumulatedPerMonth;
                }
            }
            else if (periodFinalDate.Month == 4)
            {
                var accumulatedPerMonth = 0M;
                if (IsMonthly)
                {
                    accumulatedPerMonth = historicAccumulatedEmployees.Sum(p => p.April);
                    result = accumulatedPerMonth;
                }
                else
                {
                    result = accumulatedEmployee.Sum(p => p.April) + accumulatedPerMonth;
                }
            }
            else if (periodFinalDate.Month == 5)
            {
                var accumulatedPerMonth = 0M;
                if (IsMonthly)
                {
                    accumulatedPerMonth = historicAccumulatedEmployees.Sum(p => p.May);
                    result = accumulatedPerMonth;
                }
                else
                {
                    result = accumulatedEmployee.Sum(p => p.May) + accumulatedPerMonth;
                }
            }
            else if (periodFinalDate.Month == 6)
            {
                var accumulatedPerMonth = 0M;
                if (IsMonthly)
                {
                    accumulatedPerMonth = historicAccumulatedEmployees.Sum(p => p.June);
                    result = accumulatedPerMonth;
                }
                else
                {
                    result = accumulatedEmployee.Sum(p => p.June) + accumulatedPerMonth;
                }
            }
            else if (periodFinalDate.Month == 7)
            {
                var accumulatedPerMonth = 0M;
                if (IsMonthly)
                {
                    accumulatedPerMonth = historicAccumulatedEmployees.Sum(p => p.July);
                    result = accumulatedPerMonth;
                }
                else
                {
                    result = accumulatedEmployee.Sum(p => p.July) + accumulatedPerMonth;
                }
            }
            else if (periodFinalDate.Month == 8)
            {
                var accumulatedPerMonth = 0M;
                if (IsMonthly)
                {
                    accumulatedPerMonth = historicAccumulatedEmployees.Sum(p => p.August);
                    result = accumulatedPerMonth;
                }
                else
                {
                    result = accumulatedEmployee.Sum(p => p.August) + accumulatedPerMonth;
                }
            }
            else if (periodFinalDate.Month == 9)
            {
                var accumulatedPerMonth = 0M;
                if (IsMonthly)
                {
                    accumulatedPerMonth = historicAccumulatedEmployees.Sum(p => p.September);
                    result = accumulatedPerMonth;
                }
                else
                {
                    result = accumulatedEmployee.Sum(p => p.September) + accumulatedPerMonth;
                }
            }
            else if (periodFinalDate.Month == 10)
            {
                var accumulatedPerMonth = 0M;
                if (IsMonthly)
                {
                    accumulatedPerMonth = historicAccumulatedEmployees.Sum(p => p.October);
                    result = accumulatedPerMonth;
                }
                else
                {
                    result = accumulatedEmployee.Sum(p => p.October) + accumulatedPerMonth;
                }
            }
            else if (periodFinalDate.Month == 11)
            {
                var accumulatedPerMonth = 0M;
                if (IsMonthly)
                {
                    accumulatedPerMonth = historicAccumulatedEmployees.Sum(p => p.November);
                    result = accumulatedPerMonth;
                }
                else
                {
                    result = accumulatedEmployee.Sum(p => p.November) + accumulatedPerMonth;
                }
            }
            else if (periodFinalDate.Month == 12)
            {
                var accumulatedPerMonth = 0M;
                if (IsMonthly)
                {
                    accumulatedPerMonth = historicAccumulatedEmployees.Sum(p => p.December);
                    result = accumulatedPerMonth;
                }
                else
                {
                    result = accumulatedEmployee.Sum(p => p.December) + accumulatedPerMonth;
                }
            }

            return result;
        }

        public static decimal CalculateAnualAccumulated(DateTime periodFinalDate,
           IEnumerable<AccumulatedEmployee> accumulatedEmployee,
           IEnumerable<HistoricAccumulatedEmployee> historicAccumulatedEmployees,
           bool IsMonthly = false)
        {
            var result = 0M;
            var accumulatedPerMonth = 0M;
            if (IsMonthly)
            {
                accumulatedPerMonth = historicAccumulatedEmployees.Sum(p => p.January + p.February + 
                p.March + p.April + p.May + p.June + p.July + p.August + p.September + p.October + p.November
                + p.December);
            }
            result = accumulatedEmployee.Sum(p => p.January + p.February +
                p.March + p.April + p.May + p.June + p.July + p.August + p.September + p.October + p.November
                + p.December) + accumulatedPerMonth;

            return result;
        }
    }
}
