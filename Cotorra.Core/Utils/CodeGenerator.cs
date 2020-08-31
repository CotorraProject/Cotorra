using System;
using System.Linq;

namespace Cotorra.Core.Utils
{
    /// <summary>
    /// Genera de forma aleatoria un código de la longitud indicada
    /// </summary>
    public static class CodeGenerator
    {        
        public static string GetRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[RandomSecure.RandomIntFromRNG(0,s.Length)]).ToArray());
        }
    }
}
