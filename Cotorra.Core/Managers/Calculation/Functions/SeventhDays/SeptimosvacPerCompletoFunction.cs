using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions
{
    public class SeptimosvacPerCompletoFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public SeptimosvacPerCompletoFunction(FunctionParams functionParams)
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
           
        }
        public double calculate()
        {
            var seventhDays = _functionParams.CalculationBaseResult.Vacations.Sum(p => p.Break_Seventh_Days);
            return Convert.ToDouble(seventhDays);
        }

        public FunctionExtension clone()
        {
            return new SeptimosvacPerCompletoFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}
