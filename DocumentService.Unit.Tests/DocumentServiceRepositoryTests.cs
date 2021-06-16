namespace DocumentService.Unit.Tests
{
    using DocumentService.Models;
    using DocumentService.Repositories;
    using DocumentService.ServiceModels;
    using DocumentService.Tests.Common.Services;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

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

        public void GetDocumentAsync_WhenExists_ReturnsDocumentInfo()
        {
            // Arrange
            var guid = Guid.NewGuid();
            int count = 1;
            var expectedResult = this.databaseFixture.CreateDocument(count, guid).First();
            this.databaseFixture.InsertDocument(expectedResult, guid);

            // Act 
            var result = this.documentRepository.GetDocumentAsync(guid).Result;

            string jsonExcepted = JsonConvert.SerializeObject(expectedResult, Formatting.Indented);
            string jsonResult = JsonConvert.SerializeObject(result, Formatting.Indented);

            Assert.Equal(jsonExcepted, jsonResult);
            // We do this as comparing the objects themselves doesn't work, but passes if tested differently
            foreach (var value in expectedResult.GetType().GetProperties())
            {
                var exceptedValue = value.GetValue(expectedResult, null);
                var resultValue = value.GetValue(result, null);

                Assert.Equal(exceptedValue, resultValue);
            }
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
            var result = this.documentRepository.GetDocumentAsync(documentInfoId).Result;

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetDocumentAsync_WhenNotExists_ReturnsNull()
        {
            // Arrange
            var documentInfoId = new Guid("11111111-1111-1111-1111-111111111110");

            // Act
            var result = this.documentRepository.GetDocumentAsync(documentInfoId).Result;

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void UploadDocumentAsync_UploadSuccessful_ReturnsOne()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var document = this.databaseFixture.CreateDocument(1, guid).First();

            // Act
            var result = this.documentRepository.UploadDocumentAsync(document).Result;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void UploadDocumentAsync_UploadFailed_ThrowsNullReferenceException()
        {
            // Assert
            Assert.ThrowsAsync<NullReferenceException>(() => this.documentRepository.UploadDocumentAsync(null));
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
            var result = this.documentRepository.SetFileDeleted(testDocumentInfo.DocumentId, "Tester").Result;

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void SetFileDeleted_SetDeletedFailed_ReturnsFalse()
        {
            // Arrange
            var expectedResult = false;

            // Act
            var result = this.documentRepository.SetFileDeleted(Guid.Empty, "Tester").Result;

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Update_UpdateSuccessful_ReturnsTrue()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var document = this.databaseFixture.CreateDocument(1, guid).First();
            this.databaseFixture.InsertDocument(document, guid);

            // Act
            document.FileName = "Our new file name";
            var result = this.documentRepository.Update(document).Result;

            // Assert
            Assert.Equal(document, result);
        }
        [Fact]
        public void Update_UpdateFailed_ReturnsEmptyDocumentUpdatedResultList()
        {
            //Arrange
            var expectedResult = new Document();

            // Act
            var result = this.documentRepository.Update(expectedResult).Result;

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void GetDocumentsByIds_EntriesFound_ReturnsListOfEntries()
        {
            // Arrange
            var testGuids = this.createMultipleGuids();
            var expectedResult = this.databaseFixture.CreateListOfDocumentInfos(testGuids).Select(x => x.DocumentId);

            // Act
            var result = this.documentRepository.GetDocumentsByIds(testGuids);
            expectedResult = expectedResult.OrderBy(x => x);
            var assertedResult = result.Select(x => x.DocumentId).OrderBy(x => x);

            // Assert
            Assert.Equal(expectedResult, assertedResult);
        }

        [Fact]
        public void GetDocumentsByIds_EntriesNotFound_ReturnsEmpty()
        {
            // Arrange
            var testGuids = this.createMultipleGuids();

            // Act
            var result = this.documentRepository.GetDocumentsByIds(testGuids);

            // Assert
            Assert.NotNull(result);
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
                DocumentTypes = new List<DocumentType> { new DocumentType() { Description = "Test", Id = "0" } },
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
