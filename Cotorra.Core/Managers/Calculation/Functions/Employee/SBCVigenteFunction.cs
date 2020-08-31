using Cotorra.Core.Managers.Calculation.Functions.Catalogs;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions
{
    public class SBCVigenteFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public SBCVigenteFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }
        #endregion

        public SBCVigenteFunction(double x)
        {
        }
        public int getParametersNumber()
        {
            return 1;
        }
        public void setParameterValue(int argumentIndex, double argumentValue)
        {

        }
        public double calculate()
        {
            var sbc = _functionParams.CalculationBaseResult.Overdraft.Employee.SBCMax25UMA;
            //Originalmente el sbc debe de tomarse del 25 tope UMA, pero en el caso de ser 0, 
            //deberá tomarse el Fijo, siempre y cuando el cálculo sea para obligaciones
            var saldiariovigente = new SalDiarioVigenteFunction(_functionParams).calculate();
            double salariominimo = 0;

            if (_functionParams.CalculationBaseResult.Overdraft.Employee.SalaryZone == SalaryZone.ZoneA)
            {
                salariominimo = new SalariosMinimos_ZonaA(_functionParams).calculate();
            }
            else if (_functionParams.CalculationBaseResult.Overdraft.Employee.SalaryZone == SalaryZone.ZoneB)
            {
                salariominimo = new SalariosMinimos_ZonaB(_functionParams).calculate();
            }
            else if (_functionParams.CalculationBaseResult.Overdraft.Employee.SalaryZone == SalaryZone.ZoneC)
            {
                salariominimo = new SalariosMinimos_ZonaC(_functionParams).calculate();
            }

            var condition = saldiariovigente > salariominimo;
            if (sbc <= 0 && (_functionParams.CalculationBaseResult.IsLiability || condition))
            {
                sbc = _functionParams.CalculationBaseResult.Overdraft.Employee.SBCFixedPart;
            }

            var result = Convert.ToDouble(sbc);
            return result;
        }
        public FunctionExtension clone()
        {
            return new SBCVigenteFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}
