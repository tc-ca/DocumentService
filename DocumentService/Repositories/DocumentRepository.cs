using DocumentService.Contexts;
using DocumentService.Models;
using DocumentService.Repositories.Entities;
using Microsoft.AspNetCore.Http;
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
        
        public async Task<bool> GetCorrelationById(Guid id)
        {
            var query = context.Correlation.Where(x => x.CorrelationId == id);
            return await query.AnyAsync();
        }
        
        /// <inheritdoc/>
        public async Task<List<Guid>> UploadDocumentAsync(DocumentDTO documentDTO) 
        {
            if (documentDTO == null)
            {
                throw new NullReferenceException("DocumentInfo cannot be null");
            }

            try
            {
                List<DocumentInfo> documentInfo = PopulateDocumentInfo(documentDTO);
                var numberOfEntitiesUpdated = await SaveChanges(documentInfo);
                List<Guid> ids = documentInfo.Select(x => x.DocumentId).ToList();
                return ids;
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<DocumentDTO> GetDocumentAsync(Guid id)
        {
            try
            {
                DocumentInfo documentInfo = await this.GetDocument(id);
                if (documentInfo != null)
                {
                    Document document = new Document
                    {
                        DocumentId = documentInfo.DocumentId,
                        FileName = documentInfo.FileName,
                        DocumentType = documentInfo.DocumentTypes,
                        DocumentSize = documentInfo.FileSize,
                        Description = documentInfo.Description,
                        SubmissionMethod  = documentInfo.SubmissionMethod,
                        FileType = documentInfo.FileType,
                        Language = documentInfo.Language,
                        UserCreatedById = documentInfo.UserCreatedById,
                        UserLastUpdatedById = documentInfo.UserLastUpdatedById,
                        RequesterId = documentInfo.UserCreatedById,
                        DeletedById = documentInfo.DeletedById,
                        DateCreated = documentInfo.DateCreated,
                        DateDeleted = documentInfo.DateDeleted,
                        DateLastUpdated = documentInfo.DateLastUpdated
                    };
                    DocumentDTO documentDTO = new DocumentDTO
                    {
                        Documents = new List<Document> {  document }
                    };

                    return documentDTO;
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
        public async Task<IEnumerable<DocumentUpdatedResult>> Update(DocumentDTO documentDTO)
        {
            var documentUpdatedResults = new List<DocumentUpdatedResult>();
            try
            {
                foreach (var documentInfo in documentDTO.Documents)
                {
                    var updatedDocumentInfo = await this.GetDocument(documentInfo.DocumentId);
                    if (updatedDocumentInfo != null)
                    {
                        updatedDocumentInfo.UserLastUpdatedById = string.IsNullOrEmpty(documentInfo.RequesterId) ? updatedDocumentInfo.UserLastUpdatedById : documentInfo.RequesterId;
                        updatedDocumentInfo.FileName = string.IsNullOrEmpty(documentInfo.RequesterId) ? updatedDocumentInfo.FileName : documentInfo.FileName;
                        updatedDocumentInfo.FileType = string.IsNullOrEmpty(documentInfo.FileType) ? updatedDocumentInfo.FileType : documentInfo.FileType;
                        updatedDocumentInfo.Description = string.IsNullOrEmpty(documentInfo.Description) ? updatedDocumentInfo.Description : documentInfo.Description;
                        updatedDocumentInfo.Language = string.IsNullOrEmpty(documentInfo.Language) ? updatedDocumentInfo.Language : documentInfo.Language;
                        updatedDocumentInfo.DocumentTypes = documentInfo.DocumentType == null ? updatedDocumentInfo.DocumentTypes : documentInfo.DocumentType;
                        updatedDocumentInfo.DateLastUpdated = DateTime.UtcNow;
                        updatedDocumentInfo.SubmissionMethod = string.IsNullOrEmpty(documentInfo.SubmissionMethod) ? updatedDocumentInfo.SubmissionMethod : documentInfo.SubmissionMethod;

                        this.context.DocumentInfo.Update(updatedDocumentInfo);
                        this.context.SaveChanges();
                        documentUpdatedResults.Add(new DocumentUpdatedResult()
                        {
                            IsUpdated = true,
                            DocumentId = documentInfo.DocumentId,
                        });
                    }
                    else
                    {
                        documentUpdatedResults.Add(new DocumentUpdatedResult()
                        {
                            IsUpdated = true,
                            DocumentId = documentInfo.DocumentId,
                        });
                    }
                }
                return documentUpdatedResults;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <inheritdoc/>
        public IEnumerable<DocumentInfo> GetDocumentsByIds(IEnumerable<Guid> ids)
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
      
        private List<DocumentInfo> PopulateDocumentInfo(DocumentDTO documentDTO)
        {
            List<DocumentInfo> documentInfos = new List<DocumentInfo>();
          
            foreach (var document in documentDTO.Documents)
            {
                var dateNow = DateTime.UtcNow;
                DocumentInfo documentInfo = new DocumentInfo
                {
                    FileName = document.FileName,
                    FileSize = document.DocumentSize,
                    DocumentTypes = document.DocumentType,
                    Description = document.Description,
                    DocumentUrl = document.DocumentUrl,
                    Language = document.Language,
                    UserCreatedById = document.RequesterId,
                    DateCreated = dateNow,
                    DateLastUpdated = dateNow,
                    IsDeleted = false,
                    SubmissionMethod = document.SubmissionMethod
                };
               
                documentInfos.Add(documentInfo);
            }
            return documentInfos;
        }
        private async Task<int> SaveChanges(List<DocumentInfo> documentInfos)
        {
            int numberOfEntitiesUpdated;
            foreach (var documents in documentInfos)
            {
                this.context.DocumentInfo.Add(documents);
            }
            return numberOfEntitiesUpdated = await this.context.SaveChangesAsync();

        }
            
    }
}

