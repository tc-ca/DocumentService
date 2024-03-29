﻿namespace DocumentService.Unit.Tests.Controllers
{
    using DocumentService.Azure;
    using DocumentService.Contexts;
    using DocumentService.Controllers;
    using DocumentService.Models;
    using DocumentService.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using System;
    using System.Threading.Tasks;
    using Xunit;
    using System.Net;
    using System.Collections.Generic;
    using System.Linq;
    using DocumentService.Repositories.Entities;
    using DocumentService.ServiceModels;

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

        [Fact]
        public void GetDocumentById_True_WhenIdExists()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var documentController = new DocumentsController(this.documentRepository.Object, this.azureBlobService.Object, this.configuration.Object);

            Document documentInfo = new Document
            {
                DocumentId = guid,
                DateCreated = DateTime.UtcNow,
                Description = "Generic Description",
                FileName = "Test Doc",
                DocumentTypes = new List<DocumentType> { new DocumentType { Description = "Test", Id = "0" } },
                IsDeleted = false
            };
            documentRepository.Setup(x => x.GetDocumentAsync(guid)).Returns(Task.FromResult(documentInfo));

            // Act
            var response = (OkObjectResult)documentController.GetDocumentById(guid);
            var document = (Document)response.Value;

            // Assert
            Assert.Equal(document.FileName, documentInfo.FileName);
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

            documentRepository.Setup(x => x.GetDocumentAsync(guid)).Returns(Task.FromResult((Document)null));
            var documentController = new DocumentsController(documentRepository.Object, azureBlobService.Object, configuration.Object);

            // Act
            var response = documentController.GetDocumentById(guid);

            var result = response.GetType().GetProperty("StatusCode").GetValue(response).ToString();


            // Assert
            Assert.Equal(expected.ToString(), result);
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
            var result = (JsonResult)documentController.DeleteDocumentById(guid, userName);

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

        [Fact]
        public void DeleteDocumentById_ReturnsBadRequestResult_WhenExceptionThrown()
        {
            // Arrange
            var guid = Guid.Empty;
            var userName = "John Wick";
            documentRepository.Setup(x => x.SetFileDeleted(guid, userName)).Throws<NullReferenceException>();
            var documentController = new DocumentsController(this.documentRepository.Object, this.azureBlobService.Object, this.configuration.Object);
            var expectedResult = 400;

            // Act
            var result = documentController.DeleteDocumentById(guid, userName);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(expectedResult, ((BadRequestObjectResult)result).StatusCode);
        }

        [Fact]
        public void UpdateMetadataForDocument_ReturnsTrue_WhenUpdateSuccessful()
        {
            // Arrange
            var expectedResult = new Document();
            documentRepository.Setup(x => x.Update(It.IsAny<Document>())).Returns(Task.FromResult(expectedResult));
            var documentController = new DocumentsController(this.documentRepository.Object, this.azureBlobService.Object, this.configuration.Object);
            var updateMetaDataDTO = new UpdateMetaDataDTO()
            {
                DocumentId = new Guid("11111111-1111-1111-1111-111111111111")
            };

            // Act
            var unparsedResult = (JsonResult)documentController.UpdateMetadataForDocument(updateMetaDataDTO);
            var parsedResult = (Document)unparsedResult.Value;

            // Assert
            Assert.NotNull(parsedResult);
        }

        [Fact]
        public void UpdateMetadataForDocument_ReturnsFalse_WhenUpdateUnsuccessful()
        {
            // Arrange
            var documentInfo = new DocumentInfo();
            var expectedResult = new Document();
            documentRepository.Setup(x => x.Update(It.IsAny<Document>())).Returns(Task.FromResult(expectedResult));
            var documentController = new DocumentsController(this.documentRepository.Object, this.azureBlobService.Object, this.configuration.Object);
            var updateMetaDataDTO = new UpdateMetaDataDTO()
            {
                DocumentId = new Guid("11111111-1111-1111-1111-111111111111")
            };

            // Act
            var unparsedResult = (JsonResult)documentController.UpdateMetadataForDocument(updateMetaDataDTO);
            var parsedResult = (Document)unparsedResult.Value;

            // Assert
            Assert.NotNull(parsedResult);
        }

        [Fact]
        public void UpdateMetadataForDocument_ReturnsBadRequest_WhenExceptionThrown()
        {
            // Arrange
            var documentInfo = new DocumentInfo();
            documentRepository.Setup(x => x.Update(It.IsAny<Document>())).Throws<NullReferenceException>();
            var documentController = new DocumentsController(this.documentRepository.Object, this.azureBlobService.Object, this.configuration.Object);
            var expectedResult = 400;
            var updateMetaDataDTO = new UpdateMetaDataDTO();

            // Act
            var result = documentController.UpdateMetadataForDocument(updateMetaDataDTO);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(expectedResult, ((BadRequestObjectResult)result).StatusCode);
        }
    }
}
