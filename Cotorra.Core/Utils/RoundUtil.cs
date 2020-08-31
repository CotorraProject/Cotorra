using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Utils
{
    public class RoundUtil : IRoundUtil
    {
        private readonly string _currencyName;
        private static RoundUtil _roundUtil;

        public RoundUtil(string currencyName)
        {
            _currencyName = currencyName;
        }

        public static RoundUtil GetInstance()
        {
            if (null == _roundUtil)
            {
                _roundUtil = new RoundUtil("MXN");
            }
            return _roundUtil;
        }

        /// <summary>
        /// Get CurrencyName Selected
        /// </summary>
        /// <returns></returns>
        public string GetCurrencyName()
        {
            return _currencyName;
        }

        /// <summary>
        /// Round value from currency
        /// </summary>
        /// <param name="currencyName"></param>
        /// <param name="valueToRound"></param>
        /// <returns></returns>
        public virtual decimal RoundValue(decimal valueToRound)
        {
            var result = valueToRound;
            //c_Moneda	Descripción	  Decimales	    Porcentaje variación	Fecha inicio de vigencia	Fecha fin de vigencia
            //MXN	    Peso Mexicano	2	           500%	    14/08/2017	
            if (_currencyName == "MXN")
            {
                result = Math.Round(valueToRound, 2);
            }

            return result;
        }

        public virtual double RoundValue(double valueToRound)
        {
            var result = valueToRound;
            //c_Moneda	Descripción	  Decimales	    Porcentaje variación	Fecha inicio de vigencia	Fecha fin de vigencia
            //MXN	    Peso Mexicano	2	           500%	    14/08/2017	
            if (_currencyName == "MXN")
            {
                result = Math.Round(valueToRound, 2);
            }

            return result;
        }

        public virtual decimal RoundValue(decimal valueToRound, int numberOfDecimals)
        {
            return decimal.Round(valueToRound, numberOfDecimals);
        }
    }
}
