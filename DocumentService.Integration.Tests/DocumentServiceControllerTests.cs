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
            var azureBlobConnectionFactory = new AzureBlobConnectionFactory(this.configuration, azureKeyVaultService);

            this.azureBlobService = new AzureBlobService(this.configuration, azureBlobConnectionFactory, azureKeyVaultService);
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
    }
}
