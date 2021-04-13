using DocumentService.Contexts;
using DocumentService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DocumentService.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly DocumentContext context;


        public DocumentRepository(DocumentContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public async Task<int> UploadDocumentAsync(DocumentInfo documentInfo)
        {
            if (documentInfo == null)
            {
                throw new NullReferenceException("DocumentInfo cannot be null");
            }

            try
            {
                context.DocumentInfo.Add(documentInfo);
                int numberOfEntitiesUpdated = await this.context.SaveChangesAsync();
                return numberOfEntitiesUpdated;
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<DocumentInfo> GetDocumentAsync(Guid id)
        {
            try
            {
                DocumentInfo documentInfo = await this.GetDocument(id);
                if (documentInfo != null)
                {
                    return documentInfo;
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

        /// <inheritdoc/>
        public async Task<bool> SetFileDeleted(Guid id, string userId)
        {
            try
            {
                DocumentInfo documentInfo = await this.GetDocument(id);

                if (documentInfo != null)
                {
                    documentInfo.IsDeleted = true;
                    documentInfo.DeletedById = userId;
                    documentInfo.DateDeleted = DateTime.UtcNow;
                    this.context.DocumentInfo.Update(documentInfo);
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

        /// <inheritdoc/>
        public async Task<bool> Update(DocumentInfo documentInfo)
        {
            var updatedDocumentInfo = await this.GetDocument(documentInfo.DocumentId);
            try
            {
                if (updatedDocumentInfo != null)
                {
                    updatedDocumentInfo.UserLastUpdatedById = documentInfo.UserLastUpdatedById;
                    updatedDocumentInfo.FileName = documentInfo.FileName;
                    updatedDocumentInfo.FileType = documentInfo.FileType;
                    updatedDocumentInfo.Description = documentInfo.Description;
                    updatedDocumentInfo.SubmissionMethod = documentInfo.SubmissionMethod;
                    updatedDocumentInfo.Language = documentInfo.Language;
                    updatedDocumentInfo.DocumentTypes = documentInfo.DocumentTypes;
                    updatedDocumentInfo.DateLastUpdated = DateTime.UtcNow;

                    this.context.DocumentInfo.Update(updatedDocumentInfo);
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

        /// <inheritdoc/>
        public IEnumerable<DocumentInfo> GetDocumentsByIds(Guid[] ids)
        {
            return this.Filter(x => ids.Contains(x.DocumentId));
        }

        /// <inheritdoc/>
        public IEnumerable<DocumentInfo> Filter(Expression<Func<DocumentInfo, bool>> predicate)
        {
            return context.DocumentInfo.Where(predicate).AsEnumerable<DocumentInfo>();
        }

        /// <summary>
        /// Function to return a single document that is not set for deletion
        /// </summary>
        /// <param name="id">Id of document to find</param>
        /// <returns>Document found</returns>
        private async Task<DocumentInfo> GetDocument(Guid id)
        {
            var query = context.DocumentInfo.Where(x => x.DocumentId == id && !x.IsDeleted);
            return await query.FirstOrDefaultAsync();

        }
    }
}

