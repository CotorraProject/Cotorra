using MoreLinq;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.IMSS
{
    public class Riesgo_trabajoFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public Riesgo_trabajoFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }
        public Riesgo_trabajoFunction(FunctionParams functionParams, double x)
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
            var result = 0.0;
            var iMSSWorkRisks = _functionParams.CalculationBaseResult.IMSSWorkRisks;
            if (iMSSWorkRisks.Any())
            {
                var iMSSWorkRisksTop = iMSSWorkRisks.MaxBy(p => p.ValidityDate);
                result = Convert.ToDouble(iMSSWorkRisksTop.FirstOrDefault().Value);
            }
            return result;
        }
        public FunctionExtension clone()
        {
            return new Riesgo_trabajoFunction(_functionParams, _x);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}