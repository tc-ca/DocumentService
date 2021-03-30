using DocumentService.Contexts;
using DocumentService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Repositories
{
    public class DocumentRepository
    {
        private readonly DocumentContext _context;


        public DocumentRepository(DocumentContext context)
        {
            _context = context;
        }

        public async Task<int> UploadDocumentAsync(DocumentInfo documentInfo)
        {
         
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
        public async Task<DocumentInfo> GetDocumentAsync(Guid id)
        {
            try
            {
                IQueryable<DocumentInfo> query = from q in _context.DocumentInfo
                                                 where q.DocumentId == id
                                                 select q;
                if (await query.AnyAsync())
                {
                    DocumentInfo entity = await query.FirstAsync();
                    return entity;
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
        public IEnumerable<DocumentInfo> GetDocumentsByIds(Guid[] ids)
        {
            var query = _context.DocumentInfo.Where(x => ids.Contains(x.DocumentId));

            return query.AsEnumerable<DocumentInfo>();
        }

        private async Task<DocumentInfo> GetDocument(Guid id)
        {
           var query = from q in _context.DocumentInfo
                                             where q.DocumentId == id && !q.IsDeleted
                                             select q;
            return await query.FirstAsync();
            
        }
        }
    }

