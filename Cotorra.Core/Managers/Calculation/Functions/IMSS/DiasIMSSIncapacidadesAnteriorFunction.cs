using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.IMSS
{
    public class DiasIMSSIncapacidadesAnteriorFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public DiasIMSSIncapacidadesAnteriorFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }
        public DiasIMSSIncapacidadesAnteriorFunction(FunctionParams functionParams, double x)
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
            var result = 0.0;
            var initialDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.InitialDate;
            var finalDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;

            var historicEmployeeSalaryAdjustments = _functionParams.CalculationBaseResult.Overdraft.Employee.HistoricEmployeeSalaryAdjustments.
                Where(p => p.ModificationDate >= initialDate && p.ModificationDate <= finalDate);

            if (historicEmployeeSalaryAdjustments.Any())
            {
                var modificationDate = historicEmployeeSalaryAdjustments.Select(p => p.ModificationDate);
                if (modificationDate.Any())
                {
                    //Obtiene incidentes sin derecho a sueldo antes de la modificación de salario
                    var inhabilities = _functionParams.CalculationBaseResult.Inhabilities.Where(p =>
                        p.InitialDate < modificationDate.FirstOrDefault());

                    if (inhabilities.Any())
                    {
                        var diff = modificationDate.FirstOrDefault() - inhabilities.FirstOrDefault().InitialDate;
                        var numDays = inhabilities.Where(p => p.IncidentType.TypeOfIncident == TypeOfIncident.Days).Sum(p => p.AuthorizedDays);
                        
                        //calcula el número de horas a descontar por incidencias sin goce de sueldo.
                        result = Convert.ToDouble(numDays);

                        result =  result > diff.TotalDays ? diff.TotalDays : result;
                    }
                }
            }

            return result;
        }

        public FunctionExtension clone()
        {
            return new DiasIMSSIncapacidadesAnteriorFunction(_functionParams, _x);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}