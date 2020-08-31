using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions
{
    public class TipoEstadoEmpleadoPeriodoFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public TipoEstadoEmpleadoPeriodoFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }
        #endregion

        public TipoEstadoEmpleadoPeriodoFunction(double x)
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
            /*  "VEstadoEmpleadoPeriodo",
                "TipoEstadoEmpleadoPeriodo()",
                "Esta funcion  regresa un rango de 1 al 17 dependiendo si el empleado esta de alta 
                baja o reingreso: 

                1  = alta en el periodo
                2  = alta en periodo anterior
                3  = alta, baja y reingreso en el periodo
                4  = reingreso, baja y reingreso en el periodo
                5  = baja y primer reingreso en el periodo
                6  = baja y reingreso en el periodo (2do reingreso o mas)|
                7  = reingreso en el periodo
                8  = reingreso en el periodo(2do reingreso o mas)
                9  = reingreso en periodos pasados
                10 = reingreso en periodos pasados(2do reingreso o mas)
                11 = baja en el periodo
                12 = baja en periodos pasados
                13 = alta y baja en el periodo
                14 = baja, reingreso y otra vez baja en el periodo
                15 = reingreso y baja en el periodo
                16 = baja en el periodo(2da baja o mas)
                17 = baja en periodos pasados(2da baja o mas)
            */

            var result = 1.0;
            return Convert.ToDouble(result);
        }
        public FunctionExtension clone()
        {
            return new TipoEstadoEmpleadoPeriodoFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}
