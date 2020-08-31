using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.ExpressionParser.Core.Cotorra
{

    public class AlwaysReturn1Function : FunctionExtension
    {
        double x;
        double y;
        public AlwaysReturn1Function()
        {
            x = Double.NaN;
            y = Double.NaN;
        }
        public AlwaysReturn1Function(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public int getParametersNumber()
        {
            return 2;
        }
        public void setParameterValue(int argumentIndex, double argumentValue)
        {
            if (argumentIndex == 0) x = argumentValue;
            if (argumentIndex == 1) y = argumentValue;
        }
        public double calculate()
        {
            return 1;
        }
        public FunctionExtension clone()
        {
            return new AlwaysReturn1Function(x, y);
        }

        public string getParameterName(int parameterIndex)
        {
            if (parameterIndex == 0)
            {
                return "x";
            }
            else
            { return "y"; }
        }
    }
}
