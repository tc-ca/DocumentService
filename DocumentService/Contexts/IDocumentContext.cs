using DocumentService.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentService.Contexts
{
    public interface IDocumentContext
    {
       // DbSet<Document> Document { get; set; }
        DbSet<Correlation> CORRELATION { get; set; }
        DbSet<DocumentInfo> DOCUMENT_INFO { get; set; }
    }
}