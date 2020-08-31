using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.IMSS
{
    public class DiasIMSSAusenciasVigenteFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public DiasIMSSAusenciasVigenteFunction(FunctionParams functionParams)
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
            var initialDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.InitialDate;
            var finalDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;

            var historicEmployeeSalaryAdjustments = _functionParams.CalculationBaseResult.Overdraft.Employee.HistoricEmployeeSalaryAdjustments.
                Where(p => p.ModificationDate >= initialDate && p.ModificationDate <= finalDate);

            if (historicEmployeeSalaryAdjustments.Any())
            {
                var modificationDate = historicEmployeeSalaryAdjustments.Select(p => p.ModificationDate);
                if (modificationDate.Any())
                {
                    //Obtiene incidentes sin derecho a sueldo por incapacidad antes de la modificación de salario
                    var incidents = _functionParams.CalculationBaseResult.Incidents
                        .Where(p =>
                            !p.IncidentType.SalaryRight &&
                            p.IncidentType.ItConsiders == ItConsiders.Absence &&
                            p.Date >= modificationDate.FirstOrDefault());

                    if (incidents.Any())
                    {
                        var numDays = incidents.Where(p => p.IncidentType.TypeOfIncident == TypeOfIncident.Days).Sum(p => p.Value);

                        //calcula el número de horas a descontar por incidencias sin goce de sueldo.
                        result = Convert.ToDouble(numDays);
                    }
                }
            }
            else
            {
                var incidents = _functionParams.CalculationBaseResult.Incidents
                    .Where(p =>
                        p.IncidentType.ItConsiders == ItConsiders.Absence &&
                       !p.IncidentType.SalaryRight);

                if (incidents.Any())
                {
                    var numDays = incidents
                        .Where(p => p.IncidentType.TypeOfIncident == TypeOfIncident.Days)
                        .Sum(p => p.Value);

                    //calcula el número de horas a descontar por incidencias sin goce de sueldo.
                    result = Convert.ToDouble(numDays);
                }
            }

            return result;
        }
        public FunctionExtension clone()
        {
            return new DiasIMSSAusenciasVigenteFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}