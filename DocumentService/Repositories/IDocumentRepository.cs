using DocumentService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentService.Repositories
{
    public interface IDocumentRepository
    {
        Task<DocumentInfo> GetDocumentAsync(Guid id);
        IEnumerable<DocumentInfo> GetDocumentsByIds(Guid[] ids);
        Task<bool> SetFileDeleted(Guid id, string userId);
        Task<bool> Update(DocumentInfo documentInfo);
        Task<int> UploadDocumentAsync(DocumentInfo documentInfo);
    }
}