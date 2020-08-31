using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;

namespace Cotorra.Core.Managers.Calculation.Functions
{
    public class DiasLFTSalarioVigenteFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public DiasLFTSalarioVigenteFunction(FunctionParams functionParams)
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
            _x = argumentValue;
        }

        public double calculate()
        {
            var initialDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.InitialDate;
            var finalDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;
            var result = 0.0;

            var entryDate = _functionParams.CalculationBaseResult.Overdraft.Employee.EntryDate;
            var entryDays = 0.0;
            if (entryDate >= initialDate && entryDate <= finalDate)
            {
                entryDays = (entryDate - initialDate).TotalDays;
            }

            var historicEmployeeSalaryAdjustments = _functionParams.CalculationBaseResult.Overdraft.Employee.HistoricEmployeeSalaryAdjustments.
                Where(p => p.ModificationDate >= initialDate && p.ModificationDate <= finalDate);

            if (historicEmployeeSalaryAdjustments.Any())
            {
                var modificationDate = historicEmployeeSalaryAdjustments.Select(p => p.ModificationDate);
                if (modificationDate.Any())
                {
                    result = (finalDate - modificationDate.FirstOrDefault()).TotalDays + 1 - entryDays;
                }
            }
            else
            {
                if (_functionParams.CalculationBaseResult.Overdraft.PeriodDetail.Period.PaymentPeriodicity == PaymentPeriodicity.Weekly)
                {
                    result = Convert.ToDouble((finalDate - initialDate).TotalDays + 1) - entryDays;
                }
                else
                {
                    //Días del periodo
                    result = Convert.ToDouble(_functionParams.CalculationBaseResult.Overdraft.PeriodDetail.PaymentDays) - entryDays;
                }
            }

            return result;
        }
        public FunctionExtension clone()
        {
            return new DiasLFTSalarioVigenteFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}