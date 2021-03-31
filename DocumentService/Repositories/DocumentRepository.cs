using DocumentService.Contexts;
using DocumentService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly DocumentContext _context;


        public DocumentRepository(DocumentContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Updates the database with a new document
        /// </summary>
        /// <param name="documentInfo">DocumentInfo object created in the controller</param>
        /// <returns>Number of entities added</returns>
        public async Task<int> UploadDocumentAsync(DocumentInfo documentInfo)
        {
            if (documentInfo == null)
            {
                throw new NullReferenceException("DocumentInfo cannot be null");
            }
            try
            {
                _context.DocumentInfo.Add(documentInfo);
                int result = await _context.SaveChangesAsync();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Function that returns the Document if found
        /// </summary>
        /// <param name="id">Id of the document you would like to find</param>
        /// <returns>Document object if found</returns>
        public async Task<DocumentInfo> GetDocumentAsync(Guid id)
        {
            try
            {
                DocumentInfo doc = await GetDocument(id);
                if (doc != null)
                {

                    return doc;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Sets the corresponding document's IsDeleted flag to true for soft delete functionality
        /// </summary>
        /// <param name="id">Id of the document to moify</param>
        /// <param name="userId">User who deleted the file</param>
        /// <returns>True if successful</returns>
        public async Task<bool> SetFileDeleted(Guid id, string userId)
        {
            try
            {
                DocumentInfo doc = await GetDocument(id);

                if (doc != null)
                {
                    doc.IsDeleted = true;
                    doc.DeletedById = userId;
                    doc.DateDeleted = DateTime.UtcNow;
                    _context.DocumentInfo.Update(doc);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Updates a document's metadata
        /// </summary>
        /// <param name="documentInfo">Document object with updated data</param>
        /// <returns>True if successful</returns>
        public async Task<bool> Update(DocumentInfo documentInfo)
        {
            var doc = await GetDocument(documentInfo.DocumentId);
            try
            {
                if (doc != null)
                {
                    doc.UserLastUpdatedById = documentInfo.UserLastUpdatedById;
                    doc.FileName = documentInfo.FileName;
                    doc.FileType = documentInfo.FileType;
                    doc.Description = documentInfo.Description;
                    doc.SubmissionMethod = documentInfo.SubmissionMethod;
                    doc.Language = documentInfo.Language;
                    doc.DocumentTypes = documentInfo.DocumentTypes;
                    doc.DateLastUpdated = DateTime.UtcNow;

                    _context.DocumentInfo.Update(doc);
                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Returns multiple documents based off a list of IDs
        /// </summary>
        /// <param name="ids">List of IDs to find</param>
        /// <returns>List of documents</returns>
        public IEnumerable<DocumentInfo> GetDocumentsByIds(Guid[] ids)
        {
            var query = _context.DocumentInfo.Where(x => ids.Contains(x.DocumentId));

            return query.AsEnumerable<DocumentInfo>();
        }
        /// <summary>
        /// Function to return a single document that is not set for deletion
        /// </summary>
        /// <param name="id">Id of document to find</param>
        /// <returns>Document found</returns>
        private async Task<DocumentInfo> GetDocument(Guid id)
        {
            var query = from q in _context.DocumentInfo
                        where q.DocumentId == id && !q.IsDeleted
                        select q;
            return await query.FirstOrDefaultAsync();

        }
    }
}

