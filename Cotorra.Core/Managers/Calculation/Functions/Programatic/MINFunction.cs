using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.Programatic
{
    public class MINFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x, _y;
        #endregion

        #region "Constructor"
        public MINFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }

        public MINFunction(FunctionParams functionParams, double x, double y)
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
            return _x < _y ? _x : _y;
        }

        public FunctionExtension clone()
        {
            return new MINFunction(_functionParams, _x, _y);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}
