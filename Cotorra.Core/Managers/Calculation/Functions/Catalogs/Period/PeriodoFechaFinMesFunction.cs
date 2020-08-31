using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class PeriodoFechaFinMesFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public PeriodoFechaFinMesFunction(FunctionParams functionParams)
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
            var isFinal = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.PeriodMonth == PeriodMonth.Final
                        || _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.PeriodMonth == PeriodMonth.Both;
            return isFinal ? 1 : 0;
        }
        public FunctionExtension clone()
        {
            return new PeriodoFechaFinMesFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}