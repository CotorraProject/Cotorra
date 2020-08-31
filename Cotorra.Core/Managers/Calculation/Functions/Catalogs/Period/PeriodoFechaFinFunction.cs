using Cotorra.Core.Utils;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class PeriodoFechaFinFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public PeriodoFechaFinFunction(FunctionParams functionParams)
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
            var dateTimeInitOrigin = DateTimeUtil.DATETIME_ORIGIN;
            var dateTo = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate.Date;

            result = (dateTo - dateTimeInitOrigin).TotalDays;
            return result;
        }
        public FunctionExtension clone()
        {
            return new PeriodoFechaFinFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}