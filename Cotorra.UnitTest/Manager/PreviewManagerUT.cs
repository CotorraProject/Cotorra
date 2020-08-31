using Cotorra.Core.Managers.FiscalPreview;
using Cotorra.Core.Utils;
using Cotorra.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace Cotorra.UnitTest
{
    public class PreviewManagerUT
    {
        [Fact]
        public async Task PreviewXMLToPDF_ShouldGenerate()
        {
            var fiscalStampingVersion = Schema.FiscalStampingVersion.CFDI33_Nom12;
            var globalPath = Path.Combine(DirectoryUtil.AssemblyDirectory, "Fiscal", "cfdi33nom12", "preview");
            var xmlPath = Path.Combine(globalPath,"6b181142-78bb-443a-927f-e7b969cb8a36.xml");
            var xmlContent = await File.ReadAllTextAsync(xmlPath);

            var previewInstance = FiscalPreviewFactory.CreateInstance(fiscalStampingVersion);
            var transformationResult = await previewInstance.TransformAsync(new Schema.PreviewTransformParams()
            {
                FiscalStampingVersion = fiscalStampingVersion,
                PreviewTransformParamsDetails = new List<Schema.PreviewTransformParamsDetail>() { new Schema.PreviewTransformParamsDetail()
                    {
                        OverdraftID = Guid.Parse("A08D10B9-597F-4ECF-A2A4-5758D9AFDBF5"),
                        XML = xmlContent
                    } 
                },
                IdentityWorkID = Guid.Parse("0C08DAA6-F775-42A8-B75E-1B9B685B7977"),
                InstanceID = Guid.Parse("D0FBA45F-3F09-463F-8FB6-1035D8302ABC"),
                user = Guid.Parse("82CED4F6-A6CF-27E7-5683-95B6891B0B10")
            });

            var htmlPath = Path.Combine(DirectoryUtil.AssemblyDirectory, "6b181142-78bb-443a-927f-e7b969cb8a36.html");
            await File.WriteAllTextAsync(htmlPath, transformationResult.PreviewTransformResultDetails.FirstOrDefault().TransformHTMLResult);

            var pdfPath = Path.Combine(DirectoryUtil.AssemblyDirectory, "6b181142-78bb-443a-927f-e7b969cb8a36.pdf");
            await File.WriteAllBytesAsync(pdfPath, transformationResult.PreviewTransformResultDetails.FirstOrDefault().TransformPDFResult);
        }

    }
}
