using CotorraNode.Common.Library.Private;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.ExtraHours
{
    public class VSalarioHEVigFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public VSalarioHEVigFunction(FunctionParams functionParams)
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

            const int MAX_DOUBLE_PER_WEEK = 9;

            //Salario / horas por turno
            //Horas por turno del empleado
            var workshiftHours = new HorasPorTurnoFunction(_functionParams).calculate();

            var historicEmployeeSalaryAdjustments = _functionParams.CalculationBaseResult.Overdraft.Employee.HistoricEmployeeSalaryAdjustments.
                Where(p => p.ModificationDate >= initialDate && p.ModificationDate <= finalDate);

            if (historicEmployeeSalaryAdjustments.Any())
            {
                //Salario Diario           
                var dailySalaryVig = Convert.ToDouble(_functionParams.CalculationBaseResult.Overdraft.Employee.DailySalary);
                var salaryxHour = (dailySalaryVig / workshiftHours);

                var incidents = _functionParams.CalculationBaseResult.Incidents;

                var incidentHE = incidents.Where(p =>
                        p.IncidentType.TypeOfIncident == TypeOfIncident.Hours &&
                        p.IncidentType.Code == "HE" &&
                        p.Date > historicEmployeeSalaryAdjustments.FirstOrDefault().ModificationDate);

                var initialTempDate = initialDate;
                var finalTempDate = initialTempDate.AddDays(7);

                if (finalTempDate > finalDate)
                {
                    finalTempDate = finalDate;
                }

                while (finalTempDate >= initialDate && finalTempDate <= finalDate &&
                    initialTempDate >= initialDate && initialTempDate <= finalDate)
                {
                    //incidencias de la semana
                    var incidentsFirstRule = incidentHE.Where(p =>
                          p.Date >= initialTempDate &&
                          p.Date <= finalTempDate);

                    //Suma de las incidencias semanales
                    var WeeklyIncidents = incidentsFirstRule.Sum(p => p.Value);

                    if (WeeklyIncidents > MAX_DOUBLE_PER_WEEK)
                    {
                        result += salaryxHour * MAX_DOUBLE_PER_WEEK * 2;
                        result += salaryxHour * (Convert.ToDouble(WeeklyIncidents) - MAX_DOUBLE_PER_WEEK) * 3;
                    }
                    else
                    {
                        result += salaryxHour * Convert.ToDouble(WeeklyIncidents) * 2;
                    }

                    initialTempDate = initialTempDate.AddDays(7 + 1);

                    //regla de poner los dias restantes del periodo                        
                    finalTempDate = finalTempDate.AddDays(7);
                    if (finalTempDate > finalDate)
                    {
                        finalTempDate = finalDate;
                    }
                }
            }
            else
            {
                //Salario Diario           
                var dailySalaryVig = Convert.ToDouble(_functionParams.CalculationBaseResult.Overdraft.Employee.DailySalary);
                var salaryxHour = (dailySalaryVig / workshiftHours);

                var incidents = _functionParams.CalculationBaseResult.Incidents;

                var incidentHE = incidents.Where(p =>
                        p.IncidentType.TypeOfIncident == TypeOfIncident.Hours &&
                        p.IncidentType.Code == "HE");

                var initialTempDate = initialDate;
                var finalTempDate = initialTempDate.AddDays(7);

                if (finalTempDate > finalDate)
                {
                    finalTempDate = finalDate;
                }

                while (finalTempDate >= initialDate && finalTempDate <= finalDate &&
                    initialTempDate >= initialDate && initialTempDate <= finalDate)
                {
                    //incidencias de la semana
                    var incidentsFirstRule = incidentHE.Where(p =>
                          p.Date >= initialTempDate &&
                          p.Date <= finalTempDate);

                    //Suma de las incidencias semanales
                    var WeeklyIncidents = incidentsFirstRule.Sum(p => p.Value);

                    if (WeeklyIncidents > MAX_DOUBLE_PER_WEEK)
                    {
                        result += salaryxHour * MAX_DOUBLE_PER_WEEK * 2;
                        result += salaryxHour * (Convert.ToDouble(WeeklyIncidents) - MAX_DOUBLE_PER_WEEK) * 3;
                    }
                    else
                    {
                        result += salaryxHour * Convert.ToDouble(WeeklyIncidents) * 2;
                    }

                    initialTempDate = initialTempDate.AddDays(7 + 1);

                    //regla de poner los dias restantes del periodo                        
                    finalTempDate = finalTempDate.AddDays(7);
                    if (finalTempDate > finalDate)
                    {
                        finalTempDate = finalDate;
                    }
                }
            }

            return result;
        }
        public FunctionExtension clone()
        {
            return new VSalarioHEVigFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}