using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.Programatic
{
    public class RoundToFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x, _y;
        #endregion

        #region "Constructor"
        public RoundToFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }

        public RoundToFunction(FunctionParams functionParams, double x, double y)
        {
            _functionParams = functionParams;
            _x = x;
            _y = y;
        }
        #endregion

        public int getParametersNumber()
        {
            return 2;
        }
        public void setParameterValue(int argumentIndex, double argumentValue)
        {
            if (argumentIndex == 0)
            {
                _x = argumentValue;
            }
            else if (argumentIndex == 1)
            {
                _y = argumentValue;
            }
        }
        public double calculate()
        {
            return Math.Round(_x, (int)_y);
        }

        public FunctionExtension clone()
        {
            return new RoundToFunction(_functionParams, _x, _y);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}