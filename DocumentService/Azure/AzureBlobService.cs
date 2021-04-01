using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
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

        public AzureBlobService(IKeyVaultService azureKeyVaultService)
        {
            if (azureKeyVaultService != null)
            {
                this.connectionString = azureKeyVaultService.GetSecretByName("BlobStorage");
            }
        }

        public async Task<BlobContentInfo> UploadFileAsync(IFormFile file, string container = null)
        {

            // Perhaps we can fail more gracefully then just throwing an exception
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            BlobContentInfo result = null;

            var blobName = UniqueFileName(file.FileName);

            using (var stream = file.OpenReadStream())
            {

                result = await GetBlobContainer(container).UploadBlobAsync(blobName, stream);

            }

            return result;
        }

        public BlobContainerClient GetBlobContainer(string container = null)
        {
            try
            {
                // Create the container and return a container client object
                containerClient = GetBlobClient().CreateBlobContainerAsync(container).GetAwaiter().GetResult().Value;

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

            string nameWithNoExt = Path.GetFileNameWithoutExtension(currentFileName);

            return string.Format(CultureInfo.InvariantCulture, "{1}{2}", Guid.NewGuid().ToString(), ext);
        }

        private BlobServiceClient GetBlobClient()
        {
            // Create a BlobServiceClient object which will be used to create a container client
            blobServiceClient = new BlobServiceClient(connectionString);

            return blobServiceClient;
        }
    }
}
