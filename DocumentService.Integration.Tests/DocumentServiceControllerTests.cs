namespace DocumentService.Integration.Tests
{
    using DocumentService.Controllers;
    using DocumentService.Repositories;
    using DocumentService.Azure;
    using DocumentService.Tests.Common.Services;
    using Microsoft.Extensions.Configuration;
    using Xunit;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Http;
    using System.IO;
    using System.Text;
    using Microsoft.AspNetCore.Mvc;
    using System;

    [CollectionDefinition("Database collection")]
    public class DocumentServiceControllerTests : IClassFixture<DatabaseFixture>
    {
        private DatabaseFixture databaseFixture;

        private readonly DocumentRepository documentRepository;

        private readonly IAzureBlobService azureBlobService;

        private readonly IConfiguration configuration;

        public DocumentServiceControllerTests()
        {
            var testConfigurationBuilder = new TestConfigurationBuilder();
            this.configuration = testConfigurationBuilder.Build();

            var azureKeyVaultService = new AzureKeyVaultService(this.configuration);

            this.azureBlobService = new AzureBlobService(azureKeyVaultService);
            this.databaseFixture = new DatabaseFixture();
            this.documentRepository = new DocumentRepository(this.databaseFixture.Context);
        }

        [Fact]
        public void UploadDocument_SucessfullyUploads()
        {
            // Arrange
            var documentController = new DocumentsController(this.documentRepository, this.azureBlobService, this.configuration);

            // Act
            IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy2 image")), 0, 10, "Data", "image.png")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };

            // Act
            var response = documentController.UploadDocument(1, "John Wick", file, string.Empty, "My Test file", "FAX", "EN", new List<string>(), string.Empty);
            var res = response as OkObjectResult;
            dynamic result = res.Value;
            var documentIds = (List<Guid>)result.GetType().GetProperty("uploadedDocumentIds").GetValue(result, null);

            // Assert
            Assert.NotEmpty(documentIds);
        }
    }
}
