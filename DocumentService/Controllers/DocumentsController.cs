namespace DocumentService.Controllers
{
    using DocumentService.Authorization;
    using DocumentService.Azure;
    using DocumentService.Models;
    using DocumentService.Repositories;
    using DocumentService.ServiceModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Identity.Web.Resource;
    using MimeTypes;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    [Authorize]
    [ApiController]
    [Route("api/")]
    [RequiredScope(RequiredScopesConfigurationKey = ScopePolicy.ReadWritePermission)]
    public class DocumentsController : ControllerBase, IDocumentsController
    {
        private readonly IDocumentRepository documentRepository;
        private readonly IAzureBlobService azureBlobService;
        private readonly IConfiguration configuration;

        public DocumentsController(IDocumentRepository documentRepository, IAzureBlobService azureBlobService, IConfiguration configuration)
        {
            this.documentRepository = documentRepository;
            this.azureBlobService = azureBlobService;
            this.configuration = configuration;
        }

        /// <summary>
        /// Get the current environment
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/documents/environment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetEnvironment()
        {
            var word = string.Format("Environment variable is {0}, which means {1}.", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), configuration.GetSection("Env").Value);
            return Ok(word);
        }

        [HttpPost]
        [Route("v1/documents/testing")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadTestAsync(IFormFile file)
        {
           var res =  await azureBlobService.UploadFileAsync(file, "testing");
            
            return Ok(new { result = file.Name, res });
        }

        /// <summary>
        /// Upload a document.
        /// </summary>
        /// <param name="uploadedDocumentsDTO">The uploaded documents data transfer object.</param>
        /// <returns></returns>
        [Authorize(Policy = RolePolicy.RoleAssignmentRequiredWriters)]
        [HttpPost]
        [Route("v1/documents")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UploadDocument([FromBody] UploadedDocumentsDTO uploadedDocumentsDTO)
        {
            // Create FormFile here
            var stream = new MemoryStream(uploadedDocumentsDTO.FileBytes);
            string documentUrl = string.Empty;
            IFormFile file = new FormFile(stream, 0, uploadedDocumentsDTO.FileBytes.Length, uploadedDocumentsDTO.FileName, uploadedDocumentsDTO.FileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = MimeTypeMap.GetMimeType(uploadedDocumentsDTO.FileName)
            };

            try
            {
                var result = this.azureBlobService.UploadFileAsync(file, configuration.GetSection("BlobContainers")["Documents"]).GetAwaiter().GetResult();
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

        /// <summary>
        /// Updates metadata for the provided document identifier.
        /// </summary>
        /// <param name="CorrelationId">Correlation identifier of the operation.</param>
        /// <param name="UserName">Azure AD identifier of the user uploading the document.</param>
        /// <param name="FileName">File name of the document.</param>
        /// <param name="FileContentType">MIME content type of the document. We only accept the following values: [“application/pdf”, “image/jpeg”, “image/png”, “text/plain”, “application/msword”, “application/vnd.openxmlformats-officedocument.wordprocessingml.document”].</param>
        /// <param name="ShortDescription">Short description of the document.</param>
        /// <param name="SubmissionMethod">Indicates how the file was submitted to Transport Canada. (“FAX”, “MAIL”, “EMAIL”, "EMER").</param>
        /// <param name="FileLanguage">Language of document being uploaded. (1) English, (2) French.</param>
        /// <param name="DocumentTypes">Document type of the uploaded document. A document can have multiple types associated with it. The document type id is supplied by the client using the document service.</param>
        /// <param name="CustomMetadata">Document metadata specific to the program using the service.</param>
        /// <returns></returns>
        [Authorize(Policy = RolePolicy.RoleAssignmentRequiredWriters)]
        [HttpPut]
        [Route("v1/documents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Retrieve a document by supplying its identifier.
        /// </summary>
        /// <param name="Id">Identifier of the document being retrieved.</param>
        /// <returns>Document specificed</returns>
        [Authorize(Policy = RolePolicy.RoleAssignmentRequiredReaders)]
        [HttpGet]
        [Route("v1/documents/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetDocumentById(Guid id)
        {
            var document = this.documentRepository.GetDocumentAsync(id).Result;
            if (document != null)
            {
                return Ok(document);
            }
            return BadRequest(string.Format("Couldn't find document with id: {0}", id));
        }

        /// <summary>
        /// Retrieve all metadata for all specified documents. 
        /// </summary>
        /// <param name="ListOfIds">List of identifiers of the uploaded documents. Should be like 1,2,3,4</param>
        /// <returns></returns>
        [Authorize(Policy = RolePolicy.RoleAssignmentRequiredReaders)]
        [HttpGet]
        [Route("v1/documents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAllSpecifiedDocuments([FromQuery] List<Guid> documentGuid)
        {
            var documents = this.documentRepository.GetDocumentsByIds(documentGuid);
            return Ok(documents);
        }

        /// <summary>
        /// Deletes the identified document.
        /// </summary>
        /// <param name="Id">Identifier of the document being deleted.</param>
        /// <returns></returns>
        [Authorize(Policy = RolePolicy.RoleAssignmentRequiredWriters)]
        [HttpDelete]
        [Route("v1/documents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteDocumentById(Guid id, string userName)
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

        [Authorize(Policy = RolePolicy.RoleAssignmentRequiredReaders)]
        [HttpGet]
        [Route("v1/documents/downloadlink/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetFileByDocumentId(Guid id)
        {
            var document = this.documentRepository.GetDocumentAsync(id).Result;
            var azureDownloadLink = this.azureBlobService.GetDownloadLinkAsync("documents", document.DocumentUrl, DateTime.UtcNow.AddHours(8), false).Result;
            return Ok(azureDownloadLink);
        }

        [Authorize(Policy = RolePolicy.RoleAssignmentRequiredReaders)]
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
