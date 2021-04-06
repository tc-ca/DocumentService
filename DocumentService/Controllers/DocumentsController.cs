using DocumentService.Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DocumentService.Controllers
{
    [Route("api/")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IAzureBlobService azureBlobService;

        private readonly IConfiguration configuration;


        public DocumentsController(IAzureBlobService azureBlobService, IConfiguration configuration)
        {
            this.azureBlobService = azureBlobService;

            this.configuration = configuration;
        }

        [HttpPost]
        [Route("v1/documents/test")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UploadDocumentTest(IFormFile file)
        {
            
            var result = this.azureBlobService.UploadFileAsync(file, configuration.GetSection("BlobContainers")["Documents"]).GetAwaiter().GetResult();

            return Ok(new { fileName = result.Name, url = result.Uri.AbsoluteUri  });
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
        public IActionResult UploadDocument(int CorrelationId, string UserName, string FileName, int FileSize, string FileContentType, string ShortDescription, string SubmissionMethod, int? FileLanguage, List<string>? DocumentTypes, string CustomMetadata, string Bytes)
        {

            return Ok();
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
        public IActionResult GetAllSpecifiedDocuments(string ListOfIds)
        {
            // Note: We can't pass an array, so I added a string but will be comma seperated

            var list = ListOfIds.Split(",").ToList().Select(int.Parse).ToList();

            return Ok(new { list });

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
        public IActionResult UpdateMetadataForDocument(int CorrelationId, string UserName, string FileName, string FileContentType, string ShortDescription, string SubmissionMethod, int? FileLanguage, string DocumentTypes, string CustomMetadata)
        {
            return Ok();
        }

        /// <summary>
        /// Retrieve a document by supplying its identifier.
        /// </summary>
        /// <param name="Id">Identifier of the document being retrieved.</param>
        /// <returns>Document specificed</returns>
        [HttpGet]
        [Route("v1/documents/{CorrelationId}/{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetDocumentById(int Id)
        {
            return Ok(new { Id });
        }

        /// <summary>
        /// Deletes the identified document.
        /// </summary>
        /// <param name="CorrelationId">Correlation identifier of the operation. </param>
        /// <param name="Id">Identifier of the document being deleted.</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("v1/documents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteDocumentById(int CorrelationId, int Id)
        {
            return Ok(new { Id });
        }

    }
}
