using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions
{
    public class IncidenciaSinDerechoASueldoFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public IncidenciaSinDerechoASueldoFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }

        public IncidenciaSinDerechoASueldoFunction(double x)
        {
            _x = x;
        }

        public IncidenciaSinDerechoASueldoFunction(FunctionParams functionParams, double x)
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

        private double calculatePrevious()
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
                    var incidents = _functionParams.CalculationBaseResult.Incidents.Where(p =>
                        !p.IncidentType.SalaryRight &&
                        p.IncidentType.Code != "HE" &&
                        p.IncidentType.Code != "RET " &&
                        p.Date < modificationDate.FirstOrDefault());

                    if (incidents.Any())
                    {
                        var numHours = incidents.Where(p => p.IncidentType.TypeOfIncident == TypeOfIncident.Hours).Sum(p => p.Value);
                        result = Convert.ToDouble(numHours);
                    }
                }
            }

            return result;
        }

        private double calculateActual()
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
                    var incidents = _functionParams.CalculationBaseResult.Incidents.Where(p =>
                        !p.IncidentType.SalaryRight &&
                        p.IncidentType.Code != "HE" &&
                         p.IncidentType.Code != "RET " &&
                        p.Date >= modificationDate.FirstOrDefault());

                    if (incidents.Any())
                    {
                        var numHours = incidents.Where(p => p.IncidentType.TypeOfIncident == TypeOfIncident.Hours).Sum(p => p.Value);
                        result = Convert.ToDouble(numHours);
                    }
                }
            }
            else
            {
                var incidents = _functionParams.CalculationBaseResult.Incidents.Where(p =>
                       !p.IncidentType.SalaryRight &&
                        p.IncidentType.Code != "HE" &&
                        p.IncidentType.Code != "RET ");

                if (incidents.Any())
                {
                    var numHours = incidents.Where(p => p.IncidentType.TypeOfIncident == TypeOfIncident.Hours).Sum(p => p.Value);
                    result = Convert.ToDouble(numHours);
                }
            }

            return result;
        }

        public double calculate()
        {
            var result = 0.0;

            if (_x == 0)
            {
                result = calculatePrevious();
            }
            else
            {
                result = calculateActual();
            }

            return result;
        }

        public FunctionExtension clone()
        {
            return new IncidenciaSinDerechoASueldoFunction(_functionParams, _x);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}