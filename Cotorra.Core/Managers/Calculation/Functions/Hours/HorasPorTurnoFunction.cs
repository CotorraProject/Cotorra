using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions
{
    public class HorasPorTurnoFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public HorasPorTurnoFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }

        public HorasPorTurnoFunction(double x)
        {
        }
        #endregion

        public int getParametersNumber()
        {
            return 1;
        }
        public void setParameterValue(int argumentIndex, double argumentValue)
        {

        }
        public double calculate()
        {
            var result = _functionParams.CalculationBaseResult.Overdraft.Employee.Workshift.Hours;
            return Convert.ToDouble(result);
        }
        public FunctionExtension clone()
        {
            return new HorasPorTurnoFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}
