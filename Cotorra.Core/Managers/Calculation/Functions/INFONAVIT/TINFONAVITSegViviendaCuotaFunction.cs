using MoreLinq;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cotorra.Core.Managers.Calculation.Functions.INFONAVIT
{
    public class TINFONAVITSegViviendaCuotaFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public TINFONAVITSegViviendaCuotaFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
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

            var infonavitInsurances = _functionParams.CalculationBaseResult.InfonavitInsurances;

            if (infonavitInsurances.Any())
            {
                var infonavitInsuranceInPeriod = infonavitInsurances.Where(p => p.ValidityDate <= _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate);
                var infonavitInsuranceTop = infonavitInsuranceInPeriod.MaxBy(p => p.ValidityDate);
                result = Convert.ToDouble(infonavitInsuranceTop.FirstOrDefault().Value);
            }

            return result;
        }
        public FunctionExtension clone()
        {
            return new TINFONAVITSegViviendaCuotaFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}