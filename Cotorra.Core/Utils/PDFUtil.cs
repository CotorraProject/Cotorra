using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.IO.Font.Otf;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Splitting;
using iText.StyledXmlParser.Css.Media;
using System.Collections.Generic;
using System.IO;

namespace Cotorra.Core.Utils
{
    public class PDFUtil
    {
        public class CJKSplitCharacters : ISplitCharacters
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="text"></param>
            /// <param name="glyphPos"></param>
            /// <returns></returns>
            public bool IsSplitCharacter(GlyphLine text, int glyphPos)
            {
                if (glyphPos >= 30)
                {
                    return true;
                }
                return new DefaultSplitCharacters().IsSplitCharacter(text, glyphPos);
            }
        }

        public PDFUtil()
        {

        }

        public byte[] Convert(string htmlContent)
        {
            byte[] pdf = null;
            using (var memoryStream = new MemoryStream())
            {
                var writterProperties = new WriterProperties()
                    .SetFullCompressionMode(true);
                using (PdfWriter writer = new PdfWriter(memoryStream, writterProperties))
                {
                    PdfDocument pdfDoc = new PdfDocument(writer);
                    pdfDoc.SetTagged();

                    PageSize pageSize = new PageSize(900, 1000);
                    pdfDoc.SetDefaultPageSize(pageSize);

                    ConverterProperties converterProperties = new ConverterProperties();
                    converterProperties.SetCreateAcroForm(true);

                    var fp = new DefaultFontProvider(true, false, false);
                    converterProperties.SetFontProvider(fp);

                    MediaDeviceDescription mediaDescription = new MediaDeviceDescription(MediaType.SCREEN);
                    converterProperties.SetMediaDeviceDescription(mediaDescription);

                    var elements = HtmlConverter.ConvertToElements(htmlContent, converterProperties);
                    Document document = new Document(pdfDoc);
                    CJKSplitCharacters splitCharacters = new CJKSplitCharacters();
                    document.SetSplitCharacters(splitCharacters);
                    document.SetProperty(Property.SPLIT_CHARACTERS, splitCharacters);
                    foreach (IElement element in elements)
                    {
                        document.Add((IBlockElement)element);
                    }
                    document.Close();

                    pdf = memoryStream.ToArray();

                    memoryStream.Close();
                    pdfDoc.Close();
                }
            }

            return pdf;
        }
    }
}
