namespace DocumentService.Tests.Common.Services
{
    using DocumentService.Azure;
    using DocumentService.Contexts;
    using DocumentService.Models;
    using DocumentService.ServiceModels;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using Xunit;

    [CollectionDefinition("Database collection")]
    public class DatabaseFixture : IDatabaseFixture
    {
        public DocumentContext Context { get; set; }

        private IConfiguration configuration;

        public DatabaseFixture()
        {
            var testConfigurationBuilder = new TestConfigurationBuilder();
            this.configuration = testConfigurationBuilder.Build();
            var azureKeyVault = new AzureKeyVaultService(configuration);
            this.Context = new DocumentContext(this.configuration, azureKeyVault);
            this.Context.Database.EnsureCreated();
        }
     

        public List<Document> CreateDocument(int count, Guid guid)
        {
            List<Document> documents = new List<Document>();

            for(int i = 0; i < count; i++)
            {
                documents.Add(new Document
                {
                    DocumentId = guid,
                    FileName = $"Test {i}",
                    Description = $"Test Description {i}",
                    FileSize = i,
                    Language = "EN",
                    RequesterId = $"Tester {i}",
                    UserCreatedById = $"Tester {i}",
                    DocumentTypes = new List<DocumentType> {  new DocumentType { Description = $"Type {i}", Id = i.ToString() } }
                });
            }
          
            return documents;
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
                    DocumentTypes = new List<DocumentType> { new DocumentType { Description = "Test", Id = "0" } },
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

        public void InsertDocument(Document document, Guid id)
        {
            var documentInfo = CreateUpdatedDocumentInfo(document, id);
            this.Context.DocumentInfo.Add(documentInfo);
            this.Context.SaveChanges();
        }

        private DocumentInfo CreateUpdatedDocumentInfo(Document document, Guid id)
        {
            var documentInfo = new DocumentInfo
            {
                DocumentId = id,
                FileName = document.FileName,
                UserCreatedById = document.UserCreatedById,
                Description = document.Description,
                DocumentTypes = document.DocumentTypes,
                FileSize = document.FileSize,
                Language = document.Language
            };
            return documentInfo;
        }
        public void Dispose()
        {
            this.Context.Database.EnsureDeleted();
        }
    }
}
