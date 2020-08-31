using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core.Utils
{
    public class MSSpreadsheetWriterBlob : IMSSpreadsheetWriter
    {
        public async Task<string> WriteAsync(Guid instanceID, string FileName, IWorkbook workbook)
        {
            var ms = new CotorraMemoryStream
            {
                AllowClose = false
            };
            workbook.Write(ms);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            ms.AllowClose = true;
            var blobStorageUtil = new BlobStorageUtil(instanceID);
            await blobStorageUtil.InitializeAsync();
            await blobStorageUtil.UploadDocumentAsync(FileName, ms);
            var url = blobStorageUtil.GetBlobSasUri(instanceID, FileName);
            ms.Close();
            return url;
        }
    }
}
