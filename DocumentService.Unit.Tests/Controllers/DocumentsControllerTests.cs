using DocumentService.Azure;
using DocumentService.Contexts;
using DocumentService.Controllers;
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
        [Fact]
        public void UploadDocument_UploadsSuccessfully_WhenAllInformation()
        {

        }

        [Fact]
        public void GetAllSpecifiedDocuments_GetsAllSpecifiedDocuments_WhenIds()
        {

        }

        [Fact]
        public void GetDocumentById_True_WhenIdExists()
        {

        }
        /*
         *  // public DocumentRepository(DocumentContext context)
         *  // this.documentRepository.SetFileDeleted(id, userName).Result;
            public IActionResult DeleteDocumentById(Guid id, string userName)
            {
                var isDeleted = this.documentRepository.SetFileDeleted(id, userName).Result;
                return Ok(new { isDeleted });
            }
         */

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
            var result = documentController.DeleteDocumentById(guid, userName).Value;

            // Assert
            Assert.True((bool)result);
        }
    }
}
