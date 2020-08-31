using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace Cotorra.Core.Utils
{
    public class XsltUtil
    {
        public string Transform(String xml, String xsltPath, XsltArgumentList xsltArgumentList)
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
                            transformador.Transform(xri, xsltArgumentList, xwo);
                            result = sw.ToString();
                        }
                    }
                }
            }

            return result;
        }

        public string Transform(String xml, String xsltPath)
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
    }
}
