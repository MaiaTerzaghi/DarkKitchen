using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
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
    [ExpectedException(typeof(ArgumentException))]
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

        _controller.CreateOrder(request);
    }

    [TestMethod]
    public void GetClientOrders_WhenCalled_ReturnsOk()
    {
        var orders = new List<GetClientOrdersResponseDTO>();

        _orderServiceMock
            .Setup(s => s.GetClientOrders(It.IsAny<GetClientOrdersRequestDTO>()))
            .Returns(orders);

        var result = _controller.GetClientOrders(new GetClientOrdersRequestDTO { ClientId = 1 });

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void GetOrders_ValidRequest_ReturnsOkWithOrders()
    {
        var request = new GetOrdersRequestDTO
        {
            DateFrom = new DateTime(2026, 1, 1),
            DateTo = new DateTime(2026, 1, 31)
        };

        var expectedOrders = new List<GetOrdersResponseDTO>
        {
            new GetOrdersResponseDTO
            {
                OrderId = 1,
                ClientName = "Juan Perez",
                Date = new DateTime(2026, 1, 10),
                Status = "Pending",
                Items = [new OrderItemResponseDTO { ProductName = "Hamburguesa", Quantity = 2 }]
            }
        };

        _orderServiceMock
            .Setup(s => s.GetOrders(request))
            .Returns(expectedOrders);

        var result = _controller.GetOrders(request);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        var response = (List<GetOrdersResponseDTO>)okResult.Value!;
        Assert.AreEqual(expectedOrders.Count, response.Count);
        Assert.AreEqual(expectedOrders[0].OrderId, response[0].OrderId);
        Assert.AreEqual(expectedOrders[0].ClientName, response[0].ClientName);
    }

    [TestMethod]
    public void MarkAsPrepared_ValidOrderId_ReturnsOkWithUpdatedStatus()
    {
        var orderId = 1;

        var expectedResponse = new UpdateOrderStatusResponseDTO
        {
            OrderId = orderId,
            Status = "Prepared",
            UpdatedAt = DateTime.Now
        };

        _orderServiceMock
            .Setup(s => s.MarkAsPrepared(orderId))
            .Returns(expectedResponse);

        var result = _controller.MarkAsPrepared(orderId);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        var response = (UpdateOrderStatusResponseDTO)okResult.Value!;
        Assert.AreEqual("Prepared", response.Status);
        Assert.AreEqual(orderId, response.OrderId);
    }

    [TestMethod]
    public void GetOrderDetail_WhenCalled_ReturnsOk()
    {
        var response = new OrderDetailResponseDTO
        {
            OrderId = 1,
            ClientId = 1,
            Status = "Pending",
            Total = 200.0
        };

        _orderServiceMock
            .Setup(s => s.GetOrderDetail(It.IsAny<int>()))
            .Returns(response);

        var result = _controller.GetOrderDetail(1);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void DeliverOrder_WhenCalled_ReturnsOk()
    {
        var response = new UpdateOrderStatusResponseDTO
        {
            OrderId = 1,
            Status = "Delivered",
            UpdatedAt = DateTime.Now
        };

        _orderServiceMock
            .Setup(s => s.DeliverOrder(It.IsAny<int>()))
            .Returns(response);

        var result = _controller.DeliverOrder(1);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void CancelOrder_ValidOrderId_ReturnsOkWithUpdatedStatus()
    {
        var orderId = 1;

        var expectedResponse = new UpdateOrderStatusResponseDTO
        {
            OrderId = orderId,
            Status = "Cancelled",
            UpdatedAt = DateTime.Now
        };

        _orderServiceMock
            .Setup(s => s.CancelOrder(orderId))
            .Returns(expectedResponse);

        var result = _controller.CancelOrder(orderId);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        var response = (UpdateOrderStatusResponseDTO)okResult.Value!;
        Assert.AreEqual("Cancelled", response.Status);
        Assert.AreEqual(orderId, response.OrderId);
    }
}
