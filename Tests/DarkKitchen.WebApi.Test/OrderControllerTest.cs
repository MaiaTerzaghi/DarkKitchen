using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
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
            ShippingType = "Express",
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
            .Setup(s => s.CreateOrder(request, It.IsAny<int>()))
            .Returns(expectedResponse);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.HttpContext.Items["RequestingUser"] = new User { Id = 1 };

        var result = _controller.CreateOrder(request);

        Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
        var okResult = (CreatedAtActionResult)result;
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
            ShippingType = "Express",
            Address = new AddressDTO
            {
                Street = "18 de Julio",
                DoorNumber = "1234",
                Apartment = "2B"
            },
            Items = []
        };

        _orderServiceMock
            .Setup(s => s.CreateOrder(request, It.IsAny<int>()))
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
            .Setup(s => s.GetClientOrders(It.IsAny<GetClientOrdersRequestDTO>(), It.IsAny<int>()))
            .Returns(orders);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.HttpContext.Items["RequestingUser"] = new User { Id = 1 };

        var result = _controller.GetClientOrders(new GetClientOrdersRequestDTO());

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
    public void GetSalesReport_ValidRequest_ReturnsOk()
    {
        var expectedResponse = new SalesReportWithTotalDTO
        {
            Months = [
            new SalesReportResponseDTO
            {
                Year = 2026,
                Month = 1,
                Clients = [new ClientSalesDTO { ClientId = 1, Total = 500.0 }]
            }

            ],
            GeneralTotal = 1026.0
        };

        _orderServiceMock
            .Setup(s => s.GetSalesReport(1, 20))
            .Returns(expectedResponse);

        var result = _controller.GetSalesReport(1, 20);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        var response = (SalesReportWithTotalDTO)okResult.Value!;
        Assert.AreEqual(1, response.Months.Count);
        Assert.AreEqual(2026, response.Months[0].Year);
        Assert.AreEqual(1, response.Months[0].Month);
    }

    [TestMethod]
    public void GetTopProducts_ValidRequest_ReturnsOkWithTopProducts()
    {
        var expectedResponse = new List<TopProductResponseDTO>
        {
            new TopProductResponseDTO
            {
                Code = "P0001",
                Name = "Pizza Napolitana",
                Quantity = 10,
                Images = "pizza.jpg"
            }
        };

        _orderServiceMock
            .Setup(s => s.GetTopProducts(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Returns(expectedResponse);

        var result = _controller.GetTopProducts(new DateTime(2026, 1, 1), new DateTime(2026, 1, 31));

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        var response = (List<TopProductResponseDTO>)okResult.Value!;
        Assert.AreEqual(1, response.Count);
        Assert.AreEqual("Pizza Napolitana", response[0].Name);
    }

    [TestMethod]
    public void GetDispatcherOrders_WhenCalled_ReturnsOkWithOrders()
    {
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
            .Setup(s => s.GetDispatcherOrders())
            .Returns(expectedOrders);

        var result = _controller.GetDispatcherOrders();

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        var response = (List<GetOrdersResponseDTO>)okResult.Value!;
        Assert.AreEqual(expectedOrders.Count, response.Count);
        Assert.AreEqual(expectedOrders[0].OrderId, response[0].OrderId);
        Assert.AreEqual(expectedOrders[0].Client.Id, response[0].Client.Id);
        Assert.AreEqual(expectedOrders[0].Client.Name, response[0].Client.Name);
        Assert.AreEqual(expectedOrders[0].Status, response[0].Status);
    }

    [TestMethod]
    public void ChangeStatus_WhenAdminChangesToCancelled_ReturnsOkWithUpdatedStatus()
    {
        var expectedResponse = new UpdateOrderStatusResponseDTO
        {
            OrderId = 1,
            Status = OrderStatus.Cancelled.ToString(),
            UpdatedAt = DateTime.Now
        };

        var orderServiceMock = new Mock<IOrderService>();
        orderServiceMock
            .Setup(s => s.ChangeStatus(1, OrderStatus.Cancelled, "admin@email.com"))
            .Returns(expectedResponse);

        var controller = new OrderController(orderServiceMock.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        controller.HttpContext.Items["RequestingUser"] = new User
        {
            Id = 1,
            Email = "admin@email.com",
            Role = UserRole.Administrative
        };

        var request = new ChangeStatusRequestDTO { Status = OrderStatus.Cancelled };

        var result = controller.ChangeStatus(1, request);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        Assert.AreSame(expectedResponse, okResult.Value);

        orderServiceMock.Verify(
            s => s.ChangeStatus(1, OrderStatus.Cancelled, "admin@email.com"),
            Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(UnauthorizedException))]
    public void ChangeStatus_WhenDispatcherTriesToCancel_ThrowsUnauthorizedException()
    {
        var orderServiceMock = new Mock<IOrderService>();

        var controller = new OrderController(orderServiceMock.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        controller.HttpContext.Items["RequestingUser"] = new User
        {
            Id = 2,
            Email = "dispatcher@email.com",
            Role = UserRole.Dispatcher
        };

        var request = new ChangeStatusRequestDTO { Status = OrderStatus.Cancelled };

        controller.ChangeStatus(1, request);
    }
}
