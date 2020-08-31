using MoreLinq;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class Finiquito_Indem20Function : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public Finiquito_Indem20Function(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }

        public Finiquito_Indem20Function(FunctionParams functionParams, double x)
        {
            _functionParams = functionParams;
            _x = x;
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
            //var settlementCatalogs = _functionParams.CalculationBaseResult.SettlementCatalogs.Where(p=>p.Number == _x);
            //if (settlementCatalogs.Any())
            //{
            //    var settlementCatalogsTop = settlementCatalogs.MaxBy(p => p.ValidityDate);
            //    result = Convert.ToDouble(settlementCatalogsTop.FirstOrDefault().Indem20);
            //}
            var percetionResult = _functionParams.CalculationBaseResult.Overdraft.OverdraftDetails
                .Where(p =>
                    p.ConceptPayment.Code == 27 &&
                    p.ConceptPayment.ConceptType == ConceptType.SalaryPayment)
                .Sum(p => p.Value);
            result = Convert.ToDouble(percetionResult);

            return result;
        }
        public FunctionExtension clone()
        {
            return new Finiquito_Indem20Function(_functionParams, _x);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}