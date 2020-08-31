using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class EmployeeDischargeDateFunction : FunctionExtension
    {
        #region "Attributes"
        private readonly FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public EmployeeDischargeDateFunction(FunctionParams functionParams)
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
            
            if (_functionParams.CalculationBaseResult.Overdraft.Employee.DeleteDate != null)
            {
                var dateTimeInitOrigin = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;
                var dateTo = _functionParams.CalculationBaseResult.Overdraft.Employee.DeleteDate.Value.Date;

                result = (dateTo - dateTimeInitOrigin).Ticks;
            }
            return result;
        }
        public FunctionExtension clone()
        {
            return new EmployeeDischargeDateFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}