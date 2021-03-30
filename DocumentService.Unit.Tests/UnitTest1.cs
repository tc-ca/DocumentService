using DocumentService.Contexts;
using DocumentService.Models;
using DocumentService.Repositories;
using DocumentService.TestData;
using Microsoft.Extensions.Configuration;
using System;
using Xunit;

namespace DocumentService.Unit.Tests
{
    public class UnitTest1
    {
        private readonly DocumentContext context;
        private readonly IConfiguration configuration;
        public UnitTest1()
        {
            this.configuration = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
            context = new DocumentContext(configuration);
        }
        [Fact]
        public void Test1()
        {
            // Arrange
            IDocumentContext documentContext = new DocumentContext(this.configuration);
            IDocumentsInitializer documentInitializer = new DocumentsInitializer();
            documentInitializer.context = (DocumentContext)documentContext;

            // Act
            documentInitializer.Seed();

            // Assert
            Assert.True(true);
        }
        [Fact]
        public void GetDocumentAsync_WhenExists_ReturnsDocumentInfo()
        {
            // Arrange
            var documentInfoId = Guid.NewGuid();
            DocumentRepository repo = new DocumentRepository(context);
            DocumentInfo expectedResult = new DocumentInfo
            {
                CorrelationId = Guid.NewGuid(),
                DocumentId = documentInfoId,
                DateCreated = DateTime.UtcNow,
                Description = "Generic Description",
                FileName = "Test Doc",
                DocumentTypes = new DocumentTypes { DocType = "Test", DocumentTypesId = 0 },
                IsDeleted = false
        };
            context.DocumentInfo.Add(expectedResult);

            // Act 
            var result = repo.GetDocumentAsync(documentInfoId).Result;

            // Assert
            Assert.Equal(expectedResult.FileName, result.FileName);



        }
    }

}
