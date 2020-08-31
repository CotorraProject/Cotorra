using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.Programatic
{
    public class FRACFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public FRACFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }

        public FRACFunction(FunctionParams functionParams, double x)
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
            if (argumentIndex == 0)
            {
                _x = argumentValue;
            }
        }
        public double calculate()
        {
            var result = _x - Math.Truncate(_x);
            return result;
        }

        public FunctionExtension clone()
        {
            return new FRACFunction(_functionParams, _x);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}