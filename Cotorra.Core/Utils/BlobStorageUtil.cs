using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CotorraNode.Common.Config;
using Microsoft.Extensions.Primitives;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core.Utils
{
    public class BlobStorageUtil
    {
        private readonly string _containerSubName;
        private readonly string _CotorriaBlobAccount;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        private BlobContainerClient _containerClient;
        private bool _containerExists;

        public BlobStorageUtil(Guid instanceID)
        {
            var _connectionString = ConfigManager.GetValue("CotorriaAzureBlobConnectionString");
            _CotorriaBlobAccount = ConfigManager.GetValue("CotorriaAzureBlobAccount");
            _containerSubName = $"Cotorria";

            _blobServiceClient = new BlobServiceClient(_connectionString);
            _containerName = $"{_containerSubName}{instanceID.ToString().ToLower()}";
        }

        public async Task InitializeAsync()
        {
            // Create the container and return a container client object
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            _containerExists = await containerClient.ExistsAsync();
            if (!_containerExists)
            {
                _containerClient = await _blobServiceClient.CreateBlobContainerAsync(_containerName);
            }
            else
            {
                _containerClient = containerClient;
            }
        }

        private Stream ConvertToStream(string strToConvert)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(strToConvert);
            MemoryStream stream = new MemoryStream(byteArray);
            return stream;
        }

        private Stream ConvertToStream(byte[] byteArray)
        {
            MemoryStream stream = new MemoryStream(byteArray);
            return stream;
        }

        public async Task UploadDocumentAsync(string fileName, string fileContent)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(fileName);

            // Open the file and upload its data
            using Stream uploadFileStream = ConvertToStream(fileContent);
            await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();
        }

        public async Task UploadDocumentAsync(string fileName, Stream uploadFileStream)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();
        }

        public async Task UploadDocumentAsync(string fileName, byte[] uploadFileArray)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(fileName);
            using Stream uploadFileStream = ConvertToStream(uploadFileArray);
            await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();
        }

        public async Task<bool> ExistsFile(string fileName)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(fileName);
            var exists = await blobClient.ExistsAsync();

            return exists;
        }

        public async Task<string> DownloadDocumentAsync(string fileName)
        {
            var content = String.Empty;
            if (_containerExists)
            {
                BlobClient blobClient = _containerClient.GetBlobClient(fileName);

                var exists = await blobClient.ExistsAsync();
                if (exists.Value)
                {
                    // Open the file and upload its data
                    BlobDownloadInfo download = await blobClient.DownloadAsync();

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await download.Content.CopyToAsync(memoryStream);
                        content = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
                        memoryStream.Close();
                    }
                }
            }

            return content;
        }

        public string GetBlobSasUri(Guid instanceID, string blobName, string policyName = null)
        {
            string sasBlobToken;
            string containerName = $"{_containerSubName}{instanceID.ToString().ToLower()}";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_CotorriaBlobAccount);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            // Get a reference to a blob within the container.
            // Note that the blob may not exist yet, but a SAS can still be created for it.
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            if (policyName == null)
            {
                // Create a new access policy and define its constraints.
                // Note that the SharedAccessBlobPolicy class is used both to define the parameters of an ad hoc SAS, and
                // to construct a shared access policy that is saved to the container's shared access policies.
                SharedAccessBlobPolicy adHocSAS = new SharedAccessBlobPolicy()
                {
                    // When the start time for the SAS is omitted, the start time is assumed to be the time when the storage service receives the request.
                    // Omitting the start time for a SAS that is effective immediately helps to avoid clock skew.
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                    Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create
                };

                // Generate the shared access signature on the blob, setting the constraints directly on the signature.
                sasBlobToken = blob.GetSharedAccessSignature(adHocSAS);

            }
            else
            {
                // Generate the shared access signature on the blob. In this case, all of the constraints for the
                // shared access signature are specified on the container's stored access policy.
                sasBlobToken = blob.GetSharedAccessSignature(null, policyName);
            }

            // Return the URI string for the container, including the SAS token.
            return blob.Uri + sasBlobToken;
        }
    }
}
