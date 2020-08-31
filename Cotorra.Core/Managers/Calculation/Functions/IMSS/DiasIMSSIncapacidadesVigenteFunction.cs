using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.IMSS
{
    public class DiasIMSSIncapacidadesVigenteFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public DiasIMSSIncapacidadesVigenteFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }
        public DiasIMSSIncapacidadesVigenteFunction(FunctionParams functionParams, double x)
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
                      p.InitialDate >= modificationDate.FirstOrDefault());

                    if (inhabilities.Any())
                    {
                        var diff = 0.0;
                        var inhabilityFinalDay = inhabilities.FirstOrDefault().InitialDate.AddDays(inhabilities.FirstOrDefault().AuthorizedDays);

                        if (inhabilityFinalDay > finalDate)
                        {
                            diff = Math.Abs((modificationDate.FirstOrDefault() - finalDate).TotalDays);
                        }
                        else
                        {
                            diff = Math.Abs((modificationDate.FirstOrDefault() - inhabilityFinalDay).TotalDays);
                        }
                        var hoursPerWorkshift = Convert.ToDecimal(_functionParams.CalculationBaseResult.Overdraft.Employee.Workshift.Hours);
                        var numHours = inhabilities.Where(p => p.IncidentType.TypeOfIncident == TypeOfIncident.Hours).Sum(p => p.AuthorizedDays);
                        var numDays = inhabilities.Where(p => p.IncidentType.TypeOfIncident == TypeOfIncident.Days).Sum(p => p.AuthorizedDays);

                        //calcula el número de horas a descontar por incidencias sin goce de sueldo.
                        result = Convert.ToDouble(numDays + (numHours / hoursPerWorkshift));

                        result = result > diff ? diff : result;
                    }
                }
            }
            else
            {
                var inhabilities = _functionParams.CalculationBaseResult.Inhabilities;

                if (inhabilities.Any())
                {
                    var diff = 0.0;
                    var inhabilityFinalDay = inhabilities.FirstOrDefault().InitialDate.AddDays(inhabilities.FirstOrDefault().AuthorizedDays);
                    var inhabilityInitialDay = inhabilities.FirstOrDefault().InitialDate;

                    if (inhabilityFinalDay > finalDate)
                    {
                        if (inhabilityInitialDay < initialDate)
                        {
                            diff = Math.Abs((initialDate - finalDate).TotalDays) + 1;
                        }
                        else
                        {
                            diff = Math.Abs((inhabilityInitialDay - finalDate).TotalDays) + 1;
                        }
                    }
                    else
                    {
                        diff = Math.Abs((initialDate - inhabilityFinalDay).TotalDays);
                    }
                    var hoursPerWorkshift = Convert.ToDecimal(_functionParams.CalculationBaseResult.Overdraft.Employee.Workshift.Hours);
                    var numHours = inhabilities.Where(p => p.IncidentType.TypeOfIncident == TypeOfIncident.Hours).Sum(p => p.AuthorizedDays);
                    var numDays = inhabilities.Where(p => p.IncidentType.TypeOfIncident == TypeOfIncident.Days).Sum(p => p.AuthorizedDays);

                    //calcula el número de horas a descontar por incidencias sin goce de sueldo.
                    result = Convert.ToDouble(numDays + (numHours / hoursPerWorkshift));

                    result = result > diff ? diff : result;
                }
            }

            return result;
        }

        public FunctionExtension clone()
        {
            return new DiasIMSSIncapacidadesVigenteFunction(_functionParams, _x);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}