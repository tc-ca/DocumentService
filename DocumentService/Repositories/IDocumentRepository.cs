namespace DocumentService.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using DocumentService.Models;
    using DocumentService.Repositories.Entities;
    using DocumentService.ServiceModels;

    public interface IDocumentRepository
    {
        /// <summary>
        /// Function that returns the Document if found
        /// </summary>
        /// <param name="id">Id of the document you would like to find</param>
        /// <returns>Document object if found</returns>
        Task<Document> GetDocumentAsync(Guid id);

        /// <summary>
        /// Returns multiple documents based off a list of IDs
        /// </summary>
        /// <param name="ids">List of IDs to find</param>
        /// <returns>List of documents</returns>
        IEnumerable<Document> GetDocumentsByIds(IEnumerable<Guid> ids);

        /// <summary>
        /// Returns a list of multiple <see cref="DocumentInfo"/> based on the predicate.
        /// </summary>
        /// <param name="predicate">The expression to filter by.</param>
        /// <returns>List of multiple <see cref="DocumentInfo"/></returns>
        IEnumerable<DocumentInfo> Filter(Expression<Func<DocumentInfo, bool>> predicate);

        /// <summary>
        /// Sets the corresponding document's IsDeleted flag to true for soft delete functionality
        /// </summary>
        /// <param name="id">Id of the document to moify</param>
        /// <param name="userId">User who deleted the file</param>
        /// <returns>True if successful</returns>
        Task<bool> SetFileDeleted(Guid id, string userId);

        /// <summary>
        /// Updates a document's metadata
        /// </summary>
        /// <param name="documentDTO">Document dto with updated data</param>
        /// <returns>True if successful</returns>
        Task<Document> Update(Document document);

        /// <summary>
        /// Updates the database with a new document
        /// </summary>
        /// <param name="documentDTO">Document DTO object created in the controller to populate the DocumentInfo entity</param>
        /// <returns>Number of entities added</returns>
        Task<Document> UploadDocumentAsync(Document document);
    }
}