using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class AcumuladoMesSubsEmpleoAcreditadoFunction : AccumulatedBase, FunctionExtension
    {
        #region "Attributes"
        private readonly FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public AcumuladoMesSubsEmpleoAcreditadoFunction(FunctionParams functionParams)
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
            //Subs al Empleo  Acreditado|P|38
            var accumulatedCode = 38;
            var accumulated = _functionParams.CalculationBaseResult.AccumulatedEmployees
                .Where(p => p.AccumulatedType.Code == accumulatedCode);
            var historicAccumulated = _functionParams.CalculationBaseResult.HistoricAccumulatedEmployees
             .Where(p => p.AccumulatedType.Code == accumulatedCode);

            var result = CalculateAccumulated(periodFinalDate, accumulated, historicAccumulated, true);
            return Convert.ToDouble(result);
        }
        public FunctionExtension clone()
        {
            return new AcumuladoMesSubsEmpleoAcreditadoFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}