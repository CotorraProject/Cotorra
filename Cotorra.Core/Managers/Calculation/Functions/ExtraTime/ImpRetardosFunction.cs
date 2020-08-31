using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Cotorra.Core.Managers.Calculation.Functions
{
    public class ImpRetardosFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        #endregion

        #region "Constructor"
        public ImpRetardosFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }
        #endregion

        public int getParametersNumber()
        {
            return 0;
        }
        public void setParameterValue(int argumentIndex, double argumentValue)
        {
            throw new NotImplementedException();
        }
        public double calculate()
        {
            return 1;
        }
        public FunctionExtension clone()
        {
            return new ImpRetardosFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}
