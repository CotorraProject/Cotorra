using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions
{
    public class TipoProcesoFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public TipoProcesoFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }

        public TipoProcesoFunction(double x)
        {
            _x = x;
        }

        public TipoProcesoFunction(FunctionParams functionParams, double x)
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
            /*  V = Vacaciones
                F = Finiquito
                P = PTU
                A = Cálculo periodo normal
            */

            var tipoProceso = (int)'A';
            var result = tipoProceso;
            return result;
        }

        public FunctionExtension clone()
        {
            return new TipoProcesoFunction(_functionParams, _x);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}