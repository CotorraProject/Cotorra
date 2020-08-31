using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions
{
    public class SBCAnteriorFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public SBCAnteriorFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }
        #endregion

        public SBCAnteriorFunction(double x)
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
            var initialDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.InitialDate;
            var finalDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;
            var result = 0.0;

            var historicEmployeeSalaryAdjustments = _functionParams.CalculationBaseResult.Overdraft.Employee.HistoricEmployeeSBCAdjustments.
                Where(p => p.ModificationDate >= initialDate && p.ModificationDate <= finalDate);

            if (historicEmployeeSalaryAdjustments.Any())
            {
                var sbc = historicEmployeeSalaryAdjustments.Average(p => p.SBCMax25UMA);

                var saldiarioanterior = new SalDiarioAntFunction(_functionParams).calculate();
                double salariominimo = 0;

                if (_functionParams.CalculationBaseResult.Overdraft.Employee.SalaryZone == SalaryZone.ZoneA)
                {
                    salariominimo = new Catalogs.SalariosMinimos_ZonaA(_functionParams).calculate();
                }
                else if (_functionParams.CalculationBaseResult.Overdraft.Employee.SalaryZone == SalaryZone.ZoneB)
                {
                    salariominimo = new Catalogs.SalariosMinimos_ZonaB(_functionParams).calculate();
                }
                else if (_functionParams.CalculationBaseResult.Overdraft.Employee.SalaryZone == SalaryZone.ZoneC)
                {
                    salariominimo = new Catalogs.SalariosMinimos_ZonaC(_functionParams).calculate();
                }

                var condition = saldiarioanterior > salariominimo;
                if (sbc <= 0 && (_functionParams.CalculationBaseResult.IsLiability || condition))
                {
                    sbc = _functionParams.CalculationBaseResult.Overdraft.Employee.SBCFixedPart;
                }

                result = Convert.ToDouble(sbc);
            }

            return result;
        }
        public FunctionExtension clone()
        {
            return new SBCAnteriorFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}
