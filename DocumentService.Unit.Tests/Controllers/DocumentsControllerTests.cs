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
using Moq;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using DocumentService.Models;
using System.Net;

namespace DocumentService.Unit.Tests.Controllers
{
    [CollectionDefinition("Database collection")]
    public class DocumentsControllerTests : IClassFixture<DatabaseFixture>
    {
        private readonly IConfiguration configuration;

        private readonly DatabaseFixture databaseFixture;

        public DocumentsControllerTests()
        {
            this.configuration = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();

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
            // Create the list of documents & save them into the database
            databaseFixture.CreateListOfDocumentInfos(guids.ToArray());

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

        [Fact]
        public void GetDocumentById_True_WhenIdExists()
        {
            // Arrange
            var documentContext = new Mock<IDocumentContext>();
            var documentRepository = new Mock<IDocumentRepository>();
            var azureBlobService = new Mock<IAzureBlobService>();
            var configuration = new Mock<IConfiguration>();
            var guid = Guid.NewGuid();
            var documentController = new DocumentsController(documentRepository.Object, azureBlobService.Object, configuration.Object);

            Document documentInfo = new Document
            {
                DocumentId = guid,
                DateCreated = DateTime.UtcNow,
                Description = "Generic Description",
                FileName = "Test Doc",
                DocumentType = new DocumentTypes { DocumentType = "Test", DocumentTypesId = 0 },
                IsDeleted = false
            };

            var dto = new DocumentDTO
            {
                Documents = new List<Document> { documentInfo }
            };


            documentRepository.Setup(x => x.GetDocumentAsync(guid)).Returns(Task.FromResult(dto));
            // act
            var response = documentController.GetDocumentById(guid);
            var res = response as OkObjectResult;
            dynamic result = res.Value;
            var docInfo = (DocumentDTO)result.GetType().GetProperty("document").GetValue(result, null);

            // Assert
            Assert.Equal(docInfo.Documents.First().FileName, documentInfo.FileName);


        }
        [Fact]
        public void GetDocumentById_WhenIdIsNotFound_ReturnsBadRequest()
        {
            // Arrange
            var expected = (int)HttpStatusCode.BadRequest;
            var documentContext = new Mock<IDocumentContext>();
            var documentRepository = new Mock<IDocumentRepository>();
            var azureBlobService = new Mock<IAzureBlobService>();
            var configuration = new Mock<IConfiguration>();
            var guid = Guid.Empty;

            documentRepository.Setup(x => x.GetDocumentAsync(guid)).Returns(Task.FromResult((DocumentDTO)null));
            var documentController = new DocumentsController(documentRepository.Object, azureBlobService.Object, configuration.Object);

            // Act
            var response = documentController.GetDocumentById(guid);

            var result = response.GetType().GetProperty("StatusCode").GetValue(response).ToString();


            // Assert
            Assert.Equal(expected.ToString(), result);



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
            var result = (JsonResult)documentController.DeleteDocumentById(guid, userName);

            // Assert
            Assert.True((bool)result.Value);
        }
    }
}
