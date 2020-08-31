using MoreLinq;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class SalariosMinimos_ZonaC : FunctionExtension
    {
        #region "Attributes"
        private readonly FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public SalariosMinimos_ZonaC(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }

        public SalariosMinimos_ZonaC(FunctionParams functionParams, double x)
        {
            _functionParams = functionParams;
            _x = x;
        }
        #endregion

        public int getParametersNumber()
        {
            return 1;
        }
        public void setParameterValue(int parameterIndex, double parameterValue)
        {
            _x = parameterValue;
        }
        public double calculate()
        {
            var result = 0.0;
            var minimunSalaries = _functionParams.CalculationBaseResult.MinimunSalaries;
            if (minimunSalaries.Any())
            {
                var periodFinalDay = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;
                var minimunSalary = minimunSalaries.Where(p => p.ExpirationDate < periodFinalDay).MaxBy(p => p.ExpirationDate);

                if (minimunSalary != null)
                {
                    result = Convert.ToDouble(minimunSalary.FirstOrDefault().ZoneC);
                }
            }

            return result;
        }
        public FunctionExtension clone()
        {
            return new SalariosMinimos_ZonaC(_functionParams, _x);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}