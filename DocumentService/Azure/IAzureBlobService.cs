namespace DocumentService.Azure
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.Storage.Blob;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for the Azure blob service storage.
    /// </summary>
    public interface IAzureBlobService
    {
        /// <summary>
        /// Uploads a single file to the azure blob storage.
        /// </summary>
        /// <param name="file">The file to upload.</param>
        /// <param name="container">The container to connect to..</param>
        /// <returns>The uploaded blob.</returns>
        Task<CloudBlockBlob> UploadFileAsync(IFormFile file, string container = null);

        /// <summary>
        /// Get the file download link
        /// </summary>
        /// <param name="container">The container to connect to..</param>
        /// <param name="fileUrl">File url in blob storage</param>
        /// <param name="expiryTime">The token used to access the file for read only (download)</param>
        /// <returns></returns>
        Task<string> GetDownloadLinkAsync(string container, string fileUrl, DateTime expiryTime);
    }
}
