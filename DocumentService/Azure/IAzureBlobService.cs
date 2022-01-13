namespace DocumentService.Azure
{
    using DocumentService.Models;
    using global::Azure.Storage.Blobs;
    using Microsoft.AspNetCore.Http;
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
        /// <param name="uploadFileParameter">Upload File Parameters</param>
        /// <returns>The uploaded blob.</returns>
        Task<BlobClient> UploadFileAsync(UploadFileParameters uploadFileParameter);

        /// <summary>
        /// Get the file download link
        /// </summary>
        /// <param name="container">The container to connect to..</param>
        /// <param name="fileUrl">File url in blob storage</param>
        /// <param name="expiryTime">The token used to access the file for read only (download)</param>
        /// <returns></returns>
        Task<string> GetDownloadLinkAsync(string container, string fileUrl, DateTime expiryTime, bool isViewLink);
    }
}
