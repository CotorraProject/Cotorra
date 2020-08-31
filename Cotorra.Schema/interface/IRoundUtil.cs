using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public interface IRoundUtil
    {
        decimal RoundValue(decimal valueToRound);

        double RoundValue(double valueToRound);

        decimal RoundValue(decimal valueToRound, int numberOfDecimals);

        string GetCurrencyName();
    }
}
