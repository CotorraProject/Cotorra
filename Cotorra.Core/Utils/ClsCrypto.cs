using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core.Utils
{
    /// <summary>
    /// AES Encryption
    /// </summary>
    public class ClsCrypto
    {
        #region "Attributes"
        private readonly RijndaelManaged myRijndael = new RijndaelManaged();
        private readonly int iterations;
        private readonly byte[] _salt;
        #endregion

        #region "Constructor"
        public ClsCrypto(string strPassword, string iv, string salt)
        {
            myRijndael.BlockSize = 128;
            myRijndael.KeySize = 128;
            myRijndael.IV = HexStringToByteArray(iv);

            myRijndael.Padding = PaddingMode.PKCS7;
            myRijndael.Mode = CipherMode.CBC;
            iterations = 1000;
            _salt = System.Text.Encoding.UTF8.GetBytes(salt);
            myRijndael.Key = GenerateKey(strPassword);
        }
        #endregion

        #region "Public Methods"
        public string Encrypt(string strPlainText)
        {
            byte[] strText = new System.Text.UTF8Encoding().GetBytes(strPlainText);
            ICryptoTransform transform = myRijndael.CreateEncryptor();
            byte[] cipherText = transform.TransformFinalBlock(strText, 0, strText.Length);

            return Convert.ToBase64String(cipherText);
        }

        public string Decrypt(string encryptedText)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            var decryptor = myRijndael.CreateDecryptor(myRijndael.Key, myRijndael.IV);
            byte[] originalBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(originalBytes);
        }
        #endregion

        #region "Private Methods"
        private byte[] HexStringToByteArray(string strHex)
        {
            dynamic r = new byte[strHex.Length / 2];
            for (int i = 0; i <= strHex.Length - 1; i += 2)
            {
                r[i / 2] = Convert.ToByte(Convert.ToInt32(strHex.Substring(i, 2), 16));
            }
            return r;
        }

        private byte[] GenerateKey(string strPassword)
        {
            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(System.Text.Encoding.UTF8.GetBytes(strPassword), _salt, iterations);

            return rfc2898.GetBytes(128 / 8);
        }
        #endregion
    }
}
