using DocumentService.Contexts;
using DocumentService.Models;
using DocumentService.Repositories;
using DocumentService.TestData;
using DocumentService.Unit.Tests.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;

namespace DocumentService.Unit.Tests
{
    [CollectionDefinition("Database collection")]
    public class DocumentServiceRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private DatabaseFixture databaseFixture;
        private readonly DocumentRepository documentRepository;
        
        public DocumentServiceRepositoryTests()
        {
            this.databaseFixture = new DatabaseFixture();
            this.documentRepository = new DocumentRepository(this.databaseFixture.Context);
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
            // Arrange
            var expectedResult = false;

            // Act
            var result = documentRepository.SetFileDeleted(Guid.Empty, "Tester").Result;

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Update_UpdateSuccessful_ReturnsTrue()
        {
            // Arrange
            var expectedResult = true;
            var documentId = Guid.NewGuid();
            var docinfo = createTestEntry(documentId);

            // Act
            docinfo.FileName = "Our new file name";
            var result = documentRepository.Update(docinfo).Result;

            // Assert
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
            var expectedResult = createTestEntries(testGuids);
            
            // Act
            var result = documentRepository.GetDocumentsByIds(testGuids);
            expectedResult = expectedResult.OrderBy(x => x.DocumentId);
            result = result.OrderBy(x => x.DocumentId);

            // Assert
            Assert.Equal(expectedResult, result);
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

        [Fact]
        public void Filter_WhenUsingFileName_ReturnsDocumentInfo()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var fileName = "John Wick";
            var expectedResult = new DocumentInfo()
            {
                DocumentId = documentId,
                FileName = fileName
            };
            this.databaseFixture.Context.DocumentInfo.Add(expectedResult);
            this.databaseFixture.Context.SaveChanges();

            // Act
            var result = this.documentRepository.Filter(x => x.FileName.Equals(fileName));

            // Assert
            Assert.Equal(expectedResult, result.FirstOrDefault());
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

            this.databaseFixture.Context.DocumentInfo.Add(docInfo);
            this.databaseFixture.Context.SaveChanges();
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

                this.databaseFixture.Context.DocumentInfo.Add(docInfo);
                list.Add(docInfo);
            }

            this.databaseFixture.Context.SaveChanges();
            return list;
        }
    }

}
