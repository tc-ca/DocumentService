using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Azure
{
    public interface IAzureBlobService
    {

        BlobContainerClient GetBlobContainer(string container = null);

        Task<BlobContentInfo> UploadFileAsync(IFormFile file, string container = null);

    }
}
