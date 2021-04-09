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
    public class DocumentsControllerTests 
    {
        private readonly Mock<IDocumentContext> documentContext;
        private readonly Mock<IDocumentRepository> documentRepository;
        private readonly Mock<IAzureBlobService> azureBlobService;
        private readonly Mock<IConfiguration> configuration;

        public DocumentsControllerTests()
        {
            this.documentContext = new Mock<IDocumentContext>();
            this.documentRepository = new Mock<IDocumentRepository>();
            this.azureBlobService = new Mock<IAzureBlobService>();
            this.configuration = new Mock<IConfiguration>();
        }

        //[Fact]
        //public void UploadDocument_UploadsSuccessfully_WhenAllInformation()
        //{
        //    // Arrange
        //    var azureKeyVault = new AzureKeyVaultService(configuration);
        //    var azureBlobService = new AzureBlobService(azureKeyVault);
        //    var documentContext = new DocumentContext(configuration, azureKeyVault);
        //    var documentRepository = new DocumentRepository(documentContext);

        //    var controller = new DocumentsController(documentRepository, azureBlobService, configuration);

        //    var ms = new MemoryStream();
        //    TextWriter tw = new StreamWriter(ms);
        //    tw.Write("blablaThis is a dummy file");
        //    tw.Flush();
        //    ms.Position = 0;
        //    byte[] bytes = ms.ToArray();

        //    var fileName = "test-" + DateTime.Now.ToString() + ".txt";

        //    var file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, fileName, fileName);

        //    // Act
        //    var response = controller.UploadDocument(0, "Tester", file, "text/plain", "Testing file", "Unit Test", "English", null, null);

        //    var res = response as OkObjectResult;
        //    dynamic result = res.Value;

        //    // Assert
        //    Assert.NotNull(res.Value);
        //    Assert.True(result.GetType().GetProperty("documentId").GetValue(result, null) > 0);
        //}

        [Fact]
        public void GetAllSpecifiedDocuments_GetsAllSpecifiedDocuments_WhenIds()
        {

        }

        [Fact]
        public void GetDocumentById_True_WhenIdExists()
        {

        }

        [Fact]
        public void DeleteDocumentById_ReturnsTrue_WhenDeleteSuccessful()
        {
            // Arrange
            var guid = Guid.Empty;
            var userName = "John Wick";
            this.documentRepository.Setup(x => x.SetFileDeleted(guid, userName)).Returns(Task.FromResult(true));
            var documentController = new DocumentsController(this.documentRepository.Object, this.azureBlobService.Object, this.configuration.Object);

            // Act
            var result = (JsonResult) documentController.DeleteDocumentById(guid, userName);
            
            // Assert
            Assert.True((bool)result.Value);
        }
        
        [Fact]
        public void DeleteDocumentById_ReturnsNotFoundResult_WhenItemDoesNotExist()
        {
            // Arrange
            var guid = Guid.Empty;
            var userName = "John Wick";
            documentRepository.Setup(x => x.SetFileDeleted(guid, userName)).Returns(Task.FromResult(false));
            var documentController = new DocumentsController(this.documentRepository.Object, this.azureBlobService.Object, this.configuration.Object);
            var expectedResult = 404;

            // Act
            var result = documentController.DeleteDocumentById(guid, userName);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(expectedResult, ((NotFoundResult)result).StatusCode);
        }
    }
}
