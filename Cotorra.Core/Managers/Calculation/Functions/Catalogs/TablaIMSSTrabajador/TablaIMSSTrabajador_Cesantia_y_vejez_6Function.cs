﻿using MoreLinq;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.Catalogs
{
    public class TablaIMSSTrabajador_Cesantia_y_vejez_6Function : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public TablaIMSSTrabajador_Cesantia_y_vejez_6Function(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }

        public TablaIMSSTrabajador_Cesantia_y_vejez_6Function(FunctionParams functionParams, double x)
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
            var iMSSEmployeeTables = _functionParams.CalculationBaseResult.IMSSEmployeeTables;
            if (iMSSEmployeeTables.Any())
            {
                var iMSSEmployeeTablesTop = iMSSEmployeeTables.MaxBy(p => p.ValidityDate);
                result = Convert.ToDouble(iMSSEmployeeTablesTop.FirstOrDefault().Cesantia_y_vejez_6);
            }

            return result;
        }
        public FunctionExtension clone()
        {
            return new TablaIMSSTrabajador_Cesantia_y_vejez_6Function(_functionParams, _x);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}