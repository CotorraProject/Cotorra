using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace Cotorra.DigitalSign
{
    internal class Signer
    {
        private const string Algorithm = "SHA256withRSA";

        public byte[] Sign(AsymmetricKeyParameter privatekey, byte[] data)
        {
            Org.BouncyCastle.Crypto.ISigner signer = SignerUtilities.GetSigner(Algorithm);
            signer.Init(true, privatekey);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.GenerateSignature();
        }

        public bool VerifySignature(AsymmetricKeyParameter publicKey, byte[] data, byte[] signature)
        {
            Org.BouncyCastle.Crypto.ISigner signer = SignerUtilities.GetSigner(Algorithm);
            signer.Init(false, publicKey);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.VerifySignature(signature);
        }
    }
}
