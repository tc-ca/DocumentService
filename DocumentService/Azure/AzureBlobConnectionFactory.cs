namespace DocumentService.Azure
{
    using System.Threading.Tasks;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Blob;

    /// <summary>
    /// The Azure blob storage connection factor.
    /// </summary>
    public class AzureBlobConnectionFactory : IAzureBlobConnectionFactory
    {
        private readonly string connectionString;

        private CloudBlobClient cloudBlobClient;

        private CloudBlobContainer cloudBlobContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobConnectionFactory"/> class.
        /// </summary>
        /// <param name="azureKeyVaultService">Azure key vault service.</param>
        public AzureBlobConnectionFactory(IKeyVaultService azureKeyVaultService)
        {
            if (azureKeyVaultService != null)
            {
                this.connectionString = azureKeyVaultService.GetSecretByName("BlobStorageConnectionString");
            }
        }

        /// <inheritdoc/>
        public async Task<CloudBlobContainer> GetBlobContainer(string container = null)
        {
            var blobClient = this.GetClient();

            if (!string.IsNullOrWhiteSpace(container))
            {
                this.cloudBlobContainer = blobClient.GetContainerReference(container);
            }
            else
            {
                this.cloudBlobContainer = blobClient.GetRootContainerReference();
            }

            if (await this.cloudBlobContainer.CreateIfNotExistsAsync().ConfigureAwait(false))
            {
                await this.cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob,
                }).ConfigureAwait(false);
            }

            return this.cloudBlobContainer;
        }

        public CloudBlobClient GetClient()
        {
            // TODO : fail gracefully when connection string is invalid
            if (!CloudStorageAccount.TryParse(connectionString: this.connectionString, out CloudStorageAccount cloudStorageAccount))
            {
                throw new CloudStorageAccountConnectionStringException();
            }

            this.cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            return this.cloudBlobClient;
        }
    }
}
