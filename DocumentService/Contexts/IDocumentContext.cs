using DocumentService.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentService.Contexts
{
    public interface IDocumentContext
    {
        /// <summary>
        /// Gets or sets the document info table
        /// </summary>
        DbSet<DocumentInfo> DocumentInfo { get; set; }
    }
}