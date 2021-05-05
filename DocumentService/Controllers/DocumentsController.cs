using DocumentService.Azure;
using DocumentService.Models;
using DocumentService.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DocumentService.Controllers
{
    [Route("api/")]
    [ApiController]
    public class DocumentsController : ControllerBase
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
        /// Upload a document.
        /// </summary>
        /// <param name="CorrelationId">Correlation identifier of the operation.</param>
        /// <param name="UserName">Azure AD identifier of the user uploading the document.</param>
        /// <param name="FileName">File name of the document.</param>
        /// <param name="FileSize">Size in bytes of the document.</param>
        /// <param name="FileContentType">MIME content type of the document. We only accept the following values: [“application/pdf”, “image/jpeg”, “image/png”, “text/plain”, “application/msword”, “application/vnd.openxmlformats-officedocument.wordprocessingml.document”].</param>
        /// <param name="ShortDescription">Short description of the document.</param>
        /// <param name="SubmissionMethod">Indicates how the file was submitted to Transport Canada. (“FAX”, “MAIL”, “EMAIL”).</param>
        /// <param name="FileLanguage">Language of document being uploaded. (1) English, (2) French.</param>
        /// <param name="DocumentTypes">Document type of the uploaded document. A document can have multiple types associated with it. The document type id is supplied by the client using the document service.</param>
        /// <param name="CustomMetadata">Document metadata specific to the program using the service.</param>
        /// <param name="Bytes">Document being uploaded.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/documents")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UploadDocument(int correlationId, string userName, IFormFile file, string fileContentType, string shortDescription, string submissionMethod, string fileLanguage, List<string> documentTypes, string customMetadata)
        {
            var result = this.azureBlobService.UploadFileAsync(file, configuration.GetSection("BlobContainers")["Documents"]).GetAwaiter().GetResult();

            var document = new Document()
            {
                UserCreatedById = userName,
                DateCreated = DateTime.Now,
                DocumentUrl = result.Uri.AbsoluteUri,
                FileName = file.FileName,
                FileSize = file.Length,
                FileType = fileContentType,
                Description = shortDescription,
                SubmissionMethod = submissionMethod,
                Language = fileLanguage,
                // DocumentTypes = documentTypes,
                // MetaData = customMetadata,
            };

            var dto = new DocumentDTO
            {
                Documents = new List<Document> { document }
            };

            var uploadedDocumentId = this.documentRepository.UploadDocumentAsync(dto).Result;
            return Ok(new { documentId = uploadedDocumentId });
        }

        /// <summary>
        /// Retrieve all metadata for all specified documents. 
        /// </summary>
        /// <param name="ListOfIds">List of identifiers of the uploaded documents. Should be like 1,2,3,4</param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/documents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAllSpecifiedDocuments(string idString)
        {
            var listofIds = idString.Split(",").ToList().Select(Guid.Parse).ToArray<Guid>();
            var documents = this.documentRepository.GetDocumentsByIds(listofIds);

            return Ok(documents.ToList());
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
        [HttpPut]
        [Route("v1/documents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult UpdateMetadataForDocument(Guid documentId, string userName, string fileName, string fileContentType, string shortDescription, string submissionMethod, string fileLanguage, string documentTypes)
        {
            var document  = new Document()
            {
                DocumentId = documentId,
                UserCreatedById = userName,
                DateLastUpdated = DateTime.UtcNow,
                FileName = fileName,
                Description = shortDescription,
                SubmissionMethod = submissionMethod,
                Language = fileLanguage,
            };
            var documentList = new List<Document>();
            documentList.Add(document);
            var documentDTO = new DocumentDTO()
            {
                Documents = documentList
            };

            try
            {
                var isUpdated = this.documentRepository.Update(documentDTO).Result;
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
        [HttpGet]
        [Route("v1/documents/{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetDocumentById(Guid id)
        {
            var document = this.documentRepository.GetDocumentAsync(id).Result;
            if (document != null)
            {
                return Ok(new { document });
            }
            return BadRequest();
        }

        /// <summary>
        /// Deletes the identified document.
        /// </summary>
        /// <param name="Id">Identifier of the document being deleted.</param>
        /// <returns></returns>
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
                } else
                {
                    return new NotFoundResult();
                }
            } 
            catch(Exception e)
            {
                return new BadRequestObjectResult(e);
            }
        }

    }
}
