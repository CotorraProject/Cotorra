using Cotorra.Core.Managers.FiscalStamping;
using Cotorra.Core.Utils;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Cotorra.Core.Managers.FiscalValidator
{
    public class FiscalValidatorCFDIv33Nomv12 : IFiscalValidator
    {
        private readonly string _xsdPartialPath;
        private readonly string _xsdPath;
        private readonly FiscalStampingCFDIv33Nomv12 _fiscalStampingCFDIv33Nomv12;

        #region "Constructor"
        public FiscalValidatorCFDIv33Nomv12()
        {
            _xsdPartialPath = Path.Combine(@"fiscal","cfdi33nom12","xsd");
            _xsdPath = Path.Combine(DirectoryUtil.AssemblyDirectory, _xsdPartialPath);
            _fiscalStampingCFDIv33Nomv12 = new FiscalStampingCFDIv33Nomv12();
        }
        #endregion


        public async Task<string> ValidateCFDIAsync(string xml)
        {
            List<string> xsdsFiles = new List<string> {
                Path.Combine(_xsdPath,"catNomina.xsd") ,
                Path.Combine(_xsdPath,"nomina12.xsd") ,
                Path.Combine(_xsdPath,"cfdv33.xsd") ,
            };

            var xdoc = XDocument.Parse(xml);
            var schemas = new XmlSchemaSet();
            foreach (var xsdFile in xsdsFiles)
            {
                using (FileStream stream = File.OpenRead(xsdFile))
                {
                    schemas.Add(XmlSchema.Read(stream, (s, e) =>
                    {
                        var x = e.Message;
                    }));
                }
            }

            StringBuilder sb = new StringBuilder();
            try
            {
                xdoc.Validate(schemas, (s, e) =>
                {
                    sb.AppendLine($"Line : {e.Exception.LineNumber}, Message : {e.Exception.Message} ");
                });
            }
            catch (XmlSchemaValidationException ex)
            {
                sb.AppendLine(ex.Message);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Valida el xml (validación de estructura - 301)
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public async Task<string> ValidateCFDIAsync(SignDocumentResult<ICFDINomProvider> signDocumentResult)
        {
            var xml = _fiscalStampingCFDIv33Nomv12.CreateXml<ICFDINomProvider>(signDocumentResult.CFDI);
            List<string> xsdsFiles = new List<string> {                
                Path.Combine(_xsdPath,"catNomina.xsd") ,
                Path.Combine(_xsdPath,"nomina12.xsd") ,
                Path.Combine(_xsdPath,"cfdv33.xsd") ,
            };

            var xdoc = XDocument.Parse(xml);
            var schemas = new XmlSchemaSet();
            foreach (var xsdFile in xsdsFiles)
            {
                using (FileStream stream = File.OpenRead(xsdFile))
                {
                    schemas.Add(XmlSchema.Read(stream, (s, e) =>
                    {
                        var x = e.Message;
                    }));
                }
            }

            StringBuilder sb = new StringBuilder();
            try
            {
                xdoc.Validate(schemas, (s, e) =>
                {
                    sb.AppendLine($"Line : {e.Exception.LineNumber}, Message : {e.Exception.Message} ");
                });
            }
            catch (XmlSchemaValidationException ex)
            {
                sb.AppendLine(ex.Message);
            }

            return sb.ToString();
        }
    }
}
