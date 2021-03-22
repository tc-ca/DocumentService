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


        public DbSet<Document> Document { get; set; }
        public DbSet<Correlation> Correlation { get; set; }
        public DbSet<DocumentInfo> DocumentInfo { get; set; }
        public DocumentContext(IConfiguration configuration)
        {
            this.configuration = configuration;

        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseNpgsql(this.configuration.GetConnectionString("Postgsql"));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentInfo>().HasNoKey();
        }



    }
}
