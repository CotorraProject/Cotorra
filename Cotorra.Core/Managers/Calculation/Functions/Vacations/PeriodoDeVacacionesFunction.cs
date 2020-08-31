using MoreLinq;
using Cotorra.Core.Utils;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions
{
    /// <summary>
    /// Esta Variable regresa un 1(uno) si el empleado esta de vacaciones todo el periodo; regresa 0(cero) en caso contrario.
    /// </summary>
    public class PeriodoDeVacacionesFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public PeriodoDeVacacionesFunction(FunctionParams functionParams)
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
            var periodDetail = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail;
            var vacations = _functionParams.CalculationBaseResult.Vacations;
            double vacationsDays = 0;
            vacations.ForEach(vacation =>
            {
                vacationsDays += new DateTimeUtil().InclusiveDays(periodDetail.InitialDate, periodDetail.FinalDate, vacation.InitialDate, vacation.FinalDate);
            });

            var paymentDays = Convert.ToDouble(_functionParams.CalculationBaseResult.Overdraft.PeriodDetail.PaymentDays);

            if (vacationsDays >= paymentDays)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        public FunctionExtension clone()
        {
            return new PeriodoDeVacacionesFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}
