﻿namespace DocumentService.Azure
{
    using DocumentService.Models;
    using global::Azure.Storage.Blobs;
    using global::Azure.Storage.Blobs.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Extensions.Configuration;
    using MimeTypes;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the Azure blob service.
    /// </summary>
    public class AzureBlobService : IAzureBlobService
    {
        private IConfiguration configuration;

        private readonly string connectionString;

        private readonly IAzureBlobConnectionFactory azureBlobConnectionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobService"/> class.
        /// </summary>
        /// <param name="azureBlobConnectionFactory">The Azure blob connection factory.</param>
        public AzureBlobService(IConfiguration configuration, IAzureBlobConnectionFactory azureBlobConnectionFactory, IKeyVaultService azureKeyVaultService)
        {
            this.configuration = configuration;

            this.azureBlobConnectionFactory = azureBlobConnectionFactory;

            if (azureKeyVaultService != null)
            {
                this.connectionString = azureKeyVaultService.GetSecretByName(configuration.GetSection("ConnectionStrings")["AzureBlobStorage"]);
            }
        }
        /// <inheritdoc/>

        public async Task<BlobClient> UploadFileAsync(UploadFileParameters uploadFileParameter)
        {
            var blobName = UniqueFileName(uploadFileParameter.FileName);

            // Get a reference to the blob
            BlobClient blobClient = GetBlobContainer(uploadFileParameter.Container).GetBlobClient(blobName);

            var blobHttpHeader = new BlobHttpHeaders
            {
                ContentType = MimeTypeMap.GetMimeType(uploadFileParameter.FileName)
            };

            // Send the file to the cloud storage
           await blobClient.UploadAsync(uploadFileParameter.FileStream, blobHttpHeader);
          
            return blobClient;
        }
        
        public async Task<string> GetDownloadLinkAsync(string container, string fileUrl, DateTime expiryTime, bool isViewLink)
        {
            string ext = Path.GetExtension(fileUrl);
            Uri uri = new Uri(fileUrl);

            string fileName = Path.GetFileName(uri.LocalPath);

            var blobContainer = await azureBlobConnectionFactory.GetBlobContainer(container).ConfigureAwait(false);

            var blob = blobContainer.GetBlockBlobReference(fileName);

            //Create an ad-hoc Shared Access Policy with read permissions which will expire
            SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = expiryTime,
            };

            //Set content-disposition header for force download
            var viewLink = "inline";
            if(!isViewLink)
            {
                viewLink = "attachment";
            }
            SharedAccessBlobHeaders headers = new SharedAccessBlobHeaders()
            {
                ContentDisposition = string.Format("{0}", viewLink),
            };

            var sasToken = blob.GetSharedAccessSignature(policy, headers);
            return blob.Uri.AbsoluteUri + sasToken;
        }

        private static string UniqueFileName(string currentFileName)
        {
            string ext = Path.GetExtension(currentFileName);

            string nameWithNoExt = Path.GetFileNameWithoutExtension(currentFileName);

            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}{2}", nameWithNoExt, DateTime.UtcNow.Ticks, ext);
        }

        private BlobContainerClient GetBlobContainer(string container = null)
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

        private BlobServiceClient GetBlobClient()
        {
            // Create a BlobServiceClient object which will be used to create / obtain a container client
            var blobServiceClient = new BlobServiceClient(this.connectionString);
            return blobServiceClient;
        }
    }
}
