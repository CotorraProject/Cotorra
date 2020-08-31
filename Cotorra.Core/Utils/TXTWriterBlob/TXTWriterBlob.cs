using Cotorra.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core
{
    public class TXTWriterBlob
    {
        public async Task<string> WriteAsync(Guid instanceID, string FileName, string txtContent)
        {
            var blobStorageUtil = new BlobStorageUtil(instanceID);
            await blobStorageUtil.InitializeAsync();
            await blobStorageUtil.UploadDocumentAsync(FileName, txtContent);
            var url = blobStorageUtil.GetBlobSasUri(instanceID, FileName);
            return url;
        }
    }
}
