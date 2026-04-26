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

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.HttpContext.Items["RequestingUser"] = new User { Id = 1 };

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

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.HttpContext.Items["RequestingUser"] = new User { Id = 1 };

        _controller.CreateOrder(request);
    }

    [TestMethod]
    public void GetClientOrders_WhenCalled_ReturnsOk()
    {
        var orders = new List<GetClientOrdersResponseDTO>();

        _orderServiceMock
            .Setup(s => s.GetClientOrders(It.IsAny<GetClientOrdersRequestDTO>()))
            .Returns(orders);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.HttpContext.Items["RequestingUser"] = new User { Id = 1 };

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
                Client = new ClientInfoDTO
                {
                    Id = 1,
                    Name = "Juan",
                    LastName = "Perez",
                    Phone = "+59899000000"
                },
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
        Assert.AreEqual(expectedOrders[0].Client.Id, response[0].Client.Id);
        Assert.AreEqual(expectedOrders[0].Client.Name, response[0].Client.Name);
        Assert.AreEqual(expectedOrders[0].Client.LastName, response[0].Client.LastName);
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

    [TestMethod]
    public void MarkAsOnTheWay_ValidOrderId_ReturnsOk()
    {
        var expectedResponse = new UpdateOrderStatusResponseDTO
        {
            OrderId = 1,
            Status = "OnTheWay",
            UpdatedAt = DateTime.Now
        };

        _orderServiceMock
            .Setup(s => s.MarkAsOnTheWay(1))
            .Returns(expectedResponse);

        var result = _controller.MarkAsOnTheWay(1);
        var okResult = (OkObjectResult)result;
        var response = (UpdateOrderStatusResponseDTO)okResult.Value!;

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        Assert.AreEqual("OnTheWay", response.Status);
        Assert.AreEqual(1, response.OrderId);
    }

    [TestMethod]
    public void MarkAsNotDelivered_ValidOrderId_ReturnsOkWithUpdatedStatus()
    {
        var orderId = 1;

        var expectedResponse = new UpdateOrderStatusResponseDTO
        {
            OrderId = orderId,
            Status = "NotDelivered",
            UpdatedAt = DateTime.Now
        };

        _orderServiceMock
            .Setup(s => s.MarkAsNotDelivered(orderId))
            .Returns(expectedResponse);

        var result = _controller.MarkAsNotDelivered(orderId);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        var response = (UpdateOrderStatusResponseDTO)okResult.Value!;
        Assert.AreEqual("NotDelivered", response.Status);
        Assert.AreEqual(orderId, response.OrderId);
    }

    [TestMethod]
    public void GetSalesReport_ValidRequest_ReturnsOk()
    {
        var expectedResponse = new List<SalesReportResponseDTO>
        {
            new SalesReportResponseDTO
            {
                Year = 2026,
                Month = 1,
                Clients = [new ClientSalesDTO { ClientId = 1, Total = 500.0 }]
            }
        };

        _orderServiceMock
            .Setup(s => s.GetSalesReport(1, 20))
            .Returns(expectedResponse);

        var result = _controller.GetSalesReport(1, 20);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        var response = (List<SalesReportResponseDTO>)okResult.Value!;
        Assert.AreEqual(1, response.Count);
        Assert.AreEqual(2026, response[0].Year);
        Assert.AreEqual(1, response[0].Month);
    }
}
