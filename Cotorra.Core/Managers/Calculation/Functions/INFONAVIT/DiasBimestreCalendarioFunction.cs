using MoreLinq;
using Cotorra.Core.Utils;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.INFONAVIT
{
    public class DiasBimestreCalendarioFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public DiasBimestreCalendarioFunction(FunctionParams functionParams)
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
            _x = argumentValue;
        }

        public double calculate()
        {
            var result = 0.0;
            var month = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate.Month;
            var year = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate.Year;

            if (month % 2 == 0)
            {
                var lastDateMonth = new DateTime(year, month + 1, 1).AddDays(-1);
                result = (lastDateMonth - new DateTime(year, month - 1, 1)).TotalDays + 1;
            }
            else
            {
                var currentMonth = new DateTime(year, month, 1);
                var lastDateOfNextMonth = new DateTime(year, month + 2, 1).AddDays(-1);
                result = (lastDateOfNextMonth - currentMonth).TotalDays + 1;
            }

            return result;
        }
        public FunctionExtension clone()
        {
            return new DiasBimestreCalendarioFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}