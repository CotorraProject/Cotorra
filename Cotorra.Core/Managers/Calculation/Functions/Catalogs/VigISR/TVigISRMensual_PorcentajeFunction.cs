using MoreLinq;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class TVigISRMensual_PorcentajeFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public TVigISRMensual_PorcentajeFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }

        public TVigISRMensual_PorcentajeFunction(FunctionParams functionParams, double x)
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
            var monthlyIncomeTaxes = _functionParams.CalculationBaseResult.MonthlyIncomeTaxes;
            if (monthlyIncomeTaxes.Any())
            {
                var monthlyIncomeTaxesTop = monthlyIncomeTaxes.MaxBy(p => p.ValidityDate);
                var amount = Convert.ToDecimal(_x);
                var lowerLimit = monthlyIncomeTaxesTop.MinBy(p => p.LowerLimit).FirstOrDefault().LowerLimit;
                if (amount >= lowerLimit)
                {
                    result = Convert.ToDouble(monthlyIncomeTaxesTop.OrderBy(p => p.LowerLimit).LastOrDefault(p => p.LowerLimit < amount)
                    .Rate);
                }
            }

            return result;
        }
        public FunctionExtension clone()
        {
            return new TVigISRMensual_PorcentajeFunction(_functionParams, _x);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}