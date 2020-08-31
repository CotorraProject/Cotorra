using MoreLinq;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.ExtraHours
{
    public class HE_ExentasVigenteFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        //_x  horas extras maximas por dia
        //_y horas extras maximas por semana
        private double _x, _y;
        #endregion

        #region "Constructor"
        public HE_ExentasVigenteFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }

        public HE_ExentasVigenteFunction(FunctionParams functionParams, double x, double y)
        {
            _functionParams = functionParams;
            _x = x;
            _y = y;
        }
        #endregion

        public int getParametersNumber()
        {
            return 2;
        }
        public void setParameterValue(int argumentIndex, double argumentValue)
        {
            if (argumentIndex == 0)
            {
                _x = argumentValue;
            }
            else if (argumentIndex == 1)
            {
                _y = argumentValue;
            }
        }
        public double calculate()
        {
            var result = 0.0;

            //_x  horas extras maximas por dia
            //_y horas extras maximas por semana
            var maxHoursPerDay = Convert.ToInt32(_x);
            var maxDaysPerWeek = Convert.ToInt32(_y);

            var initialDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.InitialDate;
            var finalDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;

            //ajustes de sueldo del empleado del periodo
            var historicEmployeeSalaryAdjustments = _functionParams.CalculationBaseResult.Overdraft.Employee.HistoricEmployeeSalaryAdjustments.
                Where(p => p.ModificationDate >= initialDate && p.ModificationDate <= finalDate);

            //Antes de la modificación de salario
            if (historicEmployeeSalaryAdjustments.Any())
            {
                //los incidentes del periodo
                var incidents = _functionParams.CalculationBaseResult.Incidents;

                //Sacar los incidentes de horas extras
                var incidentHE1 = incidents.Where(p =>
                        p.IncidentType.TypeOfIncident == TypeOfIncident.Hours &&
                        (p.IncidentType.Code == "HE")
                        && p.Date >= historicEmployeeSalaryAdjustments.FirstOrDefault().ModificationDate);

                if (incidentHE1.Any())
                {
                    var initialTempDate = initialDate;
                    var finalTempDate = initialTempDate.AddDays(7);
                    var hoursToExempt = 0.0M;

                    //Fix - Solamente considerar el maximo de horas por dia de cada día
                    incidentHE1.ForEach(p =>
                    {
                        if (p.Value > maxHoursPerDay)
                        {
                            p.Value = maxHoursPerDay;
                        }
                    });

                    while (finalTempDate >= initialDate && finalTempDate <= finalDate &&
                           initialTempDate >= initialDate && initialTempDate <= finalDate)
                    {
                        //Obtiene los incidentes de la semana con la condición de _x (maximo numero de horas por semana)
                        //Solamente considerar 3 de 5
                        var incidentsFirstRule = incidentHE1.Where(p =>
                            p.Date >= initialTempDate &&
                            p.Date <= finalTempDate);

                        if (incidentsFirstRule.Any())
                        {
                            incidentsFirstRule = incidentsFirstRule.OrderBy(p => p.Date);
                            //Ordena por valor de mayor a menor y toma los _y (maximo numero de dias por semana)
                            hoursToExempt += incidentsFirstRule.Take(maxDaysPerWeek).Sum(p => p.Value);
                        }

                        initialTempDate = initialTempDate.AddDays(7 + 1);

                        //regla de poner los dias restantes del periodo                        
                        finalTempDate = finalTempDate.AddDays(7);
                        if (finalTempDate > finalDate)
                        {
                            finalTempDate = finalDate;
                        }
                    }

                    result = Convert.ToDouble(hoursToExempt);
                }
            }
            else
            {
                //los incidentes del periodo
                var incidents = _functionParams.CalculationBaseResult.Incidents;

                //Sacar los incidentes de horas extras
                var incidentHE1 = incidents.Where(p =>
                        p.IncidentType.TypeOfIncident == TypeOfIncident.Hours &&
                        (p.IncidentType.Code == "HE"));

                if (incidentHE1.Any())
                {
                    var initialTempDate = initialDate;
                    var finalTempDate = initialTempDate.AddDays(7);
                    var hoursToExempt = 0.0M;

                    while (finalTempDate >= initialDate && finalTempDate <= finalDate &&
                           initialTempDate >= initialDate && initialTempDate <= finalDate)
                    {
                        //Obtiene los incidentes de la semana con la condición de _x (maximo numero de horas por semana)
                        //Solamente considerar 3 de 5
                        var incidentsFirstRule = incidentHE1.Where(p =>
                            p.Date >= initialTempDate &&
                            p.Date <= finalTempDate);

                        //Fix - Solamente considerar el maximo de horas por dia de cada día
                        incidentHE1.ForEach(p =>
                        {
                            if (p.Value > maxHoursPerDay)
                            {
                                p.Value = maxHoursPerDay;
                            }
                        });

                        if (incidentsFirstRule.Any())
                        {
                            incidentsFirstRule = incidentsFirstRule.OrderBy(p => p.Date);
                            //Ordena por valor de mayor a menor y toma los _y (maximo numero de dias por semana)
                            hoursToExempt += incidentsFirstRule.Take(maxDaysPerWeek).Sum(p => p.Value);
                        }

                        initialTempDate = initialTempDate.AddDays(7 + 1);

                        //regla de poner los dias restantes del periodo                        
                        finalTempDate = finalTempDate.AddDays(7);
                        if (finalTempDate > finalDate)
                        {
                            finalTempDate = finalDate;
                        }
                    }

                    result = Convert.ToDouble(hoursToExempt);
                }
            }
            return result;
        }

        public FunctionExtension clone()
        {
            return new HE_ExentasVigenteFunction(_functionParams, _x, _y);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}