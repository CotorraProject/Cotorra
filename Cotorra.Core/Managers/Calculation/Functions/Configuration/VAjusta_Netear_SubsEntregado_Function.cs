using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions
{
    public class VAjusta_Netear_SubsEntregado_Function : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public VAjusta_Netear_SubsEntregado_Function(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }
        #endregion

        public VAjusta_Netear_SubsEntregado_Function(double x)
        {
        }
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
            return result;
        }
        public FunctionExtension clone()
        {
            return new VAjusta_Netear_SubsEntregado_Function(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}
