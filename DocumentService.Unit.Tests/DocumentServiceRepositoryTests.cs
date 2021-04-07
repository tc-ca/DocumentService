using DocumentService.Models;
using DocumentService.Repositories;
using DocumentService.Unit.Tests.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
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
            DocumentInfo expectedResult = this.generateNewDocumentInfo(documentInfoId);
            this.databaseFixture.InsertDocumentInfo(expectedResult);

            // Act 
            var result = documentRepository.GetDocumentAsync(documentInfoId).Result;

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void GetDocumentAsync_WhenIsDeleted_ReturnsNull()
        {
            // Arrange
            var documentInfoId = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00");
            var fileName = "Test Document";
            var newDocumentInfo = new DocumentInfo()
            {
                DocumentId = documentInfoId,
                FileName = fileName,
                IsDeleted = true,
            };
            this.databaseFixture.InsertDocumentInfo(newDocumentInfo);

            // Act 
            var result = documentRepository.GetDocumentAsync(documentInfoId).Result;

            // Assert
            Assert.Null(result);
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
            var expectedResult = 5;
            DocumentDTO documentDTO = this.databaseFixture.CreateDTOWithCorId(expectedResult);

            var httpContext = new DefaultHttpContext();
            // Act
            var result = this.documentRepository.UploadDocumentAsync(documentDTO).Result;

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void UploadDocumentAsync_UploadFailed_ThrowsNullReferenceException()
        {
            var httpContext = new DefaultHttpContext();
            // Assert
            Assert.ThrowsAsync<NullReferenceException>(() => documentRepository.UploadDocumentAsync(null));
        }

        [Fact]
        public void SetFileDeleted_SetDeletedSuccessful_ReturnsTrue()
        {
            // Arrange
            var expectedResult = true;
            var documentInfoId = Guid.NewGuid();
            var testDocumentInfo = this.generateNewDocumentInfo(documentInfoId);
            this.databaseFixture.InsertDocumentInfo(testDocumentInfo);

            // Act
            var result = documentRepository.SetFileDeleted(testDocumentInfo.DocumentId, "Tester").Result;

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
            var documentInfoId = Guid.NewGuid();
            var documentDTO = this.databaseFixture.CreateDTOWithCorId(1);
            this.databaseFixture.InsertDocumentDTO(documentDTO, documentInfoId);
            HttpContext httpContext = new DefaultHttpContext();

            // Act
            documentDTO.Documents[0].FileName = "Our new file name";
            var result = documentRepository.Update(documentDTO, documentInfoId).Result;

            // Assert
            Assert.Equal(expectedResult, result);
        }
        [Fact]
        public void Update_UpdateFailed_ReturnsFalse()
        {
            //Arrange
            var expectedResult = false;
            HttpContext httpContext = new DefaultHttpContext();

            var documentInfo = new DocumentDTO();
            // Act
            var result = this.documentRepository.Update(documentInfo, Guid.Empty).Result;

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void GetDocumentsByIds_EntriesFound_ReturnsListOfEntries()
        {
            // Arrange
            var testGuids = this.createMultipleGuids();
            var expectedResult = this.databaseFixture.CreateListOfDocumentInfos(testGuids);

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
            var testGuids = this.createMultipleGuids();

            // Act
            var result = documentRepository.GetDocumentsByIds(testGuids);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Filter_WhenUsingFileName_ReturnsDocumentInfo()
        {
            // Arrange
            var documentId = new Guid("11113344-5566-7788-99AA-BBCCDDEEFF00");
            var fileName = "John Wick";
            var expectedResult = new DocumentInfo()
            {
                DocumentId = documentId,
                FileName = fileName,
            };
            this.databaseFixture.InsertDocumentInfo(expectedResult);

            // Act
            var result = this.documentRepository.Filter(x => x.FileName.Equals(fileName)).FirstOrDefault();

            // Assert
            Assert.Equal(expectedResult, result);
        }

        private DocumentInfo generateNewDocumentInfo(Guid id)
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
            return documentInfo;
        }

        private Guid[] createMultipleGuids()
        {
            var guids = new Guid[10];

            for (int i = 0; i < guids.Length; i++)
            {
                guids[i] = Guid.NewGuid();
            }

            return guids;
        }
    }
}
