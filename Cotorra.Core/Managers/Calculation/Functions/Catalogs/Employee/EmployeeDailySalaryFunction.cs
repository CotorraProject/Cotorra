using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class EmployeeDailySalaryFunction : FunctionExtension
    {
        #region "Attributes"
        private readonly FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public EmployeeDailySalaryFunction(FunctionParams functionParams)
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
            throw new NotImplementedException();
        }
        public double calculate()
        {
            var result = 0.0;
            //Si es de finiquito se toma otro salario diario
            if (_functionParams.CalculationBaseResult.Overdraft.OverdraftType == OverdraftType.CompensationSettlement ||
                _functionParams.CalculationBaseResult.Overdraft.OverdraftType == OverdraftType.OrdinarySettlement)
            {
                result = Convert.ToDouble(_functionParams.CalculationBaseResult.Overdraft.Employee.SettlementSalaryBase);
            }
            else
            {
                result = Convert.ToDouble(_functionParams.CalculationBaseResult.Overdraft.Employee.DailySalary);
            }
            return result;
        }
        public FunctionExtension clone()
        {
            return new EmployeeDailySalaryFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}