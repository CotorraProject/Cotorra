using MoreLinq;
using Cotorra.Core.Utils;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.INFONAVIT
{
    public class DiasInfonavitPorcentajeVigenteFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public DiasInfonavitPorcentajeVigenteFunction(FunctionParams functionParams)
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

            //Cambios de SBC del periodo
            var historicEmployeeSalaryAdjustments = _functionParams
                .CalculationBaseResult
                .Overdraft
                .Employee
                .HistoricEmployeeSBCAdjustments
                .Where(p => p.ModificationDate >= initialDate && p.ModificationDate <= finalDate);

            //fecha de aplicacion infonavit (si está dentro del periodo esa es mi fecha de inicio)
            var infonavitApplicationDate = _functionParams.CalculationBaseResult.InfonavitMovements.FirstOrDefault().InitialApplicationDate;
            if (infonavitApplicationDate >= initialDate && infonavitApplicationDate <= finalDate)
            {
                initialDate = infonavitApplicationDate;
            }

            var entryDate = _functionParams.CalculationBaseResult.Overdraft.Employee.EntryDate;
            var entryDays = 0.0;
            if (entryDate >= initialDate && entryDate <= finalDate)
            {
                entryDays = (entryDate - initialDate).TotalDays;
            }

            if (historicEmployeeSalaryAdjustments.Any())
            {
                //días hasta el ajuste de salario
                var modificationDate = historicEmployeeSalaryAdjustments
                    .Select(p => p.ModificationDate);

                if (modificationDate.Any())
                {
                    result = (finalDate - modificationDate.FirstOrDefault()).TotalDays + 1;

                    //Obtiene incidentes sin derecho a sueldo antes de la modificación de salario
                    var incidents = _functionParams.CalculationBaseResult.Incidents.Where(p =>
                        !p.IncidentType.SalaryRight &&
                        p.IncidentType.TypeOfIncident == TypeOfIncident.Days &&
                        p.IncidentType.Code != "HE" &&
                        p.Date >= modificationDate.FirstOrDefault());

                    if (incidents.Any())
                    {
                        var hoursPerWorkshift = Convert.ToDecimal(_functionParams.CalculationBaseResult.Overdraft.Employee.Workshift.Hours);
                        var numHours = incidents.Where(p => p.IncidentType.TypeOfIncident == TypeOfIncident.Hours).Sum(p => p.Value);
                        var numDays = incidents.Where(p => p.IncidentType.TypeOfIncident == TypeOfIncident.Days).Sum(p => p.Value);

                        //calcula el número de horas a descontar por incidencias sin goce de sueldo.
                        result -= Convert.ToDouble(numDays + (numHours / hoursPerWorkshift));
                    }

                    //Incapacidades
                    var inhabilities = _functionParams.CalculationBaseResult.Inhabilities;
                    var inhabilityTotalDays = 0.0;
                    inhabilities.ForEach(inha =>
                    {
                        inhabilityTotalDays += new DateTimeUtil().InclusiveDays(
                            modificationDate.FirstOrDefault(), inha.FinalDate,
                            initialDate, finalDate
                        );
                    });

                    result -= inhabilityTotalDays;

                    //vacaciones - si inician este periodo y finalizan el que sigue
                    var lstVacations = _functionParams.CalculationBaseResult.Vacations.Where(p => p.FinalDate > finalDate);
                    if (lstVacations.Any())
                    {
                        lstVacations.ForEach(p => {
                            result += (p.FinalDate - finalDate).TotalDays + 1;
                        });
                    }

                    //si iniciarion el periodo pasado
                    var lstVacationsInPreviousPeriod = _functionParams.CalculationBaseResult.Vacations.Where(p => p.InitialDate < initialDate);
                    if (lstVacationsInPreviousPeriod.Any())
                    {
                        lstVacationsInPreviousPeriod.ForEach(p => {
                            result -= (p.FinalDate - initialDate).TotalDays + 1;
                        });
                    }
                }

            }
            else
            {
                result = (finalDate - initialDate).TotalDays - entryDays + 1;

                //Incidencias
                var incidents = _functionParams.CalculationBaseResult.Incidents.Where(p =>
                       !p.IncidentType.SalaryRight &&
                       p.IncidentType.TypeOfIncident == TypeOfIncident.Days &&
                        p.IncidentType.Code != "HE");

                if (incidents.Any())
                {
                    var hoursPerWorkshift = Convert.ToDecimal(_functionParams.CalculationBaseResult.Overdraft.Employee.Workshift.Hours);
                    var numHours = incidents.Where(p => p.IncidentType.TypeOfIncident == TypeOfIncident.Hours).Sum(p => p.Value);
                    var numDays = incidents.Where(p => p.IncidentType.TypeOfIncident == TypeOfIncident.Days).Sum(p => p.Value);

                    //calcula el número de horas a descontar por incidencias sin goce de sueldo.
                    result -= Convert.ToDouble(numDays + (numHours / hoursPerWorkshift));
                }

                //Incapacidades
                var inhabilities = _functionParams.CalculationBaseResult.Inhabilities;
                var inhabilityTotalDays = 0.0;
                inhabilities.ForEach(inha =>
                {
                    inhabilityTotalDays +=
                        new DateTimeUtil().InclusiveDays(
                            inha.InitialDate, inha.FinalDate,
                            initialDate, finalDate
                        );
                });

                result -= inhabilityTotalDays;

                //vacaciones - si inician este periodo y finalizan el que sigue
                var lstVacations = _functionParams.CalculationBaseResult.Vacations.Where(p => p.FinalDate > finalDate);
                if (lstVacations.Any())
                {
                    lstVacations.ForEach(p => {
                        result += (p.FinalDate - finalDate).TotalDays + 1;
                    });
                }

                //si iniciarion el periodo pasado
                var lstVacationsInPreviousPeriod = _functionParams.CalculationBaseResult.Vacations.Where(p => p.InitialDate < initialDate);
                if (lstVacationsInPreviousPeriod.Any())
                {
                    lstVacationsInPreviousPeriod.ForEach(p => {
                        result -= (p.FinalDate - initialDate).TotalDays + 1;
                    });
                }
            }
            return result;
        }
        public FunctionExtension clone()
        {
            return new DiasInfonavitPorcentajeVigenteFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}