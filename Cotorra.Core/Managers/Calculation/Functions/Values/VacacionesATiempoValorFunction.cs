using CotorraNode.Common.Library.Private;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.Values
{
    public class VacacionesATiempoValorFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public VacacionesATiempoValorFunction(FunctionParams functionParams)
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
            result = Convert.ToDouble(_functionParams.CalculationBaseResult.Vacations.Sum(p => p.VacationsDays));
            return result;
        }
        public FunctionExtension clone()
        {
            return new VacacionesATiempoValorFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}