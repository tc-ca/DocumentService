namespace DocumentService.Azure
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.Storage.Blob;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the Azure blob service.
    /// </summary>
    public class AzureBlobService : IAzureBlobService
    {
        private readonly IAzureBlobConnectionFactory azureBlobConnectionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobService"/> class.
        /// </summary>
        /// <param name="azureBlobConnectionFactory">The Azure blob connection factory.</param>
        public AzureBlobService(IAzureBlobConnectionFactory azureBlobConnectionFactory)
        {
            this.azureBlobConnectionFactory = azureBlobConnectionFactory;
        }

        /// <inheritdoc/>
        public async Task<CloudBlockBlob> UploadFileAsync(IFormFile file, string container = null)
        {
            // Perhaps we can fail more gracefully then just throwing an exception
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var blobContainer = await this.azureBlobConnectionFactory.GetBlobContainer().ConfigureAwait(false);

            var blobName = AzureBlobService.UniqueFileName(file.FileName);

            // Create the blob to hold the data
            var blob = blobContainer.GetBlockBlobReference(blobName);

            // Send the file to the cloud storage
            using (var stream = file.OpenReadStream())
            {
                await blob.UploadFromStreamAsync(stream).ConfigureAwait(false);
            }

            return blob;
        }

        public async Task<string> GetDownloadLinkAsync(string container, string fileUrl, DateTime expiryTime)
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
            SharedAccessBlobHeaders headers = new SharedAccessBlobHeaders()
            {
                ContentDisposition = string.Format("attachment;filename=\"{0}\"", fileName),
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
    }
}
