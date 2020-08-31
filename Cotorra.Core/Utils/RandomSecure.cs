using System;
using System.Security.Cryptography;

namespace Cotorra.Core.Utils
{
    public static class RandomSecure
    {
        // Return a random integer between a min and max value.
        public static int RandomIntFromRNG(int min, int max)
        {
            RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();

            // Generate four random bytes
            byte[] four_bytes = new byte[4];
            rNGCryptoServiceProvider.GetBytes(four_bytes);

            // Convert the bytes to a UInt32
            UInt32 scale = BitConverter.ToUInt32(four_bytes, 0);

            // And use that to pick a random number >= min and < max
            return (int)(min + (max - min) * (scale / (uint.MaxValue + 1.0)));
        }
    }
}
