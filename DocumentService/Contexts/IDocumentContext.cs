using DocumentService.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentService.Contexts
{
    public interface IDocumentContext
    {
        /// <summary>
        /// Gets or sets the correlation table
        /// </summary>
        DbSet<Correlation> Correlation { get; set; }
        /// <summary>
        /// Gets or sets the document info table
        /// </summary>
        DbSet<DocumentInfo> DocumentInfo { get; set; }
    }
}