using DocumentService.Contexts;
using DocumentService.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Xunit;

namespace DocumentService.Unit.Tests.Services
{
    [CollectionDefinition("Database collection")]
    public class DatabaseFixture : IDatabaseFixture
    {
        public DocumentContext Context { get; set; }

        private IConfiguration configuration;

        public DatabaseFixture()
        {
            this.configuration = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
            this.Context = new DocumentContext(this.configuration);
            this.Context.Database.EnsureCreated();
        }

        public IEnumerable<DocumentInfo> CreateListOfDocumentInfos(Guid[] ids)
        {
            List<DocumentInfo> list = new List<DocumentInfo>();
            foreach (var id in ids)
            {
                DocumentInfo documentInfo = new DocumentInfo
                {
                    DocumentId = id,
                    DateCreated = DateTime.UtcNow,
                    Description = "Generic Description",
                    FileName = "Test Doc",
                    DocumentTypes = new DocumentTypes { DocType = "Test", DocumentTypesId = 0 },
                    IsDeleted = false
                };

                this.Context.DocumentInfo.Add(documentInfo);
                list.Add(documentInfo);
            }

            this.Context.SaveChanges();
            return list;
        }

        public void InsertDocumentInfo(DocumentInfo documentInfo)
        {
            this.Context.DocumentInfo.Add(documentInfo);
            this.Context.SaveChanges();
        }

        public void Dispose()
        {
            this.Context.Database.EnsureDeleted();
        }
    }
}
