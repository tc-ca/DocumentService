using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DocumentService.Models;
using Microsoft.Extensions.Configuration;
using DocumentService.Azure;

namespace DocumentService.Contexts
{
    public class DocumentContext : DbContext, IDocumentContext
    {
        private IConfiguration configuration;

        private readonly IAzureKeyVaultService azureKeyVaultService;

        /// <summary>
        /// Gets or sets the document info table
        /// </summary>
        public DbSet<DocumentInfo> DocumentInfo { get; set; }

        public DocumentContext(IConfiguration configuration, IAzureKeyVaultService azureKeyVaultService)
        {

            this.configuration = configuration;

            this.azureKeyVaultService = azureKeyVaultService;

            var db = this.azureKeyVaultService.GetSecretByName(configuration.GetSection("ConnectionStrings")["Postgsql"]);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseNpgsql(this.azureKeyVaultService.GetSecretByName(configuration.GetSection("ConnectionStrings")["Postgsql"]));
    }
}
