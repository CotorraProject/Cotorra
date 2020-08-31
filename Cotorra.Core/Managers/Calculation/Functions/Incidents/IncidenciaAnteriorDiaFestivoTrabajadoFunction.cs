using CotorraNode.Common.Library.Private;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.Incidents
{
    public class IncidenciaAnteriorDiaFestivoTrabajadoFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public IncidenciaAnteriorDiaFestivoTrabajadoFunction(FunctionParams functionParams)
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
            //IncidenciaAnterior[Día festivo trabajado]
            var result = 0.0;

            var initialDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.InitialDate;
            var finalDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;

            var historicEmployeeSalaryAdjustments = _functionParams.CalculationBaseResult.Overdraft.Employee.HistoricEmployeeSalaryAdjustments.
                Where(p => p.ModificationDate >= initialDate && p.ModificationDate <= finalDate);

            if (historicEmployeeSalaryAdjustments.Any())
            {
                var incidents = _functionParams.CalculationBaseResult.Incidents;

                var incidentHE1 = incidents.Where(p =>
                        p.IncidentType.Code == "DFT " &&
                        p.Date < historicEmployeeSalaryAdjustments.FirstOrDefault().ModificationDate);

                result = Convert.ToDouble(incidentHE1.Sum(p=>p.Value));
            }
            return result;
        }
        public FunctionExtension clone()
        {
            return new IncidenciaAnteriorDiaFestivoTrabajadoFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}