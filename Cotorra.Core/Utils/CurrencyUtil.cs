using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Cotorra.Core.Utils
{
    public class CurrencyUtil
    {
        public string ConvertAmountToWordsByCurrencyCode(string code, string amount)
        {
            decimal amountDecimal = 0;
            if (string.IsNullOrEmpty(amount))
            {
                amount = "0.00";
            }
            if (!Decimal.TryParse(amount, out amountDecimal))
            {
                amountDecimal = Convert.ToDecimal(amount);
            }
            CurrencyToTextParams currencyParams = new CurrencyToTextParams()
            {
                CurrencyAmount = amountDecimal,
                CurrencyCode = code.ToUpper(),
                CurrencyName = "pesos"
            };

            return ToText(currencyParams);
        }

        public string AmountSeparator(string amount)
        {
            if (!String.IsNullOrEmpty(amount))
            {
                var todecimal = Convert.ToDecimal(amount);
                if (todecimal > 0)
                {
                    CultureInfo esCulture = new CultureInfo("es-MX");
                    return todecimal.ToString("#,###0.00", esCulture);
                }
                else
                {
                    return "0.00";
                }
            }
            return "0.00";
        }

        public string ToText(CurrencyToTextParams currencyParams)
        {
            char decimalSeparator = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            string[] amountSplit = currencyParams.CurrencyAmount.ToString().Split(decimalSeparator);

            if (amountSplit != null && amountSplit.Length == 1)
            {
                return string.Format("{0} {1} {2} {3}",
                                      UpperCaseFirst(EvaluateNumber(Convert.ToInt64(amountSplit[0]))),
                                      (currencyParams.CurrencyName).ToLower(),
                                      "00/00",
                                      (currencyParams.CurrencyCode));
            }
            else if (amountSplit != null && amountSplit.Length == 2)
            {
                return string.Format("{0} {1} {2} {3}",
                                      UpperCaseFirst(EvaluateNumber(Convert.ToInt64(amountSplit[0]))),
                                      (currencyParams.CurrencyName).ToLower(),
                                      DecimalPartToText(amountSplit[1]),
                                      (currencyParams.CurrencyCode));
            }
            return string.Empty;
        }

        public string ConvertAmountToWords(string code, string name, decimal amount)
        {
            CurrencyToTextParams currencyParams = new CurrencyToTextParams()
            {
                CurrencyAmount = amount,
                CurrencyCode = code,
                CurrencyName = name
            };
            return ToText(currencyParams);
        }

        #region Private methods

        private string EvaluateNumber(long value)
        {
            if (value < 0)
                throw new NotSupportedException("Números negativos no soportados");
            if (value < 10)
                return DigitToText((int)value, true);
            else
            if (value < 20)
                return TenToText((int)value);
            else
            if (value < 100)
                return HighTenToText((int)value);
            else
            if (value < 1000)
                return HundredToText((int)value);
            else
            if (value < 1000000)
                return ThousandToText((int)value);
            else
            if (value < 1000000000)
                return MillionToText(value);

            return string.Empty;
        }

        private string DigitToText(int digit, bool justOneDigit = false)
        {
            switch (digit)
            {
                case 0: return "cero";
                case 1: return justOneDigit ? "un" : "uno";
                case 2: return "dos";
                case 3: return "tres";
                case 4: return "cuatro";
                case 5: return "cinco";
                case 6: return "seis";
                case 7: return "siete";
                case 8: return "ocho";
                case 9: return "nueve";
            }
            return string.Empty;
        }

        private string TenToText(int ten)
        {
            switch (ten)
            {
                case 10: return "diez";
                case 11: return "once";
                case 12: return "doce";
                case 13: return "trece";
                case 14: return "catorce";
                case 15: return "quince";
            }
            if (ten < 20)
            {
                return "dieci" + EvaluateNumber(ten - 10);
            }
            return string.Empty;
        }

        private string HighTenToText(int ten)
        {
            if (ten > 20 && ten < 30)
            {
                return "veinti" + EvaluateNumber(ten - 20);
            }
            switch (ten)
            {
                case 20: return "veinte";
                case 30: return "treinta";
                case 40: return "cuarenta";
                case 50: return "cincuenta";
                case 60: return "sesenta";
                case 70: return "setenta";
                case 80: return "ochenta";
                case 90: return "noventa";
            }
            if (ten < 100)
            {
                int highTen = (ten / 10) * 10;
                int highTenDigit = ten % 10;

                return string.Format("{0} y {1}", EvaluateNumber(highTen), highTenDigit == 1 ? "un" : EvaluateNumber(highTenDigit));
            }
            return string.Empty;
        }

        private string HundredToText(int hundred)
        {
            if (hundred > 100 && hundred < 200)
            {
                return "ciento " + EvaluateNumber(hundred - 100);
            }
            switch (hundred)
            {
                case 100: return "cien";
                case 200:
                case 300:
                case 400:
                case 600:
                case 800:
                    return EvaluateNumber(hundred / 100) + "cientos";
                case 500: return "quinientos";
                case 700: return "setecientos";
                case 900: return "novecientos";
            }
            if (hundred < 1000)
            {
                return string.Format("{0} {1}", EvaluateNumber((hundred / 100) * 100), EvaluateNumber(hundred % 100));
            }
            return string.Empty;
        }

        private string ThousandToText(int thousand)
        {
            if (thousand > 1000 && thousand < 2000)
            {
                return "mil " + EvaluateNumber(thousand % 1000);
            }
            switch (thousand)
            {
                case 1000: return "mil";
            }
            if (thousand < 1000000)
            {
                return EvaluateNumber(thousand / 1000) + " mil" +
                       (thousand % 1000 > 0 ? " " + EvaluateNumber(thousand % 1000) : string.Empty);
            }
            return string.Empty;
        }

        private string MillionToText(Int64 million)
        {
            if (million > 1000000 && million < 2000000)
            {
                return "un millón " + EvaluateNumber((int)million % 1000000);
            }
            switch (million)
            {
                case 1000000: return "un millón";
            }
            if (million < 1000000000)
            {
                int remainder = (int)(million - (million / 1000000) * 1000000);

                return EvaluateNumber((int)million / 1000000) + " millones" +
                       (remainder > 0 ? " " + EvaluateNumber(remainder) : string.Empty);
            }
            return string.Empty;
        }

        private string DecimalPartToText(string decimalValue)
        {
            int decimalCount = decimalValue.Length;
            double divider = Math.Pow(10, decimalCount);
            return string.Format("{0}/{1}", decimalValue, divider.ToString());
        }

        private string UpperCaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        #endregion
    }

    public class CurrencyToTextParams
    {
        public string CurrencyName;
        public string CurrencyCode;
        public decimal CurrencyAmount;
    }
}
