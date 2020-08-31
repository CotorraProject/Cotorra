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
    public class DiasInfonavitAmortizacionFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public DiasInfonavitAmortizacionFunction(FunctionParams functionParams)
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
          
            //Días de salario
            result += _functionParams.CalculationBaseResult.Overdraft.OverdraftDetails
                .Where(p => p.ConceptPayment.Code == 1 && p.ConceptPayment.ConceptType == ConceptType.SalaryPayment)
                .Sum(p => Convert.ToDouble(p.Value));

            //Días de séptimos
            result += _functionParams.CalculationBaseResult.Overdraft.OverdraftDetails
               .Where(p => p.ConceptPayment.Code == 3 && p.ConceptPayment.ConceptType == ConceptType.SalaryPayment)
               .Sum(p => Convert.ToDouble(p.Value));

            //Días de vacaciones
            if (_functionParams.CalculationBaseResult.Vacations.Any())
            {
                var vacacionesATiempoValorFunction = new Values.VacacionesATiempoValorFunction(_functionParams);
                result += vacacionesATiempoValorFunction.calculate();
            }
          
            /* menos(-) los dias por diferencia de cuando comienza aplicar la trajeta. Lo último es si la fecha de aplicación del 
             * crédito es 05/01/2020 y el periodo es del 01/01/2020 al 15/01/2020 entonces hay 4 días donde dicho crédito 
             * no aplicaba por lo tanto debe descontarse. */
            var infonavitApplicationDate = _functionParams.CalculationBaseResult.InfonavitMovements.FirstOrDefault().InitialApplicationDate;
            if (infonavitApplicationDate >= initialDate && infonavitApplicationDate <= finalDate)
            {
                result -= (infonavitApplicationDate - initialDate).TotalDays;
            }

            return result;
        }
        public FunctionExtension clone()
        {
            return new DiasInfonavitAmortizacionFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}