using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class AcumuladoISRLiquidacionExentoFunction : AccumulatedBase, FunctionExtension
    {
        #region "Attributes"
        private readonly FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public AcumuladoISRLiquidacionExentoFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }
        #endregion

        public int getParametersNumber()
        {
            return 0;
        }
        public void setParameterValue(int parameterIndex, double parameterValue)
        {
        }
        public double calculate()
        {
            var periodFinalDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;

            //ISR Liquidaciones Exento|P|10
            var accumulatedCode = 10;
            var accumulated = _functionParams.CalculationBaseResult.AccumulatedEmployees
                .Where(p => p.AccumulatedType.Code == accumulatedCode);
            var historicAccumulated = _functionParams.CalculationBaseResult.HistoricAccumulatedEmployees
              .Where(p => p.AccumulatedType.Code == accumulatedCode);

            var result = CalculateAccumulated(periodFinalDate, accumulated, historicAccumulated);
            return Convert.ToDouble(result);
        }
        public FunctionExtension clone()
        {
            return new AcumuladoISRLiquidacionExentoFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}