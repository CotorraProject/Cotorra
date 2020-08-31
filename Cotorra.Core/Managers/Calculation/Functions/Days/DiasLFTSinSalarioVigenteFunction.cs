using MoreLinq;
using Cotorra.Core.Utils;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions
{
    public class DiasLFTSinSalarioVigenteFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public DiasLFTSinSalarioVigenteFunction(FunctionParams functionParams)
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
            var dateTimeUtil = new DateTimeUtil();

            var historicEmployeeSalaryAdjustments = _functionParams.CalculationBaseResult.Overdraft.Employee.HistoricEmployeeSalaryAdjustments.
                Where(p => p.ModificationDate >= initialDate && p.ModificationDate <= finalDate);

            if (historicEmployeeSalaryAdjustments.Any())
            {
                var modificationDate = historicEmployeeSalaryAdjustments.Select(p => p.ModificationDate);
                if (modificationDate.Any())
                {
                    var initialDateInteration = modificationDate.FirstOrDefault();
                    var vacationDaysNewAdjustment = 0;
                    var lstVacations = _functionParams.CalculationBaseResult.Vacations.AsParallel().ToArray();

                    for (int i = 0; i < _functionParams.CalculationBaseResult.Vacations.Count(); i++)
                    {
                        var periodInitialDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.InitialDate;
                        var periodFinalDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;
                        var vacationInitialDate = lstVacations[i].InitialDate;
                        var vacationFinalDate = lstVacations[i].FinalDate;

                        vacationDaysNewAdjustment += dateTimeUtil.InclusiveDays(initialDateInteration, periodFinalDate, vacationInitialDate, vacationFinalDate);
                        var removeVacationDaysOff = lstVacations[i].VacationDaysOff.Count(daysoff => daysoff.Date >= periodInitialDate && daysoff.Date <= periodFinalDate);
                        vacationDaysNewAdjustment -= removeVacationDaysOff;

                    }
                    result += vacationDaysNewAdjustment;

                    //modificaciones
                    // me voy de vacaciones 17  - 25    ajuste  19    19->25 cuantos dias fueron
                    //                               16,18,25,26 de vacaciones a la playa :)


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
                        result += Convert.ToDouble(numDays + (numHours / hoursPerWorkshift));
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

                    result += inhabilityTotalDays;
                }
            }
            else
            {
                var vacationDaysNewAdjustment = 0;
                var lstVacations = _functionParams.CalculationBaseResult.Vacations.AsParallel().ToArray();

                for (int i = 0; i < _functionParams.CalculationBaseResult.Vacations.Count(); i++)
                {
                    var periodInitialDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.InitialDate;
                    var periodFinalDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;
                    var vacationInitialDate = lstVacations[i].InitialDate;
                    var vacationFinalDate = lstVacations[i].FinalDate;

                    vacationDaysNewAdjustment += dateTimeUtil.InclusiveDays(periodInitialDate, periodFinalDate, vacationInitialDate, vacationFinalDate);
                    var removeVacationDaysOff = lstVacations[i].VacationDaysOff.Count(daysoff => daysoff.Date >= periodInitialDate && daysoff.Date <= periodFinalDate);
                    vacationDaysNewAdjustment -= removeVacationDaysOff;

                }
                result += vacationDaysNewAdjustment;

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
                    result += Convert.ToDouble(numDays + (numHours / hoursPerWorkshift));
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

                result += inhabilityTotalDays;
            }

            return result;
        }
        public FunctionExtension clone()
        {
            return new DiasLFTSinSalarioVigenteFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}