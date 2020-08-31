using iText.Barcodes;
using Cotorra.Core.Utils;
using System;
using Xunit;


namespace Cotorra.UnitTest.Util
{
   
    public class CurrencyUtilUT
    {
        [Theory]
        [InlineData(2.50, 2.0)]
        [InlineData(2.51, 3.0)]
        [InlineData(2.4901, 2.0)]
        [InlineData(2.509, 3.0)]
        public void GetRoundBancario(double a1, double a2)
        {
            var result = Math.Round(a1, 0);
            Assert.Equal(result, a2);
        }

        [Fact]
        public void GetTextFromQuantityNoDecimalPoint()
        {
            CurrencyUtil cur = new CurrencyUtil();

            var txt = cur.ToText(new CurrencyToTextParams() { CurrencyAmount = 1200, CurrencyCode = "MXN", CurrencyName = "Pesos" });
            Assert.NotEmpty(txt);
            Assert.Equal("Mil doscientos pesos 00/00 MXN", txt);

        }


        [Fact]
        public void GetTextFromQuantityWithDecimalPoint()
        {
            CurrencyUtil cur = new CurrencyUtil();

            var txt = cur.ToText(new CurrencyToTextParams() { CurrencyAmount = 1200.5m, CurrencyCode = "MXN", CurrencyName = "Pesos" });


            Assert.NotEmpty(txt);
            Assert.Equal("Mil doscientos pesos 5/10 MXN", txt);

        }
    }
}
