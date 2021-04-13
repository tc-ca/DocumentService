using DocumentService.Azure;
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
            var azureKeyVault = new AzureKeyVaultService(configuration);
            this.Context = new DocumentContext(this.configuration, azureKeyVault);
            this.Context.Database.EnsureCreated();
        }
     

        public DocumentDTO CreateDTOWithCorId(int count, Guid guid)
        {
            DocumentDTO documentDTO = new DocumentDTO();
          
            List<Document> documents = new List<Document>();
         

            for(int i = 0; i < count; i++)
            {
                documents.Add(new Document
                {
                    DocumentId = guid,
                    FileName = $"Test {i}",
                    Description = $"Test Description {i}",
                    DocumentSize = i,
                    Language = "EN",
                    RequesterId = $"Tester {i}",
                    UserCreatedById = $"Tester {i}",
                    DocumentType = new DocumentTypes { DocumentType = $"Type {i}", DocumentTypesId = i}
                });
            }
          //  this.Context.Correlation.Add(correlation);
           
            documentDTO.CorrelationId = Guid.Empty;
            documentDTO.Documents = documents;
          
            return documentDTO;
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
                    DocumentTypes = new DocumentTypes { DocumentType = "Test", DocumentTypesId = 0 },
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

        public void InsertDocumentDTO(DocumentDTO documentDTO, Guid id)
        {
            var documentInfo = CreateUpdatedDocumentInfo(documentDTO, id);
            this.Context.DocumentInfo.Add(documentInfo);
            this.Context.SaveChanges();
        }

        private DocumentInfo CreateUpdatedDocumentInfo(DocumentDTO documentDTO, Guid id)
        {
            var document = new DocumentInfo
            {
                DocumentId = id,
                FileName = documentDTO.Documents[0].FileName,
                UserCreatedById = documentDTO.Documents[0].UserCreatedById,
                Description = documentDTO.Documents[0].Description,
                DocumentTypes = documentDTO.Documents[0].DocumentType,
                FileSize = documentDTO.Documents[0].DocumentSize,
                Language = documentDTO.Documents[0].Language
            };
            return document;
        }
        public void Dispose()
        {
            this.Context.Database.EnsureDeleted();
        }
    }
}
