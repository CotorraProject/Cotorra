using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Linq;


namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs.Deduction
{
    public class DeduccionPtmoInfonavitCfValorFunction : FunctionExtension
    {
        #region "Attributes"
        private readonly FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public DeduccionPtmoInfonavitCfValorFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }
        #endregion

        public int getParametersNumber()
        {
            return 1;
        }
        public void setParameterValue(int parameterIndex, double parameterValue)
        {
            _x = parameterValue;
        }
        public double calculate()
        {
            //Deducción 16 Ptmo Infonavit cf (Valor)
            var result = 0.0;
            var percetionResult = _functionParams.CalculationBaseResult.Overdraft.OverdraftDetails
                .Where(p =>
                    p.ConceptPayment.Code == 16 &&
                    p.ConceptPayment.ConceptType == ConceptType.DeductionPayment)
                .Sum(p => p.Value);
            result = Convert.ToDouble(percetionResult);
            return result;
        }
        public FunctionExtension clone()
        {
            return new DeduccionPtmoInfonavitCfValorFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}