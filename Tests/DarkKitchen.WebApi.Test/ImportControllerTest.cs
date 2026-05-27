using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Controllers;
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
}
