using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test;

[TestClass]
public sealed class ShippingTypeControllerTest
{
    private Mock<IShippingTypeService> _serviceMock = null!;
    private ShippingTypeController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _serviceMock = new Mock<IShippingTypeService>();
        _controller = new ShippingTypeController(_serviceMock.Object);
    }

    [TestMethod]
    public void GetAll_WhenCalled_ReturnsOkWithList()
    {
        var shippingTypes = new List<ShippingTypeResponseDTO>
        {
            new() { Id = 1, Name = "Envío express", Cost = 250 },
            new() { Id = 2, Name = "Envío en el día", Cost = 200 }
        };

        _serviceMock.Setup(s => s.GetAll()).Returns(shippingTypes);

        var result = _controller.GetAll();

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        Assert.IsNotNull(okResult.Value);
    }
}
