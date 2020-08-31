using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Linq;

namespace Cotorra.Core.Managers.Calculation.Functions.PDO
{
    public class PercepcionGenericaTotalFunction : FunctionExtension
    {
        #region "Attributes"
        private readonly FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public PercepcionGenericaTotalFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }
        public PercepcionGenericaTotalFunction(FunctionParams functionParams, double x)
        {
            _functionParams = functionParams;
            _x = x;
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
            var percetionResult = _functionParams.CalculationBaseResult.Overdraft.OverdraftDetails
                .Where(p =>
                    p.ConceptPayment.Code == _x &&
                    p.ConceptPayment.ConceptType == ConceptType.SalaryPayment)
                .Sum(p => p.Amount);
            result = Convert.ToDouble(percetionResult);
            return result;
        }
        public FunctionExtension clone()
        {
            return new PercepcionGenericaTotalFunction(_functionParams, _x);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}