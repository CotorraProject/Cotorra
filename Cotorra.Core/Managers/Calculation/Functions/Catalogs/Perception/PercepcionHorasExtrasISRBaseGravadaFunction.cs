using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Linq;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class PercepcionHorasExtrasISRBaseGravadaFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public PercepcionHorasExtrasISRBaseGravadaFunction(FunctionParams functionParams)
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
            //Percepción 4 Horas Extras base gravada ISR
            var result = 0.0;
            var percetionResult = _functionParams.CalculationBaseResult.Overdraft.OverdraftDetails
                .Where(p=>p.ConceptPayment.Code == 4 && p.ConceptPayment.ConceptType == ConceptType.SalaryPayment)
                .Sum(p=>p.Taxed);
            result = Convert.ToDouble(percetionResult);
            return result;
        }
        public FunctionExtension clone()
        {
            return new PercepcionHorasExtrasISRBaseGravadaFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}