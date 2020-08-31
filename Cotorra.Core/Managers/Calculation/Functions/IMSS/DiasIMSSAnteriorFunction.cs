using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.IMSS
{
    public class DiasIMSSAnteriorFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public DiasIMSSAnteriorFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }
        public DiasIMSSAnteriorFunction(FunctionParams functionParams, double x)
        {
            _functionParams = functionParams;
            _x = x;
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
            /*Esta función determina  el total de días que el empleado cotizó para efectos del IMSS,  
             * con un salario  anterior al vigente. Y únicamente va a aplicar para aquellos empleados 
             * que hayan tenido alguna  modificación de salarios  en el periodo de nómina vigente. 
             * NOTA.- En estos días no se descuentan  incapacidades ni ausentismos.*/
            var initialDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.InitialDate;
            var finalDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;
            var result = 0.0;

            //Dias de entrada descontados
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
                    result = (modificationDate.FirstOrDefault() - initialDate).TotalDays - entryDays;
                }
            }

            return result;
        }
        public FunctionExtension clone()
        {
            return new DiasIMSSAnteriorFunction(_functionParams, _x);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}