using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Cotorra.Core.Utils
{
    public class XMLGet
    {
        XmlDocument doc = new XmlDocument();
        XmlNamespaceManager nsmgr;

        public XMLGet(string xmlDoc)
        {
            doc.LoadXml(xmlDoc);
            nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/3");
            nsmgr.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
        }

        public XmlDocument Document { get; }

        public XmlNode SelectSingleNode(string path)
        {

            return doc.SelectSingleNode(path, nsmgr);
        }
    }
}
