using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.IMSS
{
    public class SalCuotaDiariaIMSSVigFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public SalCuotaDiariaIMSSVigFunction(FunctionParams functionParams)
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
            
        }
        public double calculate()
        {
            var result = Convert.ToDouble(_functionParams.CalculationBaseResult.Overdraft.Employee.DailySalary);
            return result;
        }
        public FunctionExtension clone()
        {
            return new SalCuotaDiariaIMSSVigFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}