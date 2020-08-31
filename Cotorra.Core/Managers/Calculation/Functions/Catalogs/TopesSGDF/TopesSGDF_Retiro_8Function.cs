﻿using MoreLinq;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class TopesSGDF_Retiro_8Function : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public TopesSGDF_Retiro_8Function(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }

        public TopesSGDF_Retiro_8Function(FunctionParams functionParams, double x)
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
            var sgdfLimits = _functionParams.CalculationBaseResult.SGDFLimits;
            if (sgdfLimits.Any())
            {
                var sgdfLimitTop = sgdfLimits.MaxBy(p => p.ValidityDate);
                result = Convert.ToDouble(sgdfLimitTop.FirstOrDefault().Retiro_8);
            }

            return result;
        }
        public FunctionExtension clone()
        {
            return new TopesSGDF_Retiro_8Function(_functionParams, _x);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}