using DocumentService.Azure;
using DocumentService.Contexts;
using DocumentService.Controllers;
using DocumentService.Repositories;
using DocumentService.Unit.Tests.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Threading.Tasks;
using DocumentService.Repositories;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.Unit.Tests.Controllers
{
    [CollectionDefinition("Database collection")]
    public class DocumentsControllerTests : IClassFixture<DatabaseFixture>
    {

        private readonly IConfiguration configuration;

        public DocumentsControllerTests()
        {
            this.configuration = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();

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

        }

        [Fact]
        public void GetDocumentById_True_WhenIdExists()
        {

        }

        [Fact]
        public void DeleteDocumentById_ReturnsId_WhenExists()
        {
            // Arrange
            var documentContext = new Mock<IDocumentContext>();
            var documentRepository = new Mock<IDocumentRepository>();
            var azureBlobService = new Mock<IAzureBlobService>();
            var configuration = new Mock<IConfiguration>();
            var guid = Guid.Empty;
            var userName = "John Wick";
            documentRepository.Setup(x => x.SetFileDeleted(guid, userName)).Returns(Task.FromResult(true));
            var documentController = new DocumentsController(documentRepository.Object, azureBlobService.Object, configuration.Object);

            // Act
            var result = (JsonResult) documentController.DeleteDocumentById(guid, userName);
            
            // Assert
            Assert.True((bool)result.Value);
        }
    }
}
