using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions
{
    public class DiasTranscurridosMesFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public DiasTranscurridosMesFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }
        #endregion

        public DiasTranscurridosMesFunction(double x)
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
            //Dias transcurridos en el mes
            var totalDays = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate.Day;
            return Convert.ToDouble(totalDays);
        }
        public FunctionExtension clone()
        {
            return new DiasTranscurridosMesFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}
