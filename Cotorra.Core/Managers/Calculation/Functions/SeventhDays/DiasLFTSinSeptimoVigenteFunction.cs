using MoreLinq;
using Cotorra.Core.Utils;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Cotorra.Core.Managers.Calculation.Functions.SeventhDays
{
    public class DiasLFTSinSeptimoVigenteFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public DiasLFTSinSeptimoVigenteFunction(FunctionParams functionParams)
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
        public double calculate()
        {
            var initialDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.InitialDate;
            var finalDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;
            var result = 0.0;

            var historicEmployeeSalaryAdjustments = _functionParams.CalculationBaseResult.Overdraft.Employee.HistoricEmployeeSalaryAdjustments.
                Where(p => p.ModificationDate >= initialDate && p.ModificationDate <= finalDate);

            if (historicEmployeeSalaryAdjustments.Any())
            {
                var modificationDate = historicEmployeeSalaryAdjustments.Select(p => p.ModificationDate);
                if (modificationDate.Any())
                {
                    var incidents = _functionParams.CalculationBaseResult.Incidents.Where(
                        p => p.Date >= modificationDate.FirstOrDefault() &&
                         !p.IncidentType.SalaryRight && 
                         p.IncidentType.TypeOfIncident == TypeOfIncident.Days && 
                         p.IncidentType.DecreasesSeventhDay);

                    var incidentsValue = incidents.Sum(p => p.Value);

                    //restamos las incidencias
                    result += Convert.ToDouble(incidentsValue);

                    //restamos las incapacidades
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
                var incidents = _functionParams.CalculationBaseResult.Incidents.Where(p=>
                        !p.IncidentType.SalaryRight && 
                        p.IncidentType.TypeOfIncident == TypeOfIncident.Days &&
                        p.IncidentType.DecreasesSeventhDay);

                var incidentsValue = incidents.Sum(p => p.Value);

                //restamos las incidencias
                result += Convert.ToDouble(incidentsValue);

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
            return new DiasLFTSinSeptimoVigenteFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}
