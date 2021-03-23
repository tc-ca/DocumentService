using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public DocumentsController()
        {

        }

        /// <summary>
        /// Upload a document.
        /// </summary>
        /// <param name="CorrelationId">Correlation identifier of the operation.</param>
        /// <param name="UserName">Azure AD identifier of the user uploading the document.</param>
        /// <param name="FileName">File name of the document.</param>
        /// <param name="ShortDescription">Short description of the document.</param>
        /// <param name="SubmissionMethod">Indicates how the file was submitted to Transport Canada. (“FAX”, “MAIL”, “EMAIL”).</param>
        /// <param name="FileLanguage">Language of document being uploaded. (1) English, (2) French.</param>
        /// <param name="FileType">Type of file being uploaded. (“PDF”) pdf file, (“IMG”) Image file, (“TXT”) text file.</param>
        /// <param name="DocumentTypes">Document type of the uploaded document. A document can have multiple types associated with it. The document type id is supplied by the client using the document service.</param>
        /// <param name="Bytes">Document being uploaded.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("v1/documents")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UploadDocument(int CorrelationId, string UserName, string FileName, string? ShortDescription, string? SubmissionMethod, int? FileLanguage, string? FileType, List<string>? DocumentTypes, string Bytes)
        {
            
            return Ok();
        }

        /// <summary>
        /// Retrieve all metadata for all specified documents. 
        /// </summary>
        /// <param name="CorrelationId">Correlation identifier of the operation.</param>
        /// <param name="ListOfIds">List of identifiers of the uploaded documents.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("v1/documents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAllSpecifiedDocuments(int CorrelationId, [FromBody] List<int> ListOfIds)
        {

            return Ok(new { id = CorrelationId, list = ListOfIds });

        }

        /// <summary>
        /// Updates metadata for the provided document identifier.
        /// </summary>
        /// <param name="CorrelationId">Correlation identifier of the operation.</param>
        /// <param name="UserName">Azure AD identifier of the user uploading the document.</param>
        /// <param name="FileName">File name of the document.</param>
        /// <param name="ShortDescription">Short description of the document.</param>
        /// <param name="SubmissionMethod">Indicates how the file was submitted to Transport Canada. (“FAX”, “MAIL”, “EMAIL”).</param>
        /// <param name="FileLanguage">Language of document being uploaded. (1) English, (2) French.</param>
        /// <param name="FileType">Type of file being uploaded. (“PDF”) pdf file, (“IMG”) Image file, (“TXT”) text file.</param>
        /// <param name="DocumentTypes">Document type of the uploaded document. A document can have multiple types associated with it. The document type id is supplied by the client using the document service.</param>
        /// <returns></returns>
        [HttpPut]
        [Route("v1/documents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult UpdateMetadataForDocument(int CorrelationId, string UserName, string FileName, string? ShortDescription, string? SubmissionMethod, int? FileLanguage, string? FileType, List<string>? DocumentTypes)
        {
            return Ok();
        }

        /// <summary>
        /// Retrieve a document by supplying its identifier.
        /// </summary>
        /// <param name="CorrelationId">Correlation identifier of the operation.</param>
        /// <param name="Id">Identifier of the document being retrieved.</param>
        /// <returns>Document specificed</returns>
        [HttpGet]
        [Route("v1/documents/{CorrelationId}/{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetDocumentById(int CorrelationId, int Id)
        {
            return Ok(new { CorrelationId, Id });
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
            return Ok(new { CorrelationId, Id });
        }

    }
}
