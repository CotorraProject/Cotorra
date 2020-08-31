using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class TipoRegimenEmpleadoFunction : FunctionExtension
    {
        #region "Attributes"
        private double _x;
        private readonly FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public TipoRegimenEmpleadoFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }
        public TipoRegimenEmpleadoFunction(FunctionParams functionParams, double x)
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
            var regimeType = (int)_functionParams.CalculationBaseResult.Overdraft.Employee.RegimeType;
            return Convert.ToDouble(regimeType);
        }
        public FunctionExtension clone()
        {
            return new TipoRegimenEmpleadoFunction(_functionParams, _x);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}