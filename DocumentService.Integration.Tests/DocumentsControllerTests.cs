using DocumentService.Azure;
using DocumentService.Contexts;
using DocumentService.Controllers;
using DocumentService.Models;
using DocumentService.Repositories;
using DocumentService.Unit.Tests.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace DocumentService.Integration.Tests
{
    [CollectionDefinition("Database collection")]
    public class DocumentsControllerTests : IClassFixture<DatabaseFixture>
    {
        private readonly IConfiguration configuration;
        private readonly DocumentContext documentContext;
        private readonly IDocumentRepository documentRepository;
        private readonly IAzureBlobService azureBlobService;
        private readonly IAzureKeyVaultService azureKeyVault;
        private readonly IDatabaseFixture databaseFixture;

        public DocumentsControllerTests()
        {
            this.configuration = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();

            this.azureKeyVault = new AzureKeyVaultService(this.configuration);
            this.azureBlobService = new AzureBlobService(this.azureKeyVault);
            this.documentContext = new DocumentContext(this.configuration, this.azureKeyVault);
            this.documentRepository = new DocumentRepository(this.documentContext);
            this.databaseFixture = new DatabaseFixture();
        }

        [Fact]
        public void UploadDocument_UploadsSuccessfully_WhenAllInformation()
        {
            // Arrange
            var azureKeyVault = new AzureKeyVaultService(configuration);
            var azureBlobService = new AzureBlobService(azureKeyVault);
            var documentContext = new DocumentContext(configuration, azureKeyVault);
            var documentRepository = new DocumentRepository(documentContext);

            var controller = new DocumentsController(documentRepository, azureBlobService, configuration);

            var ms = new MemoryStream();
            TextWriter tw = new StreamWriter(ms);
            tw.Write("blablaThis is a dummy file");
            tw.Flush();
            ms.Position = 0;
            byte[] bytes = ms.ToArray();

            var fileName = "test-" + DateTime.Now.ToString() + ".txt";
            var file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, fileName, fileName);

            // Act
            var response = controller.UploadDocument(0, "Tester", file, "text/plain", "Testing file", "Unit Test", "English", null, null);
            var res = response as OkObjectResult;
            dynamic result = res.Value;

            // Assert
            Assert.NotNull(res.Value);
            Assert.True(result.GetType().GetProperty("documentId").GetValue(result, null) > 0);
        }

        [Fact]
        public void GetAllSpecifiedDocuments_GetsAllSpecifiedDocuments_WhenIds()
        {
            // Arrange
            var azureKeyVault = new AzureKeyVaultService(configuration);
            var azureBlobService = new AzureBlobService(azureKeyVault);
            var documentContext = new DocumentContext(configuration, azureKeyVault);
            var documentRepository = new DocumentRepository(documentContext);

            // Act
            var controller = new DocumentsController(documentRepository, azureBlobService, configuration);
            List<Guid> guids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            this.databaseFixture.CreateListOfDocumentInfos(guids.ToArray());

            var response = controller.GetAllSpecifiedDocuments(string.Join(",", guids));
            var res = response as OkObjectResult;

            // Assert
            Assert.NotNull(res.Value);
            var result = res.Value as List<DocumentInfo>;

            Assert.Equal(guids.Count, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                Assert.Equal(result[i].DocumentId.ToString(), guids[i].ToString());
            }
        }
    }
}
