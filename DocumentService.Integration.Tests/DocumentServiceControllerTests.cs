namespace DocumentService.Integration.Tests
{
    using DocumentService.Controllers;
    using DocumentService.Repositories;
    using DocumentService.Extensions;
    using DocumentService.Azure;
    using DocumentService.Tests.Common.Services;
    using Microsoft.Extensions.Configuration;
    using Xunit;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Http;
    using System.IO;
    using System.Text;
    using Microsoft.AspNetCore.Mvc;
    using DocumentService.Models;
    using MimeTypes;
    using DocumentService.ServiceModels;

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
        public async void UploadDocument_SucessfullyUploads()
        {
            var fileName = "dummy-text.txt";
            // Arrange
            var documentController = new DocumentsController(this.documentRepository, this.azureBlobService, this.configuration);
            IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy2 image")), 0, 10, fileName, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = MimeTypeMap.GetMimeType(fileName)
            };
            
            var uploadedDocumentDTO = new UploadedDocumentsDTO()
            {
                UserName = "John Wick",
                FileName = file.FileName,
                FileBytes = await file.GetBytes(),
                FileContentType = string.Empty,
                ShortDescription = "My test file",
                SubmissionMethod = "FAX",
                FileLanguage = "EN",
                DocumentTypes = new List<DocumentType>()
                {
                    new DocumentType()
                    {
                        Id = "One",
                        Description = "Description test"
                    },
                    new DocumentType()
                    {
                        Id = "Two",
                        Description = "Description test two"
                    }
                },
                CustomMetadata = string.Empty
            };

            // Act
            OkObjectResult response = (OkObjectResult)documentController.UploadDocument(uploadedDocumentDTO);
            Document document = (Document)response.Value;

            // Assert
            Assert.NotNull(document);
        }

        [Fact]
        public async void EnsureDocumentMatchesBlob_Success()
        {
            // Arrange
            IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("Text is cool")), 0, 10, "image.txt", "image.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            var mimeType = MimeTypeMap.GetMimeType(file.FileName);
            // Act
            var result = await this.azureBlobService.UploadFileAsync(file, configuration.GetSection("BlobContainers")["Documents"]);

            var blob = this.azureBlobService.GetBlob(configuration.GetSection("BlobContainers")["Documents"], result.Uri.AbsoluteUri);

            // Assert
            Assert.NotNull(blob);
            Assert.True(string.Compare(blob.Properties.ContentType, file.ContentType) == 0 &&
                string.Compare(file.ContentType, mimeType) == 0);
        }
    }
}
