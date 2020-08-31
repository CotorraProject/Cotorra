using Cotorra.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cotorra.UnitTest
{
    public class EncryptManagerUT
    {
        [Fact]
        public void EncryptDecryptTM()
        {
            var originalText = "Hola";
            var t = StringCipher.Encrypt(originalText);
            var res = StringCipher.Decrypt(t);

            Assert.True(res == originalText);
        }
    }
}
