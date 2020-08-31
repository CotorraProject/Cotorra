using MoreLinq;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class TVigSubEmpMensual_Subs_al_empleoFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public TVigSubEmpMensual_Subs_al_empleoFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }

        public TVigSubEmpMensual_Subs_al_empleoFunction(FunctionParams functionParams, double x)
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
            var monthlyEmploymentSubsidies = _functionParams.CalculationBaseResult.MonthlyEmploymentSubsidies;
            if (monthlyEmploymentSubsidies.Any())
            {
                var monthlyEmploymentSubsidiesTop = monthlyEmploymentSubsidies.MaxBy(p => p.ValidityDate);
                var amount = Convert.ToDecimal(_x);
                var lowerLimit = monthlyEmploymentSubsidiesTop.MinBy(p => p.LowerLimit).FirstOrDefault().LowerLimit;
                if (amount >= lowerLimit)
                {
                    result = Convert.ToDouble(monthlyEmploymentSubsidiesTop.OrderBy(p => p.LowerLimit)
                    .LastOrDefault(p => p.LowerLimit < amount).MonthlySubsidy);
                }
            }

            return result;
        }
        public FunctionExtension clone()
        {
            return new TVigSubEmpMensual_Subs_al_empleoFunction(_functionParams, _x);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}