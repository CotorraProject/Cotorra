using Cotorra.Core.Utils;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cotorra.Core.Managers.Calculation.Functions
{
    public class Vac_Sept_Primer_PeriodoFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public Vac_Sept_Primer_PeriodoFunction(FunctionParams functionParams)
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

        /// <summary>
        /// Días de vacaciones que incluyen séptimos días
        /// </summary>
        /// <returns></returns>
        public double calculate()
        {
            var result = 0.0;
            var seventDayPosition = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.SeventhDayPosition;
            //vacaciones del periodo
            var vacations = _functionParams.CalculationBaseResult.Vacations;

            if (vacations.Any())
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

                foreach (var vacation in vacations)
                {
                    foreach (var split in splitted)
                    {
                        int splitInt = Int32.Parse(split);
                        if (splitInt != -1)
                        {
                            result += DateTimeUtil.CountDays((DayOfWeek)splitInt, vacation.InitialDate, vacation.FinalDate);
                        }
                    }
                }
            }

            return result;
        }
        public FunctionExtension clone()
        {
            return new Vac_Sept_Primer_PeriodoFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}
