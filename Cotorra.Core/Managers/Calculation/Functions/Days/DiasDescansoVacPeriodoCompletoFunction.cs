using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions
{
    public class DiasDescansoVacPeriodoCompletoFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public DiasDescansoVacPeriodoCompletoFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }
        #endregion

        public DiasDescansoVacPeriodoCompletoFunction(double x)
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
            //Dias de descanso de las vacaciones
            var result = 0.0;
            return result;
        }
        public FunctionExtension clone()
        {
            return new DiasDescansoVacPeriodoCompletoFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}
