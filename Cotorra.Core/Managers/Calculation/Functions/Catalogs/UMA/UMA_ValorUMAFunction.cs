using MoreLinq;
using Cotorra.Core.Utils;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class UMA_ValorUMAFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public UMA_ValorUMAFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }

        public UMA_ValorUMAFunction(FunctionParams functionParams, double x)
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
            var umas = _functionParams.CalculationBaseResult.UMAs;

            if (umas.Any())
            {
                var umasInPeriod = umas.Where(p => p.ValidityDate <= _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate);
                var umaTop = umasInPeriod.MaxBy(p => p.ValidityDate);
                result = Convert.ToDouble(umaTop.FirstOrDefault().Value);
            }

            return result;
        }
        public FunctionExtension clone()
        {
            return new UMA_ValorUMAFunction(_functionParams, _x);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}