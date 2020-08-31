using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class EmployeeIMSSSBCTypeFunction : FunctionExtension
    {
        #region "Attributes"
        private readonly FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public EmployeeIMSSSBCTypeFunction(FunctionParams functionParams)
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
            var result = 0.0;
            var contributionBase = _functionParams.CalculationBaseResult.Overdraft.Employee.ContributionBase;
            if (contributionBase == BaseQuotation.Fixed)
            {
                //(ASCII('F')
                result = 70;
            }
            else if (contributionBase == BaseQuotation.Mixed)
            {
                //(ASCII('M')
                result = 77;
            }
            else
            {
                //(ASCII('V')
                result = 86;
            }
            return Convert.ToDouble(result);
        }
        public FunctionExtension clone()
        {
            return new EmployeeIMSSSBCTypeFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}