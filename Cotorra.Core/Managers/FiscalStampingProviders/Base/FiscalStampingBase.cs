using CotorraNode.Common.Config;
using CotorraNode.Common.Proxy;
using CotorraNube.CommonApp.RestClient;
using Cotorra.Core.Utils;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Cotorra.Core.Managers.FiscalStamping
{
    /// <summary>
    /// CFDIBase abstract class to help CFDIProviders versions to sign and stamp
    /// </summary>
    public abstract class FiscalStampingBase
    {
        private const string XML_DEFAULTRESOLVER = "Switch.System.Xml.AllowDefaultResolver";

        #region "Constructor"
        protected FiscalStampingBase()
        {
            AppContext.SetSwitch(XML_DEFAULTRESOLVER, true);         
        }
        #endregion

        #region "Properties"
        public virtual string CFDI_Namespace { get; set; }

        public virtual string TFD_Namespace { get; set; }

        public virtual string TFD_xmlns { get; set; }

        public virtual string XSI_Namespace { get; set; }

        public virtual string Payroll_Namespace { get; set; }

        public virtual string Payroll_xmlns { get; set; }

        public virtual string xsltPath { get; set; }

        public virtual string xsltTFDPath { get; set; }

        public virtual string CFDI_PrefixName { get; set; }

        public virtual string TFD_PrefixName { get; set; }

        public virtual string Payroll_PrefixName { get; set; }

        public virtual string SealXPath { get; set; }

        #endregion

        /// <summary>
        /// Get Element Util
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public virtual XmlElement GetElement(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            List<XmlAttribute> attributesToDelete = new List<XmlAttribute>();
            foreach (XmlAttribute attribute in doc.DocumentElement.Attributes)
            {
                if (attribute.Name.Contains(Payroll_xmlns))
                {
                    attributesToDelete.Add(attribute);
                }
                else if (attribute.Name.Contains(TFD_xmlns))
                {
                    attributesToDelete.Add(attribute);
                }
            }
            attributesToDelete.ForEach(attrib =>
            {
                doc.DocumentElement.Attributes.Remove(attrib);
            });

            return doc.DocumentElement;
        }

        /// <summary>
        /// Create XML with correct namespaces
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oComprobante"></param>
        /// <returns></returns>
        public virtual string CreateXml<ICFDINomProvider>(ICFDINomProvider oComprobante, bool isTFD = false)
        {
            XmlSerializerNamespaces xmlNameSpace = new XmlSerializerNamespaces();
            xmlNameSpace.Add(CFDI_PrefixName, CFDI_Namespace);

            if (isTFD)
            {
                xmlNameSpace.Add(TFD_PrefixName, TFD_Namespace);
            }
            xmlNameSpace.Add("xsi", XSI_Namespace);
            xmlNameSpace.Add(Payroll_PrefixName, Payroll_Namespace);

            XmlSerializer oXmlSerializar = new XmlSerializer(oComprobante.GetType());

            string sXml = String.Empty;

            using (var sww = new Utils.StringWriterWithEncoding(Encoding.UTF8))
            {
                using (XmlWriter writter = XmlWriter.Create(sww))
                {

                    oXmlSerializar.Serialize(writter, oComprobante, xmlNameSpace);
                    sXml = sww.ToString();
                }
            }

            return sXml;
        }

        /// <summary>
        /// Create payroll complement
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oNomina"></param>
        /// <returns></returns>
        public virtual string CreateXmlNom<T>(T oNomina) where T : class
        {
            XmlSerializerNamespaces xmlNameSpace = new XmlSerializerNamespaces();
            XmlSerializer oXmlSerializar = new XmlSerializer(typeof(T));
            xmlNameSpace.Add(Payroll_PrefixName, Payroll_Namespace);

            string sXml = String.Empty;

            using (var sww = new Utils.StringWriterWithEncoding(Encoding.UTF8))
            {
                using (XmlWriter writter = XmlWriter.Create(sww))
                {

                    oXmlSerializar.Serialize(writter, oNomina, xmlNameSpace);
                    sXml = sww.ToString();
                }
            }

            return sXml;
        }

        /// <summary>
        /// Create the TFD xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oNomina"></param>
        /// <returns></returns>
        public virtual string CreateXmlTFD<T>(T oNomina) where T : class
        {
            XmlSerializerNamespaces xmlNameSpace = new XmlSerializerNamespaces();
            XmlSerializer oXmlSerializar = new XmlSerializer(typeof(T));
            xmlNameSpace.Add(TFD_PrefixName, TFD_Namespace);

            string sXml = String.Empty;

            using (var sww = new Utils.StringWriterWithEncoding(Encoding.UTF8))
            {
                using (XmlWriter writter = XmlWriter.Create(sww))
                {

                    oXmlSerializar.Serialize(writter, oNomina, xmlNameSpace);
                    sXml = sww.ToString();
                }
            }

            return sXml;
        }

        /// <summary>
        /// Sign the comprobante document
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oComprobante"></param>
        /// <returns></returns>
        public virtual SignDocumentResult<ICFDINomProvider> SignDocument(ICFDINomProvider cFDINomProvider,
            byte[] certificateCER, byte[] privateKey, string password)
        {
            //Instance CFDI Provider - Arrange
            SignDocumentResult<ICFDINomProvider> signDocumentResult = new SignDocumentResult<ICFDINomProvider>();
            var digitalSign = new Cotorra.DigitalSign.DigitalSign();

            //Certificate B64 y Certificate Number
            signDocumentResult.CertificateB64 = digitalSign.GetCerticate(certificateCER);
            signDocumentResult.CertificateNumber = digitalSign.GetCertificateNumber(certificateCER);
                       

            //Complete CFDI
            cFDINomProvider.Certificado = signDocumentResult.CertificateB64;
            cFDINomProvider.NoCertificado = signDocumentResult.CertificateNumber;

            //Get XML             
            var xml = CreateXml(cFDINomProvider, false);
           
            //Get Original String
            var originalString = GetOriginalString(xml);

            //Sign Document           
            signDocumentResult.SignString = digitalSign.Sign(originalString, privateKey, password);
            cFDINomProvider.Sello = signDocumentResult.SignString;
            

            //CFDIProvider
            signDocumentResult.CFDI = cFDINomProvider;
            signDocumentResult.OriginalString = originalString;

            return signDocumentResult;
        }

        /// <summary>
        /// Stamp Document (lo implementa su hijo)
        /// </summary>
        /// <param name="signDocumentResult"></param>
        /// <returns></returns>
        public virtual async Task<SignDocumentResult<ICFDINomProvider>> StampDocumetAsync(SignDocumentResult<ICFDINomProvider> signDocumentResult)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get Original String
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public virtual string GetOriginalString(String xml)
        {
            string result = String.Empty;

            string pathxsl = Path.Combine(DirectoryUtil.AssemblyDirectory, xsltPath);

            System.Xml.Xsl.XslCompiledTransform transformador = new System.Xml.Xsl.XslCompiledTransform(true);
            transformador.Load(pathxsl);

            using (StringReader sri = new StringReader(xml)) // xmlInput is a string that contains xml
            {
                using (XmlReader xri = XmlReader.Create(sri))
                {
                    using (StringWriter sw = new StringWriter())
                    {
                        using (XmlWriter xwo = XmlWriter.Create(sw, transformador.OutputSettings))
                        {
                            transformador.Transform(xri, xwo);
                            result = sw.ToString();
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get OriginalString
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public virtual string GetStampingOriginalString(String xmlString)
        {
            XMLGet xml = new XMLGet(xmlString);
            string sealName = SealXPath;

            var sealNode = xml.SelectSingleNode(sealName).OuterXml;
            return new XsltUtil().Transform(sealNode, xsltTFDPath);
        }
    }
}
