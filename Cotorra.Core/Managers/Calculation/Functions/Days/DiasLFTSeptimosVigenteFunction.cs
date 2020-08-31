using Cotorra.Core.Utils;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions
{
    public class DiasLFTSeptimosVigenteFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public DiasLFTSeptimosVigenteFunction(FunctionParams functionParams)
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
            var result = 0.0;
            var periodInitialDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.InitialDate;
            var periodFinalDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;
            var seventDayPosition = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.SeventhDayPosition;

            var historicEmployeeSalaryAdjustments = _functionParams.CalculationBaseResult.Overdraft.Employee.HistoricEmployeeSalaryAdjustments.
                Where(p => p.ModificationDate >= periodInitialDate && p.ModificationDate <= periodFinalDate);

            if (historicEmployeeSalaryAdjustments.Any())
            {
                var modificationDate = historicEmployeeSalaryAdjustments.Select(p => p.ModificationDate);
                if (modificationDate.Any() && !String.IsNullOrEmpty(seventDayPosition))
                {
                    var modificationDateSelected = modificationDate.LastOrDefault();
                    string[] splitted = null;

                    if (seventDayPosition.Contains(","))
                    {
                        splitted = seventDayPosition.Split(",");
                    }
                    else
                    {
                        splitted = new string[] { seventDayPosition };
                    }
                    foreach (var split in splitted)
                    {
                        int splitInt = Int32.Parse(split);
                        if (splitInt != -1)
                        {
                            result += DateTimeUtil.CountDays((DayOfWeek)splitInt, modificationDateSelected, periodFinalDate);
                        }
                    }
                }
            }
            else
            {
                string[] splitted = null;

                if (seventDayPosition.Contains(","))
                {
                    splitted = seventDayPosition.Split(",");
                }
                else
                {
                    splitted = new string[] { seventDayPosition };
                }
                foreach (var split in splitted)
                {
                    int splitInt = Int32.Parse(split);
                    if (splitInt != -1)
                    {
                        result += DateTimeUtil.CountDays((DayOfWeek)splitInt, periodInitialDate, periodFinalDate);
                    }
                }
            }

            return result;
        }
        public FunctionExtension clone()
        {
            return new DiasLFTSeptimosVigenteFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}