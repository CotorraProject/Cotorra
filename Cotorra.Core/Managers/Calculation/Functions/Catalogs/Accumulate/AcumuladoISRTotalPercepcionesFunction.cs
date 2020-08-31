using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class AcumuladoISRTotalPercepcionesFunction : AccumulatedBase, FunctionExtension
    {
        #region "Attributes"
        private readonly FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public AcumuladoISRTotalPercepcionesFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }
        #endregion

        public int getParametersNumber()
        {
            return 0;
        }
        public void setParameterValue(int argumentIndex, double argumentValue)
        {
        }
        public double calculate()
        {
            var periodFinalDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;

            //ISR Total de percepciones|P|1 
            var accumulatedCode = 1;
            var accumulated = _functionParams.CalculationBaseResult.AccumulatedEmployees
                .Where(p => p.AccumulatedType.Code == accumulatedCode);
            var historicAccumulated = _functionParams.CalculationBaseResult.HistoricAccumulatedEmployees
              .Where(p => p.AccumulatedType.Code == accumulatedCode);

            var result = CalculateAccumulated(periodFinalDate, accumulated, historicAccumulated);
            return Convert.ToDouble(result);
        }
        public FunctionExtension clone()
        {
            return new AcumuladoISRTotalPercepcionesFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}