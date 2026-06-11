using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Models;
using DarkKitchen.Domain.States;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogicTest.Services;

[TestClass]
public class OrderServiceTest
{
    private Mock<IOrderRepository> _orderRepositoryMock = null!;
    private Mock<IPricingService> _pricingServiceMock = null!;
    private Mock<IRepository<User>> _userRepositoryMock = null!;
    private Mock<IRepository<ShippingType>> _shippingTypeRepositoryMock = null!;
    private OrderService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _pricingServiceMock = new Mock<IPricingService>();
        _userRepositoryMock = new Mock<IRepository<User>>();
        _shippingTypeRepositoryMock = new Mock<IRepository<ShippingType>>();

        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                            .Returns(new User { Id = 1, Role = UserRole.Client });

        _shippingTypeRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<ShippingType, bool>>>()))
                            .Returns(new ShippingType { Id = 1, Name = "Express", Cost = 50.0 });

        _service = new OrderService(
            _orderRepositoryMock.Object,
            _pricingServiceMock.Object,
            _userRepositoryMock.Object,
            _shippingTypeRepositoryMock.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateOrder_EmptyItems_ThrowsException()
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

        _service.CreateOrder(request, 1);
    }

    [TestMethod]
    public void GetOrders_WhenClientRole_ReturnsOrders()
    {
        var order = new Order
        {
            Id = 1,
            ClientId = 1,
            Status = OrderStatus.Pending,
            Client = new User { Id = 1, Name = "Juan", LastName = "Perez", Phone = "+59899000000" },
            Items = [new OrderItem { ProductId = 1, Quantity = 2, Product = new Product { Id = 1, Price = 100.0 } }]
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrders(1, It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<OrderStatus?>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(([order], 1));

        var request = new GetOrdersRequestDTO();

        var result = _service.GetOrders(request, UserRole.Client, 1);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Items.Count);
    }

    [TestMethod]
    public void GetOrders_ValidRequest_ReturnsMappedOrders()
    {
        var request = new GetOrdersRequestDTO
        {
            DateFrom = new DateTime(2026, 1, 1),
            DateTo = new DateTime(2026, 1, 31)
        };

        var ordersFromRepo = new List<Order>
        {
            new Order
            {
                Id = 1,
                ClientId = 10,
                Client = new User
                {
                    Id = 10,
                    Name = "Juan",
                    LastName = "Perez",
                    Email = "juan@test.com",
                    Phone = "+59899000000"
                },
                Date = new DateTime(2026, 1, 10),
                Status = OrderStatus.Pending,
                Street = "18 de Julio",
                DoorNumber = "1234",
                Items =
                [
                    new OrderItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        Product = new Product { Name = "Hamburguesa" }
                    }

                ]
            }
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrders(null, It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<OrderStatus?>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns((ordersFromRepo, ordersFromRepo.Count));

        var result = _service.GetOrders(request, UserRole.Administrative, null);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Items.Count);
        Assert.AreEqual(1, result.Items[0].OrderId);
        Assert.AreEqual("Pending", result.Items[0].Status);
        Assert.AreEqual("Hamburguesa", result.Items[0].Items[0].ProductName);
        Assert.AreEqual(2, result.Items[0].Items[0].Quantity);
    }

    [TestMethod]
    [ExpectedException(typeof(NotFoundException))]
    public void CreateOrder_ClientNotFound_ThrowsException()
    {
        _userRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns((User)null!);

        var request = new CreateOrderRequestDTO
        {
            ClientId = 99,
            ShippingType = "Express",
            Address = new AddressDTO { Street = "18 de Julio", DoorNumber = "1234" },
            Items = [new OrderItemRequestDTO { ProductId = 1, Quantity = 1 }]
        };

        _service.CreateOrder(request, 1);
    }

    [TestMethod]
    public void MarkAsPrepared_PendingOrder_ReturnsUpdatedStatus()
    {
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.Pending
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderById(1))
            .Returns(order);

        _orderRepositoryMock
            .Setup(r => r.Update(It.IsAny<Order>()))
            .Returns(order);

        var result = _service.MarkAsPrepared(1);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.OrderId);
        Assert.AreEqual("Prepared", result.Status);
    }

    [TestMethod]
    [ExpectedException(typeof(NotFoundException))]
    public void MarkAsPrepared_OrderNotFound_ThrowsException()
    {
        _orderRepositoryMock
            .Setup(r => r.GetOrderById(99))
            .Returns((Order)null!);

        _service.MarkAsPrepared(99);
    }

    [TestMethod]
    [ExpectedException(typeof(ConflictException))]
    public void MarkAsPrepared_OrderNotPending_ThrowsException()
    {
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.Prepared
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderById(1))
            .Returns(order);

        _service.MarkAsPrepared(1);
    }

    [TestMethod]
    public void GetOrderDetail_WhenOrderExists_ReturnsDetail()
    {
        var product = new Product { Id = 1, Name = "Pizza napolitana", Price = 100.0 };
        var order = new Order
        {
            Id = 1,
            ClientId = 1,
            Status = OrderStatus.Pending,
            ShippingTypeId = 1,
            Date = DateTime.Now,
            Items = [new OrderItem { ProductId = 1, Quantity = 2, Product = product }]
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderById(1))
            .Returns(order);

        var result = _service.GetOrderDetail(1);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.OrderId);
        Assert.AreEqual("Pending", result.Status);
        Assert.AreEqual(1, result.Items.Count);
    }

    [TestMethod]
    public void DeliverOrder_WhenOrderIsOnTheWay_ReturnsDelivered()
    {
        var order = new Order
        {
            Id = 1,
            ClientId = 1,
            Status = OrderStatus.OnTheWay,
            ShippingTypeId = 1,
            Items = []
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderById(1))
            .Returns(order);

        _orderRepositoryMock
            .Setup(r => r.Update(It.IsAny<Order>()))
            .Returns(order);

        var result = _service.DeliverOrder(1);

        Assert.IsNotNull(result);
        Assert.AreEqual("Delivered", result.Status);
    }

    [TestMethod]
    [ExpectedException(typeof(NotFoundException))]
    public void DeliverOrder_WhenOrderNotFound_ThrowsException()
    {
        _orderRepositoryMock
            .Setup(r => r.GetOrderById(It.IsAny<int>()))
            .Returns((Order?)null);

        _service.DeliverOrder(999);
    }

    [TestMethod]
    [ExpectedException(typeof(ConflictException))]
    public void DeliverOrder_WhenOrderIsNotOnTheWay_ThrowsException()
    {
        var order = new Order
        {
            Id = 1,
            ClientId = 1,
            Status = OrderStatus.Pending,
            ShippingTypeId = 1,
            Items = []
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderById(1))
            .Returns(order);

        _service.DeliverOrder(1);
    }

    [TestMethod]
    public void CancelOrder_PendingOrder_ReturnsUpdatedStatus()
    {
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.Pending
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderById(1))
            .Returns(order);

        _orderRepositoryMock
            .Setup(r => r.Update(It.IsAny<Order>()))
            .Returns(order);

        var result = _service.CancelOrder(1);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.OrderId);
        Assert.AreEqual("Cancelled", result.Status);
    }

    [TestMethod]
    [ExpectedException(typeof(NotFoundException))]
    public void CancelOrder_OrderNotFound_ThrowsException()
    {
        _orderRepositoryMock
            .Setup(r => r.GetOrderById(99))
            .Returns((Order)null!);

        _service.CancelOrder(99);
    }

    [TestMethod]
    [ExpectedException(typeof(ConflictException))]
    public void CancelOrder_OrderNotPending_ThrowsException()
    {
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.Prepared
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderById(1))
            .Returns(order);

        _service.CancelOrder(1);
    }

    [TestMethod]
    [ExpectedException(typeof(NotFoundException))]
    public void MarkAsOnTheWay_OrderNotFound_ThrowsException()
    {
        _orderRepositoryMock
            .Setup(r => r.GetOrderById(99))
            .Returns((Order)null!);

        _service.MarkAsOnTheWay(99);
    }

    [TestMethod]
    [ExpectedException(typeof(ConflictException))]
    public void MarkAsOnTheWay_OrderNotPrepared_ThrowsException()
    {
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.Pending
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderById(1))
            .Returns(order);

        _service.MarkAsOnTheWay(1);
    }

    [TestMethod]
    public void MarkAsOnTheWay_PreparedOrder_ReturnsOnTheWay()
    {
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.Prepared
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderById(1))
            .Returns(order);

        _orderRepositoryMock
            .Setup(r => r.Update(It.IsAny<Order>()))
            .Returns(order);

        var result = _service.MarkAsOnTheWay(1);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.OrderId);
        Assert.AreEqual("OnTheWay", result.Status);
    }

    [TestMethod]
    public void MarkAsNotDelivered_OnTheWayOrder_ReturnsUpdatedStatus()
    {
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.OnTheWay
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderById(1))
            .Returns(order);

        _orderRepositoryMock
            .Setup(r => r.Update(It.IsAny<Order>()))
            .Returns(order);

        var result = _service.MarkAsNotDelivered(1);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.OrderId);
        Assert.AreEqual("NotDelivered", result.Status);
    }

    [TestMethod]
    [ExpectedException(typeof(NotFoundException))]
    public void MarkAsNotDelivered_OrderNotFound_ThrowsException()
    {
        _orderRepositoryMock
            .Setup(r => r.GetOrderById(99))
            .Returns((Order)null!);

        _service.MarkAsNotDelivered(99);
    }

    [TestMethod]
    [ExpectedException(typeof(ConflictException))]
    public void MarkAsNotDelivered_OrderNotOnTheWay_ThrowsException()
    {
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.Pending
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderById(1))
            .Returns(order);

        _service.MarkAsNotDelivered(1);
    }

    [TestMethod]
    public void GetTopProducts_ValidRequest_ReturnsTopProducts()
    {
        var dateFrom = new DateTime(2026, 1, 1);
        var dateTo = new DateTime(2026, 1, 31);

        var product = new Product
        {
            Id = 1,
            Code = "P0001",
            Name = "Pizza Napolitana",
            Images = "/9j/2Q=="
        };

        var topProducts = new List<(Product Product, int Quantity)>
        {
            (product, 10)
        };

        _orderRepositoryMock
            .Setup(r => r.GetTopProducts(
                It.IsAny<Expression<Func<Order, bool>>>(),
                5))
            .Returns(topProducts);

        var result = _service.GetTopProducts(dateFrom, dateTo);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Pizza Napolitana", result[0].Name);
        Assert.AreEqual(10, result[0].Quantity);
    }

    [TestMethod]
    public void GetSalesReport_ValidRequest_ReturnsSalesReport()
    {
        var expectedReport = new List<SalesReportItem>
        {
            new() { Year = 2026, Month = 1, ClientId = 1, ClientName = "Juan Perez", Total = 500.0 },
            new() { Year = 2026, Month = 1, ClientId = 2, ClientName = "Juan Perez", Total = 300.0 }
        };

        _orderRepositoryMock
            .Setup(r => r.GetSalesReport(1, 20))
            .Returns((expectedReport, expectedReport.Count));

        var result = _service.GetSalesReport(1, 20);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Months.Count);
        Assert.AreEqual(2026, result.Months[0].Year);
        Assert.AreEqual(1, result.Months[0].Month);
        Assert.AreEqual(2, result.Months[0].Clients.Count);
        Assert.AreEqual(500.0, result.Months[0].Clients[0].Total);
        Assert.AreEqual(300.0, result.Months[0].Clients[1].Total);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateOrder_WhenStreetIsEmpty_ThrowsException()
    {
        var request = new CreateOrderRequestDTO
        {
            ClientId = 1,
            ShippingType = "Express",
            Address = new AddressDTO
            {
                Street = string.Empty,
                DoorNumber = "1234",
                Apartment = "2B"
            },
            Items = [new OrderItemRequestDTO { ProductId = 1, Quantity = 1 }]
        };

        _pricingServiceMock
            .Setup(s => s.CalculateOrderPricing(It.IsAny<List<OrderItemRequestDTO>>(), It.IsAny<ShippingType>()))
            .Returns(new PricingResult
            {
                Items = [new OrderItem { ProductId = 1, Quantity = 1 }],
                Subtotal = 100.0,
                ShippingCost = 50.0,
                Total = 172.0
            });

        _service.CreateOrder(request, 1);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateOrder_WhenDoorNumberIsEmpty_ThrowsException()
    {
        var request = new CreateOrderRequestDTO
        {
            ClientId = 1,
            ShippingType = "Express",
            Address = new AddressDTO
            {
                Street = "18 de Julio",
                DoorNumber = string.Empty,
                Apartment = "2B"
            },
            Items = [new OrderItemRequestDTO { ProductId = 1, Quantity = 1 }]
        };

        _pricingServiceMock
            .Setup(s => s.CalculateOrderPricing(It.IsAny<List<OrderItemRequestDTO>>(), It.IsAny<ShippingType>()))
            .Returns(new PricingResult
            {
                Items = [new OrderItem { ProductId = 1, Quantity = 1 }],
                Subtotal = 100.0,
                ShippingCost = 50.0,
                Total = 172.0
            });

        _service.CreateOrder(request, 1);
    }

    [TestMethod]
    public void GetOrders_WhenOrderHasClient_MapsClientInfoCorrectly()
    {
        var client = new User
        {
            Id = 42,
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899000000"
        };
        var order = new Order
        {
            Id = 1,
            ClientId = 42,
            Client = client,
            Date = new DateTime(2026, 4, 20),
            Status = OrderStatus.Pending,
            Street = "Rivera",
            DoorNumber = "1234",
            Items = []
        };

        _ = _orderRepositoryMock
            .Setup(r => r.GetOrders(
                It.IsAny<int?>(),
                It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string?>(),
                It.IsAny<OrderStatus?>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(([order], 1));

        var result = _service.GetOrders(new GetOrdersRequestDTO
        {
            DateFrom = new DateTime(2026, 4, 1),
            DateTo = new DateTime(2026, 4, 30)
        }, UserRole.Administrative, null);

        Assert.AreEqual(1, result.Items.Count);
        Assert.AreEqual(42, result.Items[0].Client.Id);
        Assert.AreEqual("Juan", result.Items[0].Client.Name);
        Assert.AreEqual("Perez", result.Items[0].Client.LastName);
        Assert.AreEqual("+59899000000", result.Items[0].Client.Phone);
    }

    [TestMethod]
    public void CreateOrder_ValidRequest_FreezesUnitPriceInOrderItem()
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
            Items = [new OrderItemRequestDTO { ProductId = 1, Quantity = 2 }]
        };

        _pricingServiceMock
            .Setup(s => s.CalculateOrderPricing(It.IsAny<List<OrderItemRequestDTO>>(), It.IsAny<ShippingType>()))
            .Returns(new PricingResult
            {
                Items = [new OrderItem { ProductId = 1, Quantity = 2, UnitPrice = 100.0 }],
                Subtotal = 200.0,
                Discount = 0,
                ShippingCost = 50.0,
                Vat = 44.0,
                Total = 294.0
            });

        Order? savedOrder = null;
        _orderRepositoryMock
            .Setup(r => r.Add(It.IsAny<Order>()))
            .Returns((Order o) =>
            {
                savedOrder = o;
                o.Id = 1;
                return o;
            });

        _service.CreateOrder(request, 1);

        Assert.IsNotNull(savedOrder);
        Assert.AreEqual(100.0, savedOrder.Items[0].UnitPrice);
    }

    [TestMethod]
    public void CreateOrder_ValidRequest_PersistsSubtotal()
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
            Items = [new OrderItemRequestDTO { ProductId = 1, Quantity = 2 }]
        };

        _pricingServiceMock
            .Setup(s => s.CalculateOrderPricing(It.IsAny<List<OrderItemRequestDTO>>(), It.IsAny<ShippingType>()))
            .Returns(new PricingResult
            {
                Items = [new OrderItem { ProductId = 1, Quantity = 2, UnitPrice = 100.0 }],
                Subtotal = 200.0,
                Discount = 0,
                ShippingCost = 50.0,
                Vat = 44.0,
                Total = 294.0
            });

        Order? savedOrder = null;
        _orderRepositoryMock
            .Setup(r => r.Add(It.IsAny<Order>()))
            .Returns((Order o) =>
            {
                savedOrder = o;
                o.Id = 1;
                return o;
            });

        _service.CreateOrder(request, 1);

        Assert.IsNotNull(savedOrder);
        Assert.AreEqual(200.0, savedOrder.Subtotal);
    }

    [TestMethod]
    public void CreateOrder_WithPromotion_PersistsDiscount()
    {
        var request = new CreateOrderRequestDTO
        {
            ClientId = 1,
            ShippingType = "Standard",
            Address = new AddressDTO { Street = "18 de Julio", DoorNumber = "1234", Apartment = "2B" },
            Items = [new OrderItemRequestDTO { ProductId = 1, Quantity = 2 }]
        };

        _pricingServiceMock
            .Setup(s => s.CalculateOrderPricing(It.IsAny<List<OrderItemRequestDTO>>(), It.IsAny<ShippingType>()))
            .Returns(new PricingResult
            {
                Items = [new OrderItem { ProductId = 1, Quantity = 2, UnitPrice = 100.0 }],
                Subtotal = 200.0,
                Discount = 20.0,
                ShippingCost = 30.0,
                Vat = 39.6,
                Total = 249.6
            });

        Order? savedOrder = null;
        _orderRepositoryMock
            .Setup(r => r.Add(It.IsAny<Order>()))
            .Returns((Order o) =>
            {
                savedOrder = o;
                o.Id = 1;
                return o;
            });

        _service.CreateOrder(request, 1);

        Assert.IsNotNull(savedOrder);
        Assert.AreEqual(20.0, savedOrder.Discount);
    }

    [TestMethod]
    public void CreateOrder_ExpressDelivery_PersistsShippingCost()
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
            Items = [new OrderItemRequestDTO { ProductId = 1, Quantity = 2 }]
        };

        _pricingServiceMock
            .Setup(s => s.CalculateOrderPricing(It.IsAny<List<OrderItemRequestDTO>>(), It.IsAny<ShippingType>()))
            .Returns(new PricingResult
            {
                Items = [new OrderItem { ProductId = 1, Quantity = 2, UnitPrice = 100.0 }],
                Subtotal = 200.0,
                Discount = 0,
                ShippingCost = 50.0,
                Vat = 44.0,
                Total = 294.0
            });

        Order? savedOrder = null;
        _orderRepositoryMock
            .Setup(r => r.Add(It.IsAny<Order>()))
            .Returns((Order o) =>
            {
                savedOrder = o;
                o.Id = 1;
                return o;
            });

        _service.CreateOrder(request, 1);

        Assert.IsNotNull(savedOrder);
        Assert.AreEqual(50.0, savedOrder.ShippingCost);
    }

    [TestMethod]
    public void CreateOrder_ValidRequest_PersistsVat()
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
            Items = [new OrderItemRequestDTO { ProductId = 1, Quantity = 2 }]
        };

        _pricingServiceMock
            .Setup(s => s.CalculateOrderPricing(It.IsAny<List<OrderItemRequestDTO>>(), It.IsAny<ShippingType>()))
            .Returns(new PricingResult
            {
                Items = [new OrderItem { ProductId = 1, Quantity = 2, UnitPrice = 100.0 }],
                Subtotal = 200.0,
                Discount = 0,
                ShippingCost = 50.0,
                Vat = 44.0,
                Total = 294.0
            });

        Order? savedOrder = null;
        _orderRepositoryMock
            .Setup(r => r.Add(It.IsAny<Order>()))
            .Returns((Order o) =>
            {
                savedOrder = o;
                o.Id = 1;
                return o;
            });

        _service.CreateOrder(request, 1);

        Assert.IsNotNull(savedOrder);
        Assert.AreEqual(44.0, savedOrder.Vat);
    }

    [TestMethod]
    public void CreateOrder_ValidRequest_PersistsTotal()
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
            Items = [new OrderItemRequestDTO { ProductId = 1, Quantity = 2 }]
        };

        _pricingServiceMock
            .Setup(s => s.CalculateOrderPricing(It.IsAny<List<OrderItemRequestDTO>>(), It.IsAny<ShippingType>()))
            .Returns(new PricingResult
            {
                Items = [new OrderItem { ProductId = 1, Quantity = 2, UnitPrice = 100.0 }],
                Subtotal = 200.0,
                Discount = 0,
                ShippingCost = 50.0,
                Vat = 44.0,
                Total = 294.0
            });

        Order? savedOrder = null;
        _orderRepositoryMock
            .Setup(r => r.Add(It.IsAny<Order>()))
            .Returns((Order o) =>
            {
                savedOrder = o;
                o.Id = 1;
                return o;
            });

        _service.CreateOrder(request, 1);

        Assert.IsNotNull(savedOrder);
        Assert.AreEqual(294.0, savedOrder.Total);
    }

    [TestMethod]
    public void GetOrders_WhenClientRole_ReturnsPersistedTotal()
    {
        var order = new Order
        {
            Id = 1,
            ClientId = 1,
            Status = OrderStatus.Pending,
            Total = 500.0,
            Client = new User { Id = 1, Name = "Juan", LastName = "Perez", Phone = "+59899000000" },
            Items = [new OrderItem { ProductId = 1, Quantity = 2, Product = new Product { Id = 1, Price = 100.0 } }]
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrders(1, It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<OrderStatus?>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(([order], 1));

        var request = new GetOrdersRequestDTO();

        var result = _service.GetOrders(request, UserRole.Client, 1);

        Assert.AreEqual(500.0, result.Items[0].Total);
    }

    [TestMethod]
    public void GetOrderDetail_WhenOrderExists_ReturnsPersistedTotal()
    {
        var product = new Product { Id = 1, Name = "Pizza napolitana", Price = 100.0 };
        var order = new Order
        {
            Id = 1,
            ClientId = 1,
            Status = OrderStatus.Pending,
            ShippingTypeId = 1,
            Date = DateTime.Now,
            Total = 500.0,
            Items = [new OrderItem { ProductId = 1, Quantity = 2, Product = product }]
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderById(1))
            .Returns(order);

        var result = _service.GetOrderDetail(1);

        Assert.AreEqual(500.0, result.Total);
    }

    [TestMethod]
    public void GetOrderDetail_WhenOrderExists_ReturnsPersistedUnitPrice()
    {
        var product = new Product { Id = 1, Name = "Pizza napolitana", Price = 150.0 };
        var order = new Order
        {
            Id = 1,
            ClientId = 1,
            Status = OrderStatus.Pending,
            ShippingTypeId = 1,
            Date = DateTime.Now,
            Total = 500.0,
            Items = [new OrderItem { ProductId = 1, Quantity = 2, UnitPrice = 100.0, Product = product }]
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderById(1))
            .Returns(order);

        var result = _service.GetOrderDetail(1);

        Assert.AreEqual(100.0, result.Items[0].UnitPrice);
        Assert.AreEqual(200.0, result.Items[0].Subtotal);
    }

    [TestMethod]
    public void GetSalesReport_ReturnsMonthlyTotal()
    {
        var reportData = new List<SalesReportItem>
        {
            new() { Year = 2026, Month = 4, ClientId = 1, ClientName = "Juan Perez", Total = 500.0 },
            new() { Year = 2026, Month = 4, ClientId = 2, ClientName = "Maria Lopez", Total = 300.0 }
        };
        _orderRepositoryMock
            .Setup(r => r.GetSalesReport(1, 20))
            .Returns((reportData, reportData.Count));

        var result = _service.GetSalesReport(1, 20);

        Assert.AreEqual(800.0, result.Months[0].MonthlyTotal);
    }

    [TestMethod]
    public void MarkAsDelayed_PendingOrder_ReturnsDelayedStatus()
    {
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.Pending
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderById(1))
            .Returns(order);

        _orderRepositoryMock
            .Setup(r => r.Update(It.IsAny<Order>()))
            .Returns(order);

        var result = _service.MarkAsDelayed(1);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.OrderId);
        Assert.AreEqual("Delayed", result.Status);
    }

    [TestMethod]
    [ExpectedException(typeof(ConflictException))]
    public void MarkAsDelayed_OrderNotPending_ThrowsException()
    {
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.Prepared
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderById(1))
            .Returns(order);

        _service.MarkAsDelayed(1);
    }

    [TestMethod]
    public void MarkAsPrepared_DelayedOrder_ReturnsPreparedStatus()
    {
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.Delayed
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderById(1))
            .Returns(order);

        _orderRepositoryMock
            .Setup(r => r.Update(It.IsAny<Order>()))
            .Returns(order);

        var result = _service.MarkAsPrepared(1);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.OrderId);
        Assert.AreEqual("Prepared", result.Status);
    }

    [TestMethod]
    public void CancelOrder_DelayedOrder_ReturnsCancelledStatus()
    {
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.Delayed
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderById(1))
            .Returns(order);

        _orderRepositoryMock
            .Setup(r => r.Update(It.IsAny<Order>()))
            .Returns(order);

        var result = _service.CancelOrder(1);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.OrderId);
        Assert.AreEqual("Cancelled", result.Status);
    }

    [TestMethod]
    [ExpectedException(typeof(ConflictException))]
    public void MarkAsPrepared_CancelledOrder_ThrowsConflictException()
    {
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.Cancelled
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderById(1))
            .Returns(order);

        _service.MarkAsPrepared(1);
    }

    [TestMethod]
    [ExpectedException(typeof(ConflictException))]
    public void MarkAsPrepared_DeliveredOrder_ThrowsConflictException()
    {
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.Delivered
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderById(1))
            .Returns(order);

        _service.MarkAsPrepared(1);
    }

    [TestMethod]
    [ExpectedException(typeof(ConflictException))]
    public void MarkAsPrepared_NotDeliveredOrder_ThrowsConflictException()
    {
        var order = new Order
        {
            Id = 1,
            Status = OrderStatus.NotDelivered
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrderById(1))
            .Returns(order);

        _service.MarkAsPrepared(1);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void OrderStateFactory_InvalidStatus_ThrowsArgumentException()
    {
        OrderStateFactory.Create((OrderStatus)999);
    }

    [TestMethod]
    public void GetDispatcherOrders_WhenCalled_ReturnsMappedOrders()
    {
        var ordersFromRepo = new List<Order>
        {
            new Order
            {
                Id = 1,
                ClientId = 10,
                Client = new User
                {
                    Id = 10,
                    Name = "Juan",
                    LastName = "Perez",
                    Email = "juan@test.com",
                    Phone = "+59899000000"
                },
                Date = new DateTime(2026, 1, 10),
                Status = OrderStatus.Pending,
                Street = "18 de Julio",
                DoorNumber = "1234",
                Items =
                [
                    new OrderItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        Product = new Product { Name = "Hamburguesa" }
                    }

                ]
            }
        };

        _orderRepositoryMock
            .Setup(r => r.GetDispatcherOrders())
            .Returns(ordersFromRepo);

        var result = _service.GetDispatcherOrders();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(1, result[0].OrderId);
        Assert.AreEqual(10, result[0].Client.Id);
        Assert.AreEqual("Juan", result[0].Client.Name);
        Assert.AreEqual("Perez", result[0].Client.LastName);
        Assert.AreEqual("Pending", result[0].Status);
        Assert.AreEqual("Hamburguesa", result[0].Items[0].ProductName);
        Assert.AreEqual(2, result[0].Items[0].Quantity);
    }

    [TestMethod]
    public void PreviewOrder_ValidRequest_DelegatesToPricingService()
    {
        var expectedPreview = new OrderPreviewResponseDTO
        {
            Items =
            [
                new OrderPreviewItemDTO
                {
                    ProductId = 1,
                    ProductName = "Pizza Margherita",
                    Quantity = 1,
                    UnitPrice = 350.0,
                    DiscountPercentage = 35,
                    DiscountedUnitPrice = 227.5,
                    ItemTotal = 227.5,
                },
            ],
            Subtotal = 350.0,
            Discount = 122.5,
            Vat = 50.05,
            ShippingCost = 50.0,
            Total = 327.55
        };

        _pricingServiceMock
            .Setup(s => s.PreviewOrderPricing(It.IsAny<List<OrderItemRequestDTO>>(), It.IsAny<ShippingType>()))
            .Returns(expectedPreview);

        var items = new List<OrderItemRequestDTO>
        {
            new OrderItemRequestDTO { ProductId = 1, Quantity = 1 }
        };

        var result = _service.PreviewOrder(items, "Express");

        Assert.IsNotNull(result);
        Assert.AreEqual(350.0, result.Subtotal);
        Assert.AreEqual(122.5, result.Discount);
        Assert.AreEqual(50.0, result.ShippingCost);
        Assert.AreEqual(1, result.Items.Count);
        Assert.AreEqual(35, result.Items[0].DiscountPercentage);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void PreviewOrder_InvalidShippingType_ThrowsException()
    {
        _shippingTypeRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<ShippingType, bool>>>()))
            .Returns((ShippingType)null!);

        var items = new List<OrderItemRequestDTO>
        {
            new OrderItemRequestDTO { ProductId = 1, Quantity = 1 }
        };

        _service.PreviewOrder(items, "Inexistente");
    }
}
