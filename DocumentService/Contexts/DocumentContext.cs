using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DocumentService.Models;
using Microsoft.Extensions.Configuration;

namespace DocumentService.Contexts
{
    public class DocumentContext : DbContext, IDocumentContext
    {
        private IConfiguration configuration;


        /// <summary>
        /// Gets or sets the correlation table
        /// </summary>
        public DbSet<Correlation> Correlation { get; set; } 
        /// <summary>
        /// Gets or sets the document info table
        /// </summary>
        public DbSet<DocumentInfo> DocumentInfo { get; set; }
        public DocumentContext(IConfiguration configuration)
        {
            this.configuration = configuration;

        }
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseNpgsql(this.configuration.GetConnectionString("Postgsql"));
    }
}
