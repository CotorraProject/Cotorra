using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions
{
    public class Ajusta_SubsEntregado_MenosFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public Ajusta_SubsEntregado_MenosFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }
        #endregion

        public Ajusta_SubsEntregado_MenosFunction(double x)
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
            return new Ajusta_SubsEntregado_MenosFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}
