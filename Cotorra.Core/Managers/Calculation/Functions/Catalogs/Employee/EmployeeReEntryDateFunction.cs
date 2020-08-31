using Cotorra.Core.Utils;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class EmployeeReEntryDateFunction : FunctionExtension
    {
        #region "Attributes"
        private readonly FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public EmployeeReEntryDateFunction(FunctionParams functionParams)
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

            if (_functionParams.CalculationBaseResult.Overdraft.Employee.ReEntryDate != null)
            {
                var dateTimeInitOrigin = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;
                var dateTo = _functionParams.CalculationBaseResult.Overdraft.Employee.ReEntryDate.Value.Date;

                result = (dateTo - dateTimeInitOrigin).TotalDays;
            }
            return result;
        }
        public FunctionExtension clone()
        {
            return new EmployeeReEntryDateFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}