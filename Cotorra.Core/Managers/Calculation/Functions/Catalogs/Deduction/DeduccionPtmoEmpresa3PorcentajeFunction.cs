using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Linq;


namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs.Deduction
{
    public class DeduccionPtmoEmpresa3PorcentajeFunction : FunctionExtension
    {
        #region "Attributes"
        private readonly FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public DeduccionPtmoEmpresa3PorcentajeFunction(FunctionParams functionParams)
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
            //Deducción 165 Ptmo Empresa 3 (Valor)
            var result = 0.0;
            var percetionResult = _functionParams.CalculationBaseResult.Overdraft.OverdraftDetails
                .Where(p =>
                    p.ConceptPayment.Code == 165 &&
                    p.ConceptPayment.ConceptType == ConceptType.DeductionPayment)
                .Sum(p => p.Value);
            result = Convert.ToDouble(percetionResult);
            return result;
        }
        public FunctionExtension clone()
        {
            return new DeduccionPtmoEmpresa3PorcentajeFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}