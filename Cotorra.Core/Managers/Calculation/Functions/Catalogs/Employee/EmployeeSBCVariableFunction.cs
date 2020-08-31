using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class EmployeeSBCVariableFunction : FunctionExtension
    {
        #region "Attributes"
        private readonly FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public EmployeeSBCVariableFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }
        #endregion

        public int getParametersNumber()
        {
            return 0;
        }
        public void setParameterValue(int parameterIndex, double parameterValue)
        {
        }
        public double calculate()
        {
            var sbcVariablePart = _functionParams.CalculationBaseResult.Overdraft.Employee.SBCVariablePart;
            return Convert.ToDouble(sbcVariablePart);
        }
        public FunctionExtension clone()
        {
            return new EmployeeSBCVariableFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}