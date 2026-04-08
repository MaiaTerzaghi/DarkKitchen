using DarkKitchen.BusinessLogic.Args.In;
using DarkKitchen.BusinessLogic.Args.Output;
using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test;

[TestClass]
public class OrderControllerTest
{
    private Mock<IOrderService> _orderServiceMock = null!;
    private OrderController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _orderServiceMock = new Mock<IOrderService>();
        _controller = new OrderController(_orderServiceMock.Object);
    }

    [TestMethod]
    public void CreateOrder_ValidRequest_ReturnsOkWithResponse()
    {
        var request = new CreateOrderRequestDTO
        {
            ClientId = 1,
            DeliveryType = "Express",
            Address = new AddressDTO
            {
                Street = "18 de Julio",
                DoorNumber = "1234",
                Apartment = "2B"
            },
            Items =
            [
                new OrderItemRequestDTO { ProductId = 1, Quantity = 2 }
            ]
        };

        var expectedResponse = new CreateOrderResponseDTO
        {
            ClientId = request.ClientId,
            OrderId = 1,
            Subtotal = 100.0,
            ShippingCost = 50.0,
            Total = 147.96
        };

        _orderServiceMock
            .Setup(s => s.CreateOrder(request))
            .Returns(expectedResponse);

        var result = _controller.CreateOrder(request);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        var response = (CreateOrderResponseDTO)okResult.Value!;
        Assert.AreEqual(expectedResponse.OrderId, response.OrderId);
        Assert.AreEqual(expectedResponse.Total, response.Total);
    }

    [TestMethod]
    public void CreateOrder_EmptyItems_ReturnsBadRequest()
    {
        var request = new CreateOrderRequestDTO
        {
            ClientId = 1,
            DeliveryType = "Express",
            Address = new AddressDTO
            {
                Street = "18 de Julio",
                DoorNumber = "1234",
                Apartment = "2B"
            },
            Items = []
        };

        _orderServiceMock
            .Setup(s => s.CreateOrder(request))
            .Throws(new ArgumentException("El pedido debe tener al menos un producto."));

        var result = _controller.CreateOrder(request);

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }
}
