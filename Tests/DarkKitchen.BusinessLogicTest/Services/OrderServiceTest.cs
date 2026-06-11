using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
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

    private const string Val1234 = "1234";
    private const string Val18DeJulio = "18 de Julio";
    private const string Express = "Express";
    private const string Val2b = "2B";
    private const string Juan = "Juan";
    private const string Perez = "Perez";
    private const string Val59899000000 = "+59899000000";
    private const string Hamburguesa = "Hamburguesa";
    private const string JuanPerez = "Juan Perez";
    private const string Pending = "Pending";
    private const string PizzaNapolitana = "Pizza napolitana";
    private const string JuanTestCom = "juan@test.com";
    private const string Cancelled = "Cancelled";
    private const string PizzaNapolitana2 = "Pizza Napolitana";
    private const string Prepared = "Prepared";
    private const string Delayed = "Delayed";
    private const string Delivered = "Delivered";
    private const string Inexistente = "Inexistente";
    private const string MariaLopez = "Maria Lopez";
    private const string Notdelivered = "NotDelivered";
    private const string Ontheway = "OnTheWay";
    private const string P0001 = "P0001";
    private const string PizzaMargherita = "Pizza Margherita";
    private const string Rivera = "Rivera";
    private const string Standard = "Standard";
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
                            .Returns(new ShippingType { Id = 1, Name = Express, Cost = 50.0 });

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
            ShippingType = Express,
            Address = new AddressDTO
            {
                Street = Val18DeJulio,
                DoorNumber = Val1234,
                Apartment = Val2b
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
            Client = new User { Id = 1, Name = Juan, LastName = Perez, Phone = Val59899000000 },
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
                    Name = Juan,
                    LastName = Perez,
                    Email = JuanTestCom,
                    Phone = Val59899000000
                },
                Date = new DateTime(2026, 1, 10),
                Status = OrderStatus.Pending,
                Street = Val18DeJulio,
                DoorNumber = Val1234,
                Items =
                [
                    new OrderItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        Product = new Product { Name = Hamburguesa }
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
        Assert.AreEqual(Pending, result.Items[0].Status);
        Assert.AreEqual(Hamburguesa, result.Items[0].Items[0].ProductName);
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
            ShippingType = Express,
            Address = new AddressDTO { Street = Val18DeJulio, DoorNumber = Val1234 },
            Items = [new OrderItemRequestDTO { ProductId = 1, Quantity = 1 }]
        };

        _service.CreateOrder(request, 1);
    }

    [TestMethod]
    public void GetOrderDetail_WhenOrderExists_ReturnsDetail()
    {
        var product = new Product { Id = 1, Name = PizzaNapolitana, Price = 100.0 };
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
        Assert.AreEqual(Pending, result.Status);
        Assert.AreEqual(1, result.Items.Count);
    }

    [TestMethod]
    public void GetTopProducts_ValidRequest_ReturnsTopProducts()
    {
        var dateFrom = new DateTime(2026, 1, 1);
        var dateTo = new DateTime(2026, 1, 31);

        var product = new Product
        {
            Id = 1,
            Code = P0001,
            Name = PizzaNapolitana2,
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
        Assert.AreEqual(PizzaNapolitana2, result[0].Name);
        Assert.AreEqual(10, result[0].Quantity);
    }

    [TestMethod]
    public void GetSalesReport_ValidRequest_ReturnsSalesReport()
    {
        var expectedReport = new List<(int Year, int Month, int ClientId, string ClientName, double Total)>
        {
            (2026, 1, 1, JuanPerez, 500.0),
            (2026, 1, 2, JuanPerez, 300.0)
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
            ShippingType = Express,
            Address = new AddressDTO
            {
                Street = string.Empty,
                DoorNumber = Val1234,
                Apartment = Val2b
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
            ShippingType = Express,
            Address = new AddressDTO
            {
                Street = Val18DeJulio,
                DoorNumber = string.Empty,
                Apartment = Val2b
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
            Name = Juan,
            LastName = Perez,
            Email = JuanTestCom,
            Phone = Val59899000000
        };
        var order = new Order
        {
            Id = 1,
            ClientId = 42,
            Client = client,
            Date = new DateTime(2026, 4, 20),
            Status = OrderStatus.Pending,
            Street = Rivera,
            DoorNumber = Val1234,
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
        Assert.AreEqual(Juan, result.Items[0].Client.Name);
        Assert.AreEqual(Perez, result.Items[0].Client.LastName);
        Assert.AreEqual(Val59899000000, result.Items[0].Client.Phone);
    }

    [TestMethod]
    public void CreateOrder_ValidRequest_FreezesUnitPriceInOrderItem()
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
            ShippingType = Express,
            Address = new AddressDTO
            {
                Street = Val18DeJulio,
                DoorNumber = Val1234,
                Apartment = Val2b
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
            ShippingType = Standard,
            Address = new AddressDTO { Street = Val18DeJulio, DoorNumber = Val1234, Apartment = Val2b },
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
            ShippingType = Express,
            Address = new AddressDTO
            {
                Street = Val18DeJulio,
                DoorNumber = Val1234,
                Apartment = Val2b
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
            ShippingType = Express,
            Address = new AddressDTO
            {
                Street = Val18DeJulio,
                DoorNumber = Val1234,
                Apartment = Val2b
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
            ShippingType = Express,
            Address = new AddressDTO
            {
                Street = Val18DeJulio,
                DoorNumber = Val1234,
                Apartment = Val2b
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
            Client = new User { Id = 1, Name = Juan, LastName = Perez, Phone = Val59899000000 },
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
        var product = new Product { Id = 1, Name = PizzaNapolitana, Price = 100.0 };
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
        var product = new Product { Id = 1, Name = PizzaNapolitana, Price = 150.0 };
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
        var reportData = new List<(int Year, int Month, int ClientId, string ClientName, double Total)>
        {
            (2026, 4, 1, JuanPerez, 500.0),
            (2026, 4, 2, MariaLopez, 300.0)
        };
        _orderRepositoryMock
            .Setup(r => r.GetSalesReport(1, 20))
            .Returns((reportData, reportData.Count));

        var result = _service.GetSalesReport(1, 20);

        Assert.AreEqual(800.0, result.Months[0].MonthlyTotal);
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
                    Name = Juan,
                    LastName = Perez,
                    Email = JuanTestCom,
                    Phone = Val59899000000
                },
                Date = new DateTime(2026, 1, 10),
                Status = OrderStatus.Pending,
                Street = Val18DeJulio,
                DoorNumber = Val1234,
                Items =
                [
                    new OrderItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        Product = new Product { Name = Hamburguesa }
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
        Assert.AreEqual(Juan, result[0].Client.Name);
        Assert.AreEqual(Perez, result[0].Client.LastName);
        Assert.AreEqual(Pending, result[0].Status);
        Assert.AreEqual(Hamburguesa, result[0].Items[0].ProductName);
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
                    ProductName = PizzaMargherita,
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

        var result = _service.PreviewOrder(items, Express);

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

        _service.PreviewOrder(items, Inexistente);
    }

    [TestMethod]
    public void ChangeStatus_PendingToPrepared_ByDispatcher_ReturnsUpdated()
    {
        var order = new Order { Id = 1, Status = OrderStatus.Pending };
        _orderRepositoryMock.Setup(r => r.GetOrderById(1)).Returns(order);
        _orderRepositoryMock.Setup(r => r.Update(It.IsAny<Order>())).Returns(order);

        var result = _service.ChangeStatus(1, OrderStatus.Prepared, UserRole.Dispatcher);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.OrderId);
        Assert.AreEqual(Prepared, result.Status);
    }

    [TestMethod]
    public void ChangeStatus_PendingToCancelled_ByAdministrative_ReturnsUpdated()
    {
        var order = new Order { Id = 1, Status = OrderStatus.Pending };
        _orderRepositoryMock.Setup(r => r.GetOrderById(1)).Returns(order);
        _orderRepositoryMock.Setup(r => r.Update(It.IsAny<Order>())).Returns(order);
        Assert.AreEqual(Cancelled, _service.ChangeStatus(1, OrderStatus.Cancelled, UserRole.Administrative).Status);
    }

    [TestMethod]
    public void ChangeStatus_PendingToDelayed_ByDispatcher_ReturnsUpdated()
    {
        var order = new Order { Id = 1, Status = OrderStatus.Pending };
        _orderRepositoryMock.Setup(r => r.GetOrderById(1)).Returns(order);
        _orderRepositoryMock.Setup(r => r.Update(It.IsAny<Order>())).Returns(order);
        Assert.AreEqual(Delayed, _service.ChangeStatus(1, OrderStatus.Delayed, UserRole.Dispatcher).Status);
    }

    [TestMethod]
    public void ChangeStatus_DelayedToPrepared_ByDispatcher_ReturnsUpdated()
    {
        var order = new Order { Id = 1, Status = OrderStatus.Delayed };
        _orderRepositoryMock.Setup(r => r.GetOrderById(1)).Returns(order);
        _orderRepositoryMock.Setup(r => r.Update(It.IsAny<Order>())).Returns(order);
        Assert.AreEqual(Prepared, _service.ChangeStatus(1, OrderStatus.Prepared, UserRole.Dispatcher).Status);
    }

    [TestMethod]
    public void ChangeStatus_DelayedToCancelled_ByAdministrative_ReturnsUpdated()
    {
        var order = new Order { Id = 1, Status = OrderStatus.Delayed };
        _orderRepositoryMock.Setup(r => r.GetOrderById(1)).Returns(order);
        _orderRepositoryMock.Setup(r => r.Update(It.IsAny<Order>())).Returns(order);
        Assert.AreEqual(Cancelled, _service.ChangeStatus(1, OrderStatus.Cancelled, UserRole.Administrative).Status);
    }

    [TestMethod]
    public void ChangeStatus_PreparedToOnTheWay_ByDispatcher_ReturnsUpdated()
    {
        var order = new Order { Id = 1, Status = OrderStatus.Prepared };
        _orderRepositoryMock.Setup(r => r.GetOrderById(1)).Returns(order);
        _orderRepositoryMock.Setup(r => r.Update(It.IsAny<Order>())).Returns(order);
        Assert.AreEqual(Ontheway, _service.ChangeStatus(1, OrderStatus.OnTheWay, UserRole.Dispatcher).Status);
    }

    [TestMethod]
    public void ChangeStatus_OnTheWayToDelivered_ByDispatcher_ReturnsUpdated()
    {
        var order = new Order { Id = 1, Status = OrderStatus.OnTheWay };
        _orderRepositoryMock.Setup(r => r.GetOrderById(1)).Returns(order);
        _orderRepositoryMock.Setup(r => r.Update(It.IsAny<Order>())).Returns(order);
        Assert.AreEqual(Delivered, _service.ChangeStatus(1, OrderStatus.Delivered, UserRole.Dispatcher).Status);
    }

    [TestMethod]
    public void ChangeStatus_OnTheWayToNotDelivered_ByDispatcher_ReturnsUpdated()
    {
        var order = new Order { Id = 1, Status = OrderStatus.OnTheWay };
        _orderRepositoryMock.Setup(r => r.GetOrderById(1)).Returns(order);
        _orderRepositoryMock.Setup(r => r.Update(It.IsAny<Order>())).Returns(order);
        Assert.AreEqual(Notdelivered, _service.ChangeStatus(1, OrderStatus.NotDelivered, UserRole.Dispatcher).Status);
    }

    [TestMethod]
    [ExpectedException(typeof(NotFoundException))]
    public void ChangeStatus_OrderNotFound_ThrowsNotFound()
    {
        _orderRepositoryMock.Setup(r => r.GetOrderById(99)).Returns((Order)null!);
        _service.ChangeStatus(99, OrderStatus.Prepared, UserRole.Dispatcher);
    }

    [TestMethod]
    [ExpectedException(typeof(ConflictException))]
    public void ChangeStatus_PrepareDeliveredOrder_ThrowsConflict()
    {
        var order = new Order { Id = 1, Status = OrderStatus.Delivered };
        _orderRepositoryMock.Setup(r => r.GetOrderById(1)).Returns(order);
        _service.ChangeStatus(1, OrderStatus.Prepared, UserRole.Dispatcher);
    }

    [TestMethod]
    [ExpectedException(typeof(ConflictException))]
    public void ChangeStatus_CancelNotDeliveredOrder_ThrowsConflict()
    {
        var order = new Order { Id = 1, Status = OrderStatus.NotDelivered };
        _orderRepositoryMock.Setup(r => r.GetOrderById(1)).Returns(order);
        _service.ChangeStatus(1, OrderStatus.Cancelled, UserRole.Administrative);
    }

    [TestMethod]
    [ExpectedException(typeof(ConflictException))]
    public void ChangeStatus_DeliverCancelledOrder_ThrowsConflict()
    {
        var order = new Order { Id = 1, Status = OrderStatus.Cancelled };
        _orderRepositoryMock.Setup(r => r.GetOrderById(1)).Returns(order);
        _service.ChangeStatus(1, OrderStatus.Delivered, UserRole.Dispatcher);
    }

    [TestMethod]
    [ExpectedException(typeof(ConflictException))]
    public void ChangeStatus_OnTheWayFromPending_ThrowsConflict()
    {
        var order = new Order { Id = 1, Status = OrderStatus.Pending };
        _orderRepositoryMock.Setup(r => r.GetOrderById(1)).Returns(order);
        _service.ChangeStatus(1, OrderStatus.OnTheWay, UserRole.Dispatcher);
    }

    [TestMethod]
    [ExpectedException(typeof(ConflictException))]
    public void ChangeStatus_NotDeliveredFromPending_ThrowsConflict()
    {
        var order = new Order { Id = 1, Status = OrderStatus.Pending };
        _orderRepositoryMock.Setup(r => r.GetOrderById(1)).Returns(order);
        _service.ChangeStatus(1, OrderStatus.NotDelivered, UserRole.Dispatcher);
    }

    [TestMethod]
    [ExpectedException(typeof(ConflictException))]
    public void ChangeStatus_DelayFromPrepared_ThrowsConflict()
    {
        var order = new Order { Id = 1, Status = OrderStatus.Prepared };
        _orderRepositoryMock.Setup(r => r.GetOrderById(1)).Returns(order);
        _service.ChangeStatus(1, OrderStatus.Delayed, UserRole.Dispatcher);
    }

    [TestMethod]
    [ExpectedException(typeof(UnauthorizedException))]
    public void ChangeStatus_DispatcherCancels_ThrowsUnauthorized()
    {
        _service.ChangeStatus(1, OrderStatus.Cancelled, UserRole.Dispatcher);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ChangeStatus_TargetNotTransitionable_ThrowsArgument()
    {
        _service.ChangeStatus(1, OrderStatus.Pending, UserRole.Dispatcher);
    }
}
