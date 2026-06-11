using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
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

    private const string Requestinguser = "RequestingUser";
    private const string Pending = "Pending";
    private const string Val59899000000 = "+59899000000";
    private const string Val1234 = "1234";
    private const string Val18DeJulio = "18 de Julio";
    private const string Val2b = "2B";
    private const string Express = "Express";
    private const string Hamburguesa = "Hamburguesa";
    private const string Juan = "Juan";
    private const string Perez = "Perez";
    private const string PizzaNapolitana = "Pizza Napolitana";
    private const string Prepared = "Prepared";
    private const string ElPedidoDebeTenerAlMenosUnProd = "El pedido debe tener al menos un producto.";
    private const string P0001 = "P0001";
    private const string PizzaJpg = "pizza.jpg";
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
            ShippingType = Express,
            Address = new AddressDTO
            {
                Street = Val18DeJulio,
                DoorNumber = Val1234,
                Apartment = Val2b
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
        _controller.HttpContext.Items[Requestinguser] = new User { Id = 1 };

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
            ShippingType = Express,
            Address = new AddressDTO
            {
                Street = Val18DeJulio,
                DoorNumber = Val1234,
                Apartment = Val2b
            },
            Items = []
        };

        _orderServiceMock
            .Setup(s => s.CreateOrder(request, It.IsAny<int>()))
            .Throws(new ArgumentException(ElPedidoDebeTenerAlMenosUnProd));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.HttpContext.Items[Requestinguser] = new User { Id = 1 };

        _controller.CreateOrder(request);
    }

    [TestMethod]
    public void GetOrders_WhenClientRole_ReturnsOk()
    {
        var paginatedResponse = new PaginatedResponse<GetOrdersResponseDTO>();

        _orderServiceMock
            .Setup(s => s.GetOrders(It.IsAny<GetOrdersRequestDTO>(), It.IsAny<UserRole>(), It.IsAny<int?>()))
            .Returns(paginatedResponse);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.HttpContext.Items[Requestinguser] = new User { Id = 1, Role = UserRole.Client };

        var result = _controller.GetOrders(new GetOrdersRequestDTO());

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

        var paginatedResponse = new PaginatedResponse<GetOrdersResponseDTO>
        {
            Items =
            [
                new GetOrdersResponseDTO
                {
                    OrderId = 1,
                    Client = new ClientInfoDTO
                    {
                        Id = 1,
                        Name = Juan,
                        LastName = Perez,
                        Phone = Val59899000000
                    },
                    Date = new DateTime(2026, 1, 10),
                    Status = Pending,
                    Items = [new OrderItemResponseDTO { ProductName = Hamburguesa, Quantity = 2 }]
                }

            ],
            TotalCount = 1,
            Page = 1,
            PageSize = 20
        };

        _orderServiceMock
            .Setup(s => s.GetOrders(request, It.IsAny<UserRole>(), It.IsAny<int?>()))
            .Returns(paginatedResponse);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.HttpContext.Items[Requestinguser] = new User { Id = 1, Role = UserRole.Administrative };

        var result = _controller.GetOrders(request);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));

        var okResult = (OkObjectResult)result;
        var response = (PaginatedResponse<GetOrdersResponseDTO>)okResult.Value!;

        Assert.AreEqual(paginatedResponse.TotalCount, response.TotalCount);
        Assert.AreEqual(paginatedResponse.Items[0].OrderId, response.Items[0].OrderId);
        Assert.AreEqual(paginatedResponse.Items[0].Client.Id, response.Items[0].Client.Id);
        Assert.AreEqual(paginatedResponse.Items[0].Client.Name, response.Items[0].Client.Name);
        Assert.AreEqual(paginatedResponse.Items[0].Client.LastName, response.Items[0].Client.LastName);
    }

    [TestMethod]
    public void GetOrderDetail_WhenCalled_ReturnsOk()
    {
        var response = new OrderDetailResponseDTO
        {
            OrderId = 1,
            ClientId = 1,
            Status = Pending,
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
            GeneralTotal = 1026.0,
            TotalCount = 1,
            Page = 1,
            PageSize = 20
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
                Code = P0001,
                Name = PizzaNapolitana,
                Quantity = 10,
                Images = PizzaJpg
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
        Assert.AreEqual(PizzaNapolitana, response[0].Name);
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
                    Name = Juan,
                    LastName = Perez,
                    Phone = Val59899000000
                },
                Date = new DateTime(2026, 1, 10),
                Status = Pending,
                Items = [new OrderItemResponseDTO { ProductName = Hamburguesa, Quantity = 2 }]
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
    public void ChangeStatus_ValidRequest_ReturnsOkWithUpdatedStatus()
    {
        var orderId = 1;
        var request = new ChangeOrderStatusRequestDTO { Status = OrderStatus.Prepared };

        var expectedResponse = new UpdateOrderStatusResponseDTO
        {
            OrderId = orderId,
            Status = Prepared,
            UpdatedAt = DateTime.Now
        };

        _orderServiceMock
            .Setup(s => s.ChangeStatus(orderId, OrderStatus.Prepared, It.IsAny<UserRole>()))
            .Returns(expectedResponse);

        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        _controller.HttpContext.Items[Requestinguser] = new User { Id = 1, Role = UserRole.Dispatcher };

        var result = _controller.ChangeStatus(orderId, request);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        var response = (UpdateOrderStatusResponseDTO)okResult.Value!;
        Assert.AreEqual(Prepared, response.Status);
        Assert.AreEqual(orderId, response.OrderId);
    }
}
