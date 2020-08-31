using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Cotorra.Core.Managers.Calculation.Functions.Settlement
{
    public class AntiguedadEmpleadoFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public AntiguedadEmpleadoFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }

        public AntiguedadEmpleadoFunction(double x)
        { }
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
            var result = 0.0;
            var entryDate = _functionParams.CalculationBaseResult.Overdraft.Employee.EntryDate;
            var finalPeriodDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;
            //difference between entry date and final period date / days per year
            result = (finalPeriodDate - entryDate).TotalDays / 365;

            return result;
        }
        public FunctionExtension clone()
        {
            return new AntiguedadEmpleadoFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}