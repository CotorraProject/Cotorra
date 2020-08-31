using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.Perceptions
{
    public class TotalPercepcionesFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public TotalPercepcionesFunction(FunctionParams functionParams)
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
            var overdraftDetails = _functionParams.CalculationBaseResult.Overdraft.OverdraftDetails;
            var salaryOverdraftDetails = overdraftDetails.Where(p => 
                p.ConceptPayment.ConceptType == ConceptType.SalaryPayment
            );
            result = Convert.ToDouble(salaryOverdraftDetails.Sum(p => p.Amount));

            return result;
        }
        public FunctionExtension clone()
        {
            return new TotalPercepcionesFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}