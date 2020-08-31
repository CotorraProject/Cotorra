using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace Cotorra.DigitalSign
{
    public class DigitalSign
    {      
        public System.Security.Cryptography.Xml.Signature ApplySignature(
                 byte[] privateKey,
                 string password,
                 string xml)
        {
            try
            {
                SecureString lSecStr = new SecureString();
                lSecStr.Clear();

                foreach (char c in password)
                    lSecStr.AppendChar(c);

                RSACryptoServiceProvider lrsa = OpensslKey.DecodeEncryptedPrivateKeyInfo(privateKey, lSecStr);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = false;
                xmlDoc.LoadXml(xml);
                return this.SignXml(xmlDoc, (RSA)lrsa);
            }
            catch (Exception ex)
            {
                throw new DigitalSignException(109, "109", "La contraseña del certificado no es válida.", ex);
            }
        }

        private System.Security.Cryptography.Xml.Signature SignXml(XmlDocument xmlDoc, RSA Key)
        {
            if (xmlDoc == null)
                throw new ArgumentException(nameof(xmlDoc));
            if (Key == null)
                throw new ArgumentException(nameof(Key));
            SignedXml signedXml = new SignedXml(xmlDoc);
            signedXml.SigningKey = (AsymmetricAlgorithm)Key;
            Reference reference = new Reference();
            reference.Uri = "";
            XmlDsigEnvelopedSignatureTransform signatureTransform = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform((Transform)signatureTransform);
            signedXml.AddReference(reference);
            signedXml.ComputeSignature();
            XmlElement xml = signedXml.GetXml();
            System.Security.Cryptography.Xml.Signature signature = signedXml.Signature;
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode((XmlNode)xml, true));
            return signature;
        }

        /// <summary>
        /// metodo que realiza el sello reciviendo el archivo key como matriaz de bytes
        /// </summary>
        /// <param name="CadenaOriginal"></param>
        /// <param name="ArchivoClavePrivada"></param>
        /// <param name="lPassword"></param>
        /// <returns></returns>
        public string Sign(string CadenaOriginal, byte[] ArchivoClavePrivada, string lPassword)
        {
            byte[] ClavePrivada = ArchivoClavePrivada;
            byte[] bytesFirmados = null;

            SecureString lSecStr = new SecureString();
            SHA256Managed sham = new SHA256Managed();
            lSecStr.Clear();

            foreach (char c in lPassword)
                lSecStr.AppendChar(c);

            RSACryptoServiceProvider lrsa = OpensslKey.DecodeEncryptedPrivateKeyInfo(ClavePrivada, lSecStr);
            try
            {
                bytesFirmados = lrsa.SignData(Encoding.UTF8.GetBytes(CadenaOriginal), sham);

            }
            catch (NullReferenceException ex)
            {
                throw new DigitalSignException(109, "109", "Clave privada incorrecta.", ex);
            }
            string sellodigital = Convert.ToBase64String(bytesFirmados);
            return sellodigital;

        }

        public string SellarMD5(string CadenaOriginal, string ArchivoClavePrivada, string lPassword)
        {
            byte[] ClavePrivada = File.ReadAllBytes(ArchivoClavePrivada);
            byte[] bytesFirmados = null;
            byte[] bCadenaOriginal = null;
            SecureString lSecStr = new SecureString();
            lSecStr.Clear();
            foreach (char c in lPassword)
                lSecStr.AppendChar(c);
            RSACryptoServiceProvider lrsa = OpensslKey.DecodeEncryptedPrivateKeyInfo(ClavePrivada, lSecStr);
            MD5CryptoServiceProvider hasher = new MD5CryptoServiceProvider();
            bCadenaOriginal = Encoding.UTF8.GetBytes(CadenaOriginal);
            hasher.ComputeHash(bCadenaOriginal);
            bytesFirmados = lrsa.SignData(bCadenaOriginal, hasher);
            string sellodigital = Convert.ToBase64String(bytesFirmados);
            return sellodigital;
        }

        public string GetCerticate(byte[] certFile)
        {
            return Base64_Encode(certFile);
        }

        private string Base64_Encode(byte[] str)
        {
            return Convert.ToBase64String(str);
        }
      
        public string GetOriginalString(string NombreXML)
        {
            System.Xml.Xsl.XslCompiledTransform transformer = new System.Xml.Xsl.XslCompiledTransform();
          
            StringWriter strwriter = new StringWriter();
            if (File.Exists("cadenaoriginal_3_3.xslt"))
            {
                //cargamos el xslt transformer
                try
                {
                    transformer.Load("cadenaoriginal_3_3.xslt");
                    //procedemos a realizar la transfomración del archivo xml en base al xslt y lo almacenamos en un string que regresaremos 
                    transformer.Transform(NombreXML, null, strwriter);
                    //convertimos la cadena a utf8 y ya esta lista para ser utilizada en el hash
                    return strwriter.ToString();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            if (File.Exists("cadenaoriginal_3_3.xslt"))
            {
                //cargamos el xslt transformer
                try
                {
                    transformer.Load("cadenaoriginal_3_3.xslt");
                    //procedemos a realizar la transfomración del archivo xml en base al xslt y lo almacenamos en un string que regresaremos 
                    transformer.Transform(NombreXML, null, strwriter);
                    //convertimos la cadena a utf8 y ya esta lista para ser utilizada en el hash
                    return strwriter.ToString();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
            else return "Error al cargar el validador.";


        }

        public string md5(string Value)
        {
            //Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = Encoding.UTF8.GetBytes(Value);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            StringBuilder ret = new StringBuilder();
            for (int i = 0; i < encodedBytes.Length; i++)
                ret.Append(encodedBytes[i].ToString("x2").ToLower());
            return ret.ToString();
        }

        /// <summary>
        /// lee el codigo del certificado enviado este como matriz de bytes
        /// </summary>
        /// <param name="NombreArchivo">certificado en matriz de bytes</param>
        /// <param name="Inicio"></param>
        /// <param name="Final"></param>
        /// <param name="Serie"></param>
        /// <param name="Numero"></param>
        /// <returns></returns>
        public bool leerCER(byte[] NombreArchivo, out string Inicio, out string Final, out string Serie, out string Numero)
        {

            if (NombreArchivo.Length < 1)
            {
                Inicio = "";
                Final = "";
                Serie = "INVALIDO";
                Numero = "";
                return false;
            }
            X509Certificate2 objCert = new X509Certificate2(NombreArchivo);
            StringBuilder objSB = new StringBuilder("Detalle del certificado: \n\n");

            //Detalle
            objSB.AppendLine("Persona = " + objCert.Subject);
            objSB.AppendLine("Emisor = " + objCert.Issuer);
            objSB.AppendLine("Válido desde = " + objCert.NotBefore.ToString());
            Inicio = objCert.NotBefore.ToString();
            objSB.AppendLine("Válido hasta = " + objCert.NotAfter.ToString());
            Final = objCert.NotAfter.ToString();
            objSB.AppendLine("Tamaño de la clave = " + objCert.PublicKey.Key.KeySize.ToString());
            objSB.AppendLine("Número de serie = " + objCert.SerialNumber);
            Serie = objCert.SerialNumber.ToString();

            objSB.AppendLine("Hash = " + objCert.Thumbprint);
            string tNumero = "", rNumero = "", tNumero2 = "";

            int X;
            if (Serie.Length < 2)
                Numero = "";
            else
            {
                foreach (char c in Serie)
                {
                    switch (c)
                    {
                        case '0': tNumero += c; break;
                        case '1': tNumero += c; break;
                        case '2': tNumero += c; break;
                        case '3': tNumero += c; break;
                        case '4': tNumero += c; break;
                        case '5': tNumero += c; break;
                        case '6': tNumero += c; break;
                        case '7': tNumero += c; break;
                        case '8': tNumero += c; break;
                        case '9': tNumero += c; break;
                    }
                }
                for (X = 1; X < tNumero.Length; X++)
                {
                    //wNewString = wNewString & Right$(Left$(wCadena, x), 1)
                    X += 1;
                    //rNumero = rNumero + 
                    tNumero2 = tNumero.Substring(0, X);
                    rNumero = rNumero + tNumero2.Substring(tNumero2.Length - 1, 1);// Right$(Left$(wCadena, x), 1)
                }
                Numero = rNumero;

            }

            if (DateTime.Now < objCert.NotAfter && DateTime.Now > objCert.NotBefore)
            {
                return true;
            }

            return false;
        }

        public (DateTime, DateTime) GetExpirationDate(byte[] cerFile)
        {
            if (cerFile.Length < 1)
            {
                return (default(DateTime), default(DateTime));
            }
            X509Certificate2 objCert = new X509Certificate2(cerFile);

            var startDate = DateTime.Parse(objCert.NotBefore.ToString());
            var endDate = DateTime.Parse(objCert.NotAfter.ToString());

            return (startDate, endDate);
        }


        public string GetCertificateNumber(byte[] cerFile)
        {
            var certificateNumber = "";
            if (cerFile.Length < 1)
            {
                return null;
            }
            X509Certificate2 objCert = new X509Certificate2(cerFile);
            var serie = objCert.SerialNumber.ToString();

            StringBuilder tNumero = new StringBuilder();
            StringBuilder rNumero = new StringBuilder();

            int X;
            if (serie.Length < 2)
                certificateNumber = String.Empty;
            else
            {
                foreach (char c in serie)
                {
                    tNumero.Append(c);
                }
                for (X = 1; X < tNumero.Length; X++)
                {
                    X += 1;
                    var tNumero2 = tNumero.ToString().Substring(0, X);
                    rNumero.Append(tNumero2.Substring(tNumero2.Length - 1, 1));
                }
                certificateNumber = rNumero.ToString();

            }

            return certificateNumber;
        }

        private bool KeyUsageHasUsage(X509Certificate2 cert, X509KeyUsageFlags flag)
        {
            if (cert.Version < 3) { return true; }
            List<X509KeyUsageExtension> extensions = cert.Extensions.OfType<X509KeyUsageExtension>().ToList();
            if (!extensions.Any())
            {
                return flag != X509KeyUsageFlags.CrlSign && flag != X509KeyUsageFlags.KeyCertSign;
            }
            return (extensions[0].KeyUsages & flag) > 0;
        }

        public bool validateCerKEYContent(byte[] cer, byte[] key, string password, string rfc)
        {
            try
            {
                if (String.IsNullOrEmpty(password))
                {
                    throw new DigitalSignException(5050, "5051", "Es necesario especificar la contraseña del certificado.", null);
                }

                SecureString secureString = new SecureString();
                secureString.Clear();

                foreach (char c in password)
                {
                    secureString.AppendChar(c);
                }

                RSACryptoServiceProvider lrsa = OpensslKey.DecodeEncryptedPrivateKeyInfo(key, secureString);

                if (lrsa == null)
                {
                    throw new DigitalSignException(5052, "5052", "La constraseña de los certificados proporcionada no es correcta.", null);
                }
              
                //validate Subject
                X509Certificate2 x509Certificate2 = new X509Certificate2(cer);

                if (!x509Certificate2.Subject.ToLower().Contains(rfc.ToLower()))
                {
                    throw new DigitalSignException(5053, "5053", "El certificado no corresponde con el RFC proporcionado.", null);
                }

                //matches cer and key
                char[] arrayOfChars = password.ToCharArray();
                AsymmetricKeyParameter privateKey = PrivateKeyFactory.DecryptKey(arrayOfChars, key);

                Org.BouncyCastle.X509.X509Certificate bouncyCastleCert = new Org.BouncyCastle.X509.X509Certificate(
                    new X509CertificateParser().ReadCertificate(x509Certificate2.GetRawCertData()).CertificateStructure);

                RsaKeyParameters publicKey = (RsaKeyParameters)bouncyCastleCert.GetPublicKey();
                byte[] numArray = new byte[256];
                new SecureRandom().NextBytes(numArray);
                var signer = new Signer();
                byte[] signature = signer.Sign(privateKey, numArray);
                var isMatched = signer.VerifySignature(publicKey, numArray, signature);

                if (!isMatched)
                {
                    throw new DigitalSignException(5054, "5054", "El .CER no corresponde con el .KEY proporcionado.", null);
                }

                //validación de CSD / FIEL
                if (!
                    (KeyUsageHasUsage(x509Certificate2, X509KeyUsageFlags.DigitalSignature) &&
                     KeyUsageHasUsage(x509Certificate2, X509KeyUsageFlags.NonRepudiation) &&
                     !KeyUsageHasUsage(x509Certificate2, X509KeyUsageFlags.DataEncipherment) &&
                     !KeyUsageHasUsage(x509Certificate2, X509KeyUsageFlags.KeyAgreement)
                    ))
                {
                    throw new DigitalSignException(5055, "5055", "El certificado proporcionado debe de ser un CSD válido (No debe de ser FIEL).", null);
                }

                return true;
            }
            catch (DigitalSignException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DigitalSignException(109, "109", "El certificado proporcionado no es correcto.", ex);
            }
        }
    }
}
