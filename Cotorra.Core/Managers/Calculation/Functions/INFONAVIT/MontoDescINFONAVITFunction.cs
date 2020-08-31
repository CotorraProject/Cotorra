using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Cotorra.Core.Managers.Calculation.Functions.INFONAVIT
{
    public class MontoDescINFONAVITFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public MontoDescINFONAVITFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }

        public MontoDescINFONAVITFunction(FunctionParams functionParams, double x)
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
            _x = argumentValue;
        }
        public double calculate()
        {
            var infonavitMovements = _functionParams.CalculationBaseResult.InfonavitMovements;
            var result = 0.0;

            if (infonavitMovements.Any())
            {
                result = Convert.ToDouble(infonavitMovements.Sum(p => p.MonthlyFactor));
            }

            return result;
        }
        public FunctionExtension clone()
        {
            return new MontoDescINFONAVITFunction(_functionParams, _x);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}