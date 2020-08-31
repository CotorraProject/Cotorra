using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Cotorra.Core.Managers.Calculation.Functions
{
    public class Vac_Ult_Periodo_CompletoFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public Vac_Ult_Periodo_CompletoFunction(FunctionParams functionParams)
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
            var result = 0.0;

            return Convert.ToDouble(result);
        }
        public FunctionExtension clone()
        {
            return new Vac_Ult_Periodo_CompletoFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}
