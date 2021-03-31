using DocumentService.Contexts;
using DocumentService.Models;
using DocumentService.Repositories;
using DocumentService.TestData;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;

namespace DocumentService.Unit.Tests
{
    public class UnitTest1
    {
        private readonly DocumentContext context;
        private readonly IConfiguration configuration;
        private readonly DocumentRepository documentRepository;
        public UnitTest1()
        {
            this.configuration = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
            context = new DocumentContext(configuration);
            documentRepository = new DocumentRepository(context);
            context.Database.EnsureDeleted();
            createTables();
            
        }
        [Fact]
        public void GetDocumentAsync_WhenExists_ReturnsDocumentInfo()
        {
          
            // Arrange
            var documentInfoId = Guid.NewGuid();

            DocumentInfo expectedResult = createTestEntry(documentInfoId);

            
            // Act 
            var result = documentRepository.GetDocumentAsync(documentInfoId).Result;

            // Assert
            Assert.Equal(expectedResult, result);
        }
        [Fact]
        public void GetDocumentAsync_WhenNotExists_ReturnsDocumentInfo()
        {
            // Arrange
            var documentInfoId = Guid.NewGuid();
            // Act
            var result = documentRepository.GetDocumentAsync(documentInfoId).Result;

            // Assert
            Assert.Null(result);
        }
        [Fact]
        public void UploadDocumentAsync_UploadSuccessful_ReturnsOne()
        {
            // Arrange
            var expectedResult = 1;
            var documentInfoId = Guid.NewGuid();
            DocumentInfo docInfo = new DocumentInfo
            {
                DocumentId = documentInfoId,
                DateCreated = DateTime.UtcNow,
                Description = "Generic Description",
                FileName = "Test Doc",
                DocumentTypes = new DocumentTypes { DocType = "Test", DocumentTypesId = 0 },
                IsDeleted = false
            };


            // Act
            var result = documentRepository.UploadDocumentAsync(docInfo).Result;
            
            // Assert
            Assert.Equal(expectedResult, result);
        }
        [Fact]
        public void UploadDocumentAsync_UploadFailed_ThrowsNullReferenceException()
        {
            // Assert
            Assert.ThrowsAsync<NullReferenceException>(() => documentRepository.UploadDocumentAsync(null));
                
        }
        [Fact]
        public void SetFileDeleted_SetDeletedSuccessful_ReturnsTrue()
        {
            // Arrange
            var expectedResult = true;
            var id = Guid.NewGuid();
            var testDoc = createTestEntry(id);
            // Act
            var result = documentRepository.SetFileDeleted(testDoc.DocumentId, "Tester").Result;

            // Assert
            Assert.Equal(expectedResult, result);
        }
        [Fact]
        public void SetFileDeleted_SetDeletedFailed_ReturnsFalse()
        {
            var expectedResult = false;
            var result = documentRepository.SetFileDeleted(Guid.Empty, "Tester").Result;

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Update_UpdateSuccessful_ReturnsTrue()
        {
            // Arrange
            var expectedResult = true;
            var documentId = Guid.NewGuid();
            var docinfo = createTestEntry(documentId);

            docinfo.FileName = "Our new file name";
            // Act
            var result = documentRepository.Update(docinfo).Result;

            Assert.Equal(expectedResult, result);

        }
        [Fact]
        public void Update_UpdateFailed_ReturnsFalse()
        {
            //Arrange
            var expectedResult = false;
            var documentInfo = new DocumentInfo
            {
                DocumentId = Guid.Empty
        };
            // Act
            var result = documentRepository.Update(documentInfo).Result;


            // Assert
            Assert.Equal(expectedResult, result);

        }

        [Fact]
        public void GetDocumentsByIds_EntriesFound_ReturnsListOfEntries()
        {

            // Arrange
            var testGuids = createMultipleGuids();
            var expectedResult = testGuids.Length;
            var testEntries = createTestEntries(testGuids);
            
            // Act
            var result = documentRepository.GetDocumentsByIds(testGuids);

            // Assert
            Assert.Equal(testEntries, result);
        }
        [Fact]
        public void GetDocumentsByIds_EntriesNotFound_ReturnsEmpty()
        {
            // Arrange
            var testGuids = createMultipleGuids();
            var expectedResult = testGuids.Length;

            // Act
            var result = documentRepository.GetDocumentsByIds(testGuids);
            
            // Assert
            Assert.Empty(result);

        }
            private DocumentInfo createTestEntry(Guid id)
        {
            DocumentInfo docInfo = new DocumentInfo
            {
                DocumentId = id,
                DateCreated = DateTime.UtcNow,
                Description = "Generic Description",
                FileName = "Test Doc",
                DocumentTypes = new DocumentTypes { DocType = "Test", DocumentTypesId = 0 },
                IsDeleted = false
            };
            context.DocumentInfo.Add(docInfo);
            context.SaveChanges();
            return docInfo;
        }
        private Guid[] createMultipleGuids()
        {
            var guids = new Guid[10];
            for(int i = 0; i <guids.Length; i++)
            {
                guids[i] = Guid.NewGuid();
            }
            return guids;
        }
        private IEnumerable<DocumentInfo> createTestEntries(Guid[] ids)
        {
            List<DocumentInfo> list = new List<DocumentInfo>();
            foreach (var id in ids)
            {
                DocumentInfo docInfo = new DocumentInfo
                {
                    DocumentId = id,
                    DateCreated = DateTime.UtcNow,
                    Description = "Generic Description",
                    FileName = "Test Doc",
                    DocumentTypes = new DocumentTypes { DocType = "Test", DocumentTypesId = 0 },
                    IsDeleted = false
                };
                context.DocumentInfo.Add(docInfo);
                list.Add(docInfo);
            }
            context.SaveChanges();
            return list;
        }
        
        private void createTables()
        {
            IDocumentContext documentContext = new DocumentContext(this.configuration);
            IDocumentsInitializer documentInitializer = new DocumentsInitializer();
            documentInitializer.context = (DocumentContext)documentContext;
            documentInitializer.Seed();

        }
    }

}
