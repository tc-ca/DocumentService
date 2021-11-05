namespace DocumentService.Controllers
{
    using DocumentService.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDocumentsController
    {
        public IActionResult GetEnvironment();

        public IActionResult UploadDocument([FromBody] UploadedDocumentsDTO uploadedDocumentsDTO);

        public Task<IActionResult> GetFile([FromForm] IEnumerable<IFormFile> files);

        public IActionResult UpdateMetadataForDocument([FromBody] UpdateMetaDataDTO updateMetaDataDTO);

        public IActionResult GetDocumentById(Guid id);

        public IActionResult GetAllSpecifiedDocuments([FromQuery] List<Guid> documentGuid);

        public IActionResult DeleteDocumentById(Guid id, string userName);

        public IActionResult GetFileByDocumentId(Guid id);
    }
}
