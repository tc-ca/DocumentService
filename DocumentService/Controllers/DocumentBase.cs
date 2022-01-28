using DocumentService.Azure;
using DocumentService.Models;
using DocumentService.Repositories;
using DocumentService.ServiceModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DocumentService.Controllers
{
    public class DocumentBase : ControllerBase, IDocumentsController
    {
        private readonly IDocumentRepository documentRepository;
        private readonly IAzureBlobService azureBlobService;
        private readonly IConfiguration configuration;

        public DocumentBase(IDocumentRepository documentRepository, IAzureBlobService azureBlobService, IConfiguration configuration)
        {
            this.documentRepository = documentRepository;
            this.azureBlobService = azureBlobService;
            this.configuration = configuration;
        }

        /// <summary>
        /// Get the current environment
        /// </summary>
        /// <returns>the current working environment</returns>
        /// <response code="200">Returns the environment</response>
        /// <response code="400">Returns bad request</response>
        [HttpGet]
        [Route("v1/documents/environment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetEnvironment()
        {
            var word = string.Format("Environment variable is {0}, which means {1}.", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), configuration.GetSection("Env").Value);
            return Ok(word);
        }
        /// <summary>
        /// Initial testing endpoint
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <response code="200">Returns the added file</response>
        /// <response code="400">returns bad request</response>
        [HttpPost]
        [Route("v1/documents/testing")]
        [RequestFormLimits(MultipartBodyLengthLimit = 524288000)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadTestAsync(IFormFile file)
        {
            var uploadFileParameters = new UploadFileParameters()
            {
                FileName = file.FileName,
                FileStream = file.OpenReadStream(),
                Container = "testing"
            };

            var res = await azureBlobService.UploadFileAsync(uploadFileParameters);

            return Ok(new { result = file.Name, res });
        }

        public IActionResult UploadDocument([FromBody] UploadedDocumentsDTO uploadedDocumentsDTO)
        {
            // Create Memory stream here
            var stream = new MemoryStream();
            stream.Write(uploadedDocumentsDTO.FileBytes, 0, uploadedDocumentsDTO.FileBytes.Length);
            stream.Position = 0;
            string documentUrl = string.Empty;
            var uploadFileParameters = new UploadFileParameters()
            {
                FileName = uploadedDocumentsDTO.FileName,
                FileStream = stream,
                Container = configuration.GetSection("BlobContainers")["Documents"]
            };

            try
            {
                var result = this.azureBlobService.UploadFileAsync(uploadFileParameters).GetAwaiter().GetResult();
                documentUrl = result.Uri.AbsoluteUri;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

            var document = this.populateDocumentFromUploadedDocumentsDTO(uploadedDocumentsDTO, documentUrl);
            var uploadedDocument = this.documentRepository.UploadDocumentAsync(document).Result;
            return Ok(uploadedDocument);
        }


        public IActionResult UpdateMetadataForDocument([FromBody] UpdateMetaDataDTO updateMetaDataDTO)
        {
            if (updateMetaDataDTO.DocumentId == Guid.Empty)
            {
                return new BadRequestObjectResult("DocumentId is required for updating the document");
            }

            var document = new Document()
            {
                DocumentId = updateMetaDataDTO.DocumentId,
                UserLastUpdatedById = updateMetaDataDTO.UserName,
                DateLastUpdated = DateTime.UtcNow,
                FileName = updateMetaDataDTO.FileName,
                Description = updateMetaDataDTO.Description,
                SubmissionMethod = updateMetaDataDTO.Description,
                Language = updateMetaDataDTO.FileLanguage,
                DocumentTypes = updateMetaDataDTO.DocumentTypes
            };

            try
            {
                var isUpdated = this.documentRepository.Update(document).Result;
                return new JsonResult(isUpdated);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e);
            }
        }

        public IActionResult GetDocumentById(Guid id)
        {
            var document = this.documentRepository.GetDocumentAsync(id).Result;
            if (document != null)
            {
                return Ok(document);
            }
            return BadRequest(string.Format("Couldn't find document with id: {0}", id));
        }

        public IActionResult GetAllSpecifiedDocuments([FromQuery] List<Guid> documentGuid)
        {
            var documents = this.documentRepository.GetDocumentsByIds(documentGuid);
            return Ok(documents);
        }

        [HttpDelete]
        [Route("v1/documents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteDocumentById([FromQuery] Guid id, string userName)
        {
            try
            {
                var isDeleted = this.documentRepository.SetFileDeleted(id, userName).Result;
                if (isDeleted)
                {
                    return new JsonResult(isDeleted);
                }
                else
                {
                    return new NotFoundResult();
                }
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e);
            }
        }
        
        public IActionResult GetFileByDocumentId(Guid id)
        {
            var document = this.documentRepository.GetDocumentAsync(id).Result;
            var azureDownloadLink = this.azureBlobService.GetDownloadLinkAsync("documents", document.DocumentUrl, DateTime.UtcNow.AddHours(8), false).Result;
            return Ok(azureDownloadLink);
        }
       
        [HttpGet]
        [Route("v1/documents/viewlink/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetFileViewLinkByDocumentId(Guid id)
        {
            var document = this.documentRepository.GetDocumentAsync(id).Result;
            var azureDownloadLink = this.azureBlobService.GetDownloadLinkAsync("documents", document.DocumentUrl, DateTime.UtcNow.AddHours(8), true).Result;
            return Ok(azureDownloadLink);
        }

        private Document populateDocumentFromUploadedDocumentsDTO(UploadedDocumentsDTO uploadedDocumentsDTO, string documentUrl)
        {
            return new Document()
            {
                UserCreatedById = uploadedDocumentsDTO.UserName,
                DateCreated = DateTime.Now,
                DocumentUrl = documentUrl,
                FileName = uploadedDocumentsDTO.FileName,
                FileSize = uploadedDocumentsDTO.FileSize,
                FileType = uploadedDocumentsDTO.FileContentType,
                Description = uploadedDocumentsDTO.ShortDescription,
                SubmissionMethod = uploadedDocumentsDTO.SubmissionMethod,
                Language = uploadedDocumentsDTO.FileLanguage,
                DocumentTypes = uploadedDocumentsDTO.DocumentTypes,
            };
        }
    }
}
