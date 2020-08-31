using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class AcumuladoMesISRPercEspecialesGravFunction : AccumulatedBase, FunctionExtension
    {
        #region "Attributes"
        private readonly FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public AcumuladoMesISRPercEspecialesGravFunction(FunctionParams functionParams)
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
            //ISR Perc.especiales grav.|P|23 
            var accumulatedCode = 23;
            var accumulated = _functionParams.CalculationBaseResult.AccumulatedEmployees
                .Where(p => p.AccumulatedType.Code == accumulatedCode);
            var historicAccumulated = _functionParams.CalculationBaseResult.HistoricAccumulatedEmployees
             .Where(p => p.AccumulatedType.Code == accumulatedCode);

            var result = CalculateAccumulated(periodFinalDate, accumulated, historicAccumulated, true);
            return Convert.ToDouble(result);
        }
        public FunctionExtension clone()
        {
            return new AcumuladoMesISRPercEspecialesGravFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}