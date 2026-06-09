using DarkKitchen.Domain.Entities;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test;

[TestClass]
public sealed class ImportControllerTest
{
    [TestMethod]
    public void GetAvailableImporters_WhenCalled_ReturnsOkWithImporters()
    {
        var importers = new List<ImporterInfoDTO>
        {
            new ImporterInfoDTO { Name = "JSON" },
            new ImporterInfoDTO { Name = "XML" }
        };

        var importServiceMock = new Mock<IImportService>();
        importServiceMock.Setup(s => s.GetAvailableImporters()).Returns(importers);

        var controller = new ImportController(importServiceMock.Object);

        var result = controller.GetAvailableImporters();

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        Assert.AreSame(importers, okResult.Value);
    }

    [TestMethod]
    public void Import_WhenCalled_ReturnsOkWithImportResult()
    {
        var request = new ImportProductsRequestDTO
        {
            ImporterName = "JSON",
            Content = "[{ \"code\": \"P0001\" }]",
            FileName = "products.json"
        };

        var importResult = new ImportResultDTO { ImportedCount = 1 };

        var importServiceMock = new Mock<IImportService>();
        importServiceMock.Setup(s => s.Import(request, "admin@email.com"))
                        .Returns(importResult);

        var controller = new ImportController(importServiceMock.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        controller.HttpContext.Items["RequestingUser"] = new User { Id = 1, Email = "admin@email.com" };

        var result = controller.Import(request);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        Assert.AreSame(importResult, okResult.Value);
    }
}
