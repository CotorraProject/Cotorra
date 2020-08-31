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
    public class IncidenciaAnteriorHE1Function : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public IncidenciaAnteriorHE1Function(FunctionParams functionParams)
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
            //IncidenciaAnterior[Horas extras 1]
            var result = 0.0;

            var initialDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.InitialDate;
            var finalDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;

            var historicEmployeeSalaryAdjustments = _functionParams.CalculationBaseResult.Overdraft.Employee.HistoricEmployeeSalaryAdjustments.
                Where(p => p.ModificationDate >= initialDate && p.ModificationDate <= finalDate);

            if (historicEmployeeSalaryAdjustments.Any())
            {
                var incidents = _functionParams.CalculationBaseResult.Incidents;

                var incidentHE1 = incidents.Where(p =>
                        p.IncidentType.TypeOfIncident == TypeOfIncident.Hours && 
                        p.IncidentType.Code == "HE" && 
                        p.Date <= historicEmployeeSalaryAdjustments.FirstOrDefault().ModificationDate);

                result = Convert.ToDouble(incidentHE1.Sum(p=> p.Value));
            }
            return result;
        }
        public FunctionExtension clone()
        {
            return new IncidenciaAnteriorHE1Function(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}