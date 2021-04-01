using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Azure
{
    public interface IAzureBlobService
    {

        BlobServiceClient GetBlobClient();

        BlobContainerClient GetBlobContainer(string container = null);

        Task<BlobClient> UploadFileAsync(IFormFile file, string container = null);

    }
}
