using DocumentService.Contexts;
using DocumentService.Models;
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
        public async Task<int> UploadDocumentAsync(DocumentDTO documentDTO) 
        {
            int numberOfEntitiesUpdated;

            if (documentDTO == null)
            {
                throw new NullReferenceException("DocumentInfo cannot be null");
            }

            try
            {
                List<DocumentInfo> documentInfo = PopulateDocumentInfo(documentDTO);

                return numberOfEntitiesUpdated = await SaveChanges(documentInfo);
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
                List<Document> documents = new List<Document>();
                if (documentInfo != null)
                {
                    Document document = new Document
                    {
                        DocumentId = documentInfo.DocumentId,
                        FileName = documentInfo.FileName,
                        DocumentType = documentInfo.DocumentTypes,
                        DocumentSize = documentInfo.FileSize,
                        Description = documentInfo.Description,
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
                    documents.Add(document);
                }
                DocumentDTO documentDTO = new DocumentDTO
                {
                    Documents = documents
                };

                if (documentDTO != null)
                {
                    return documentDTO;
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
        public async Task<bool> Update(DocumentDTO documentDTO, Guid id)
        {
            var updatedDocumentInfo = await this.GetDocument(id);
            try
            {
                if (updatedDocumentInfo != null && documentDTO.Documents.First() != null)
                {
                    updatedDocumentInfo.UserLastUpdatedById = documentDTO.Documents.First().RequesterId;
                    updatedDocumentInfo.FileName = documentDTO.Documents.First().FileName;
                    updatedDocumentInfo.FileType = documentDTO.Documents.First().FileType;
                    updatedDocumentInfo.Description = documentDTO.Documents.First().Description;
                    updatedDocumentInfo.SubmissionMethod = "";
                    updatedDocumentInfo.Language = documentDTO.Documents.First().Language;
                    updatedDocumentInfo.DocumentTypes = documentDTO.Documents.First().DocumentType;
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
      
        private List<DocumentInfo> PopulateDocumentInfo(DocumentDTO documentDTO)
        {
            List<DocumentInfo> documentInfos = new List<DocumentInfo>();
          
            foreach (var documents in documentDTO.Documents)
            {
                DocumentInfo documentInfo = new DocumentInfo
                {
                    CorrelationId = documentDTO.CorrelationId,
                    FileName = documents.FileName,
                    FileSize = documents.DocumentSize,
                    DocumentTypes = documents.DocumentType,
                    Description = documents.Description,
                    // DocumentUrl = GetUrlFromBlob(DocumentObject)
                    Language = documents.Language,
                    UserCreatedById = documents.RequesterId,
                    DateCreated = DateTime.UtcNow,
                    IsDeleted = false,
                    SubmissionMethod = "method"
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

