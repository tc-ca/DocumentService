using DocumentService.Contexts;
using DocumentService.Models;
using DocumentService.Repositories.Entities;
using DocumentService.ServiceModels;
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
        public async Task<Document> UploadDocumentAsync(Document document)
        {
            if (document == null)
            {
                throw new NullReferenceException("DocumentInfo cannot be null");
            }

            try
            {
                DocumentInfo documentInfo = PopulateDocumentInfo(document);
                this.context.DocumentInfo.Add(documentInfo);
                await this.context.SaveChangesAsync();
                document.DocumentId = documentInfo.DocumentId;
                return document;
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Document> GetDocumentAsync(Guid id)
        {
            try
            {
                DocumentInfo documentInfo = await this.GetDocument(id);
                if (documentInfo != null)
                {
                    return this.populateDocument(documentInfo);
                }
                else { return null; }
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
                    this.context.SaveChanges();
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
        public async Task<Document> Update(Document document)
        {
            var documentUpdatedResults = new List<DocumentUpdatedResult>();
            try
            {
                var updatedDocumentInfo = await this.GetDocument(document.DocumentId);
                if (updatedDocumentInfo != null)
                {
                    updatedDocumentInfo.UserLastUpdatedById = string.IsNullOrEmpty(document.RequesterId) ? updatedDocumentInfo.UserLastUpdatedById : document.RequesterId;
                    updatedDocumentInfo.FileName = string.IsNullOrEmpty(document.RequesterId) ? updatedDocumentInfo.FileName : document.FileName;
                    updatedDocumentInfo.Description = string.IsNullOrEmpty(document.Description) ? updatedDocumentInfo.Description : document.Description;
                    updatedDocumentInfo.Language = string.IsNullOrEmpty(document.Language) ? updatedDocumentInfo.Language : document.Language;
                    updatedDocumentInfo.DocumentTypes = document.DocumentTypes == null ? updatedDocumentInfo.DocumentTypes : document.DocumentTypes;
                    updatedDocumentInfo.DateLastUpdated = DateTime.UtcNow;
                    updatedDocumentInfo.SubmissionMethod = string.IsNullOrEmpty(document.SubmissionMethod) ? updatedDocumentInfo.SubmissionMethod : document.SubmissionMethod;

                    this.context.DocumentInfo.Update(updatedDocumentInfo);
                    this.context.SaveChanges();
                }

                return document;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Document> GetDocumentsByIds(IEnumerable<Guid> ids)
        {
            var documents = new List<Document>();

            var documentInfos = this.Filter(x => ids.Contains(x.DocumentId));
            foreach (var documentInfo in documentInfos)
            {
                Document document = this.populateDocument(documentInfo);
                documents.Add(document);
            }

            return documents;
        }

        /// <inheritdoc/>
        public IEnumerable<DocumentInfo> Filter(Expression<Func<DocumentInfo, bool>> predicate)
        {
            return context.DocumentInfo.Where(predicate);
        }

        private Document populateDocument(DocumentInfo documentInfo)
        {
            return new Document()
            {
                DocumentId = documentInfo.DocumentId,
                FileName = documentInfo.FileName,
                DocumentTypes = documentInfo.DocumentTypes,
                FileSize = documentInfo.FileSize,
                Description = documentInfo.Description,
                SubmissionMethod = documentInfo.SubmissionMethod,
                FileType = documentInfo.FileType,
                Language = documentInfo.Language,
                DocumentUrl = documentInfo.DocumentUrl,
                UserCreatedById = documentInfo.UserCreatedById,
                UserLastUpdatedById = documentInfo.UserLastUpdatedById,
                RequesterId = documentInfo.UserCreatedById,
                DeletedById = documentInfo.DeletedById,
                DateCreated = documentInfo.DateCreated,
                DateDeleted = documentInfo.DateDeleted,
                IsDeleted = documentInfo.IsDeleted,
                DateLastUpdated = documentInfo.DateLastUpdated
            };
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

        private DocumentInfo PopulateDocumentInfo(Document document)
        {
            var dateNow = DateTime.UtcNow;
            DocumentInfo documentInfo = new DocumentInfo
            {
                FileName = document.FileName,
                FileSize = document.FileSize,
                DocumentTypes = document.DocumentTypes,
                Description = document.Description,
                DocumentUrl = document.DocumentUrl,
                Language = document.Language,
                UserCreatedById = document.RequesterId,
                DateCreated = dateNow,
                DateLastUpdated = dateNow,
                IsDeleted = document.IsDeleted,
                SubmissionMethod = document.SubmissionMethod,
            };

            return documentInfo;
        }
    }
}