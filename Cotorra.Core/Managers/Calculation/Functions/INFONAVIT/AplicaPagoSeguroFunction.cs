﻿using MoreLinq;
using Cotorra.Core.Utils;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.INFONAVIT
{
    public class AplicaPagoSeguroFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public AplicaPagoSeguroFunction(FunctionParams functionParams)
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
            result = _functionParams.CalculationBaseResult.InfonavitMovements.FirstOrDefault().IncludeInsurancePayment_D14 ? 1 : 0;
            return result;
        }
        public FunctionExtension clone()
        {
            return new AplicaPagoSeguroFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}