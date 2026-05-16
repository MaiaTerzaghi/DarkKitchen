using DarkKitchen.DTOs.Args.In;
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

    [TestMethod]
    public void GetById_WhenExists_ReturnsOk()
    {
        var shippingType = new ShippingTypeResponseDTO
        {
            Id = 1,
            Name = "Envío express",
            Cost = 250
        };

        _serviceMock.Setup(s => s.GetById(1)).Returns(shippingType);

        var result = _controller.GetById(1);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        Assert.IsNotNull(okResult.Value);
    }

    [TestMethod]
    public void Create_WhenValid_ReturnsCreated()
    {
        var request = new CreateShippingTypeRequestDTO
        {
            Name = "Envío express",
            Cost = 250
        };

        var response = new ShippingTypeResponseDTO
        {
            Id = 1,
            Name = "Envío express",
            Cost = 250
        };

        _serviceMock.Setup(s => s.Create(It.IsAny<CreateShippingTypeRequestDTO>()))
                    .Returns(response);

        var result = _controller.Create(request);

        Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
    }

    [TestMethod]
    public void Update_WhenValid_ReturnsOk()
    {
        var request = new UpdateShippingTypeRequestDTO
        {
            Name = "Envío express modificado",
            Cost = 300
        };

        var response = new ShippingTypeResponseDTO
        {
            Id = 1,
            Name = "Envío express modificado",
            Cost = 300
        };

        _serviceMock.Setup(s => s.Update(1, It.IsAny<UpdateShippingTypeRequestDTO>()))
                    .Returns(response);

        var result = _controller.Update(1, request);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        Assert.IsNotNull(okResult.Value);
    }
}
