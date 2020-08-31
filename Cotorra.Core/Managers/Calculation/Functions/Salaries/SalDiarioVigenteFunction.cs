using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Cotorra.Core.Managers.Calculation.Functions
{
    public class SalDiarioVigenteFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public SalDiarioVigenteFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }
        #endregion

        public int getParametersNumber()
        {
            return 0;
        }
        public void setParameterValue(int argumentIndex, double argumentValue)
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
            return new SalDiarioVigenteFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}