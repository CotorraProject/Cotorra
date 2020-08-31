using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Utils
{
    public static class DecimalExtension
    {
        /// <summary>
        /// To Round in custom fixed decimal 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="roundUtil"></param>
        /// <returns></returns>
        public static decimal ToCustomRound(this decimal value, IRoundUtil roundUtil)
        {
            return roundUtil.RoundValue(value);
        }
    }
}
