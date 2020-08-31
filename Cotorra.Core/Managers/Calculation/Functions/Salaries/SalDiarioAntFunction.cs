using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Cotorra.Core.Managers.Calculation.Functions
{
    public class SalDiarioAntFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public SalDiarioAntFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }

        public SalDiarioAntFunction(double x)
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
            var initialDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.InitialDate;
            var finalDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;
            var result = 0.0;

            var historicEmployeeSalaryAdjustments = _functionParams.CalculationBaseResult.Overdraft.Employee.HistoricEmployeeSalaryAdjustments.
                Where(p=> p.ModificationDate >= initialDate && p.ModificationDate <= finalDate);

            if (historicEmployeeSalaryAdjustments.Any())
            {
                var decimalResult = 0.0M;
                //Si es de finiquito se toma otro salario diario
                if (_functionParams.CalculationBaseResult.Overdraft.OverdraftType == OverdraftType.CompensationSettlement ||
                    _functionParams.CalculationBaseResult.Overdraft.OverdraftType == OverdraftType.OrdinarySettlement)
                {
                    result = Convert.ToDouble(_functionParams.CalculationBaseResult.Overdraft.Employee.SettlementSalaryBase);
                }
                else
                {
                    decimalResult = historicEmployeeSalaryAdjustments.FirstOrDefault().DailySalary;
                }
                
                result = Convert.ToDouble(decimalResult);
            }

            return result;
        }
        public FunctionExtension clone()
        {
            return new SalDiarioAntFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}