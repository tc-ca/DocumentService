namespace DocumentService.Controllers
{
    using DocumentService.Authorization;
    using DocumentService.Azure;
    using DocumentService.Models;
    using DocumentService.Repositories;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Identity.Web.Resource;
    using System;
    using System.Collections.Generic;

    [Authorize]
    [ApiController]
    [Route("api/Anonymous")]

    public class DocumentsAnonymousController : DocumentBase
    {
        public DocumentsAnonymousController(IDocumentRepository documentRepository, IAzureBlobService azureBlobService, IConfiguration configuration)
            : base(documentRepository, azureBlobService, configuration)
        {
        }

        /// <summary>
        /// Saves a document.
        /// </summary>
        /// <param name="uploadedDocumentsDTO">The uploaded documents data transfer object.</param>
        /// <returns>The uploaded document</returns>
        /// <response code="201">Returns the uploaded document</response>
        /// <response code="400">Returns bad request</response>
        [HttpPost]
        [Route("v1/documents")]
        [RequestFormLimits(MultipartBodyLengthLimit = 524288000)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult UploadDocument([FromBody] UploadedDocumentsDTO uploadedDocumentsDTO)
        {
            return base.UploadDocument(uploadedDocumentsDTO);
        }


        /// <summary>
        /// Updates metadata for the provided document identifier.
        /// </summary>
        /// <param name="updateMetaDataDTO">Object provided with updated document information</param>
        /// <returns>updated metadata for the document</returns>
        /// <response code="200">returns the updated document</response>
        /// <response code="400">Returns bad request</response>
        /// <response code="401">Returns Unauthorized</response>
        [HttpPut]
        [Route("v1/documents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public new IActionResult UpdateMetadataForDocument([FromBody] UpdateMetaDataDTO updateMetaDataDTO)
        {
            return base.UpdateMetadataForDocument(updateMetaDataDTO);
        }

        /// <summary>
        /// Retrieve a document by supplying its identifier.
        /// </summary>
        /// <param name="id">Identifier of the document being retrieved.</param>
        /// <returns>Document specificed</returns>
        /// <response code="200">Returns the specified document</response>
        /// <response code="400">Returns bad request</response>
        /// <response code="404">Returns not found</response>
        [HttpGet]
        [Route("v1/documents/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public new IActionResult GetDocumentById(Guid id)
        {
            return base.GetDocumentById(id);
        }

        /// <summary>
        /// Retrieve all metadata for all specified documents. 
        /// </summary>
        /// <param name="documentGuid">List of identifiers of the uploaded documents. Should be like 1,2,3,4</param>
        /// <returns>Returns all metadata for specified documents</returns>
        /// <response code="200">Returns metadata for specific document</response>
        /// <response code="400">Returns bad request</response>
        [HttpGet]
        [Route("v1/documents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public new IActionResult GetAllSpecifiedDocuments([FromQuery] List<Guid> documentGuid)
        {
            return GetAllSpecifiedDocuments(documentGuid);
        }

        /// <summary>
        /// Deletes the identified document.
        /// </summary>
        ///
        /// <returns>returns deleted confirmation</returns>
        /// <response code="200">Returns true or false if the document was deleted</response>
        /// <response code="400">Returns bad request</response>
        /// <response code="404">Returns not found</response>
        [HttpDelete]
        [Route("v1/documents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public new IActionResult DeleteDocumentById([FromQuery] Guid id, string userName)
        {
           return base.DeleteDocumentById(id, userName);
        }

        /// <summary>
        /// Returns a download link for the document at the specified Id
        /// </summary>
        /// <param name="id">Id of the document</param>
        /// <returns>Download link for the file</returns>
        /// <response code="200">Returns download link for document</response>
        /// <response code="400">Returns bad request</response>
        /// <response code="404">Returns not found</response>
        [HttpGet]
        [Route("v1/documents/downloadlink/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public new IActionResult GetFileByDocumentId(Guid id)
        {
            return base.GetFileByDocumentId(id);
        }

        /// <summary>
        /// Returns link to view the file at the specified Id
        /// </summary>
        /// <param name="id">Id of the document</param>
        /// <returns>Download link for the file</returns>
        /// <response code="200">Returns view link for document</response>
        /// <response code="400">Returns bad request</response>
        /// <response code="404">Returns not found</response>
        [HttpGet]
        [Route("v1/documents/viewlink/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public new IActionResult GetFileViewLinkByDocumentId(Guid id)
        {
            return base.GetFileViewLinkByDocumentId(id);
        }
    }
}
