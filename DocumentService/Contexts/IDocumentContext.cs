using DocumentService.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentService.Contexts
{
    public interface IDocumentContext
    {
        DbSet<Document> Document { get; set; }
        DbSet<Correlation> Correlation { get; set; }
        DbSet<DocumentInfo> DocumentInfo { get; set; }
    }
}