using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using MimeTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Azure
{
    public class AzureBlobService : IAzureBlobService
    {

        private readonly string connectionString;

        private BlobServiceClient blobServiceClient;

        private BlobContainerClient containerClient;

        public AzureBlobService(IAzureKeyVaultService azureKeyVaultService)
        {
            if (azureKeyVaultService != null)
            {
                this.connectionString = azureKeyVaultService.GetSecretByName("BlobStorage");
            }
        }

        public async Task<BlobClient> UploadFileAsync(IFormFile file, string container = null)
        {

            try
            {
                // Perhaps we can fail more gracefully then just throwing an exception
                if (file == null)
                {
                    throw new ArgumentNullException(nameof(file));
                }

                /// var originalFileName = file.FileName;
                var blobName = UniqueFileName(file.FileName);

                // Get a reference to the blob
                BlobClient blobClient = GetBlobContainer(container).GetBlobClient(blobName);

                var blobHttpHeader = new BlobHttpHeaders
                {
                    ContentType = MimeTypeMap.GetMimeType(file.FileName)
                };

                using (var stream = file.OpenReadStream())
                {
                    // Upload the blob
                    var result = await blobClient.UploadAsync(stream, blobHttpHeader);

                }

                return blobClient;

            }
            catch (Exception e)
            {
                return null;
            }

        }

        public BlobItem GetBlob(string container, string fileUrl)
        {
            try
            {
                string ext = Path.GetExtension(fileUrl);

                Uri uri = new Uri(fileUrl);

                string fileName = Path.GetFileName(uri.LocalPath);

                var blobs = GetBlobContainer(container).GetBlobs();

                var blob = blobs.Where(x => x.Name.Equals(fileName)).Single();

                return blob;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public BlobContainerClient GetBlobContainer(string container = null)
        {
            try
            {

                //Get a BlobContainerClient
                var containerClient = GetBlobClient().GetBlobContainerClient(container);

                //Check if the container exists or not, then determine to create it or not
                bool isExist = containerClient.Exists();

                if (!isExist)
                {
                    containerClient.Create();
                }

                return containerClient;
            }
            catch (Exception e)
            {

                throw;
            }

        }

        private static string UniqueFileName(string currentFileName)
        {
            string ext = Path.GetExtension(currentFileName);

            return string.Format(CultureInfo.InvariantCulture, "{0}{1}", Guid.NewGuid().ToString(), ext);
        }

        public BlobServiceClient GetBlobClient()
        {
            // Create a BlobServiceClient object which will be used to create / obtain a container client
            blobServiceClient = new BlobServiceClient(connectionString);

            return blobServiceClient;
        }
    }
}
