﻿using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Linq;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class PercepcionBonoPuntualidadTotalFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public PercepcionBonoPuntualidadTotalFunction(FunctionParams functionParams)
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
            //Percepción 15 Bono de Puntualidad Total
            var result = 0.0;
            var percetionResult = _functionParams.CalculationBaseResult.Overdraft.OverdraftDetails
                .Where(p=>
                    p.ConceptPayment.Code == 15 && 
                    p.ConceptPayment.ConceptType == ConceptType.SalaryPayment)
                .Sum(p=>p.Amount);
            result = Convert.ToDouble(percetionResult);
            return result;
        }
        public FunctionExtension clone()
        {
            return new PercepcionBonoPuntualidadTotalFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}