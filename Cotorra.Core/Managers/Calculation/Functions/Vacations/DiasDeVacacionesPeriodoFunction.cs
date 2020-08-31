using Cotorra.Core.Utils;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions
{
    public class DiasDeVacacionesPeriodoFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public DiasDeVacacionesPeriodoFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }
        #endregion

        public int getParametersNumber()
        {
            return 1;
        }
        public void setParameterValue(int argumentIndex, double argumentValue)
        {

        }

        public double calculate()
        {
            var result = 0.0;
            var lstVacations = _functionParams.CalculationBaseResult.Vacations.AsParallel().ToArray();
            var dateTimeUtil = new DateTimeUtil();

            for (int i = 0; i < _functionParams.CalculationBaseResult.Vacations.Count(); i++)
            {
                var periodInitialDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.InitialDate;
                var periodFinalDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;
                var vacationInitialDate = lstVacations[i].InitialDate;
                var vacationFinalDate = lstVacations[i].FinalDate;

                result += dateTimeUtil.InclusiveDays(periodInitialDate, periodFinalDate, vacationInitialDate, vacationFinalDate);
                var removeVacationDaysOff = lstVacations[i].VacationDaysOff.Count(daysoff => daysoff.Date >= periodInitialDate && daysoff.Date <= periodFinalDate);
                result -= removeVacationDaysOff;

            }

            return Convert.ToDouble(result);
        }
        public FunctionExtension clone()
        {
            return new DiasDeVacacionesPeriodoFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}
