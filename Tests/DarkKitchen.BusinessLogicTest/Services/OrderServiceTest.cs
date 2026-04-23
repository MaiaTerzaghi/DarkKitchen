using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogicTest.Services;

[TestClass]
public class OrderServiceTest
{
    private Mock<IOrderRepository> _orderRepositoryMock = null!;
    private Mock<IRepository<Product>> _productRepositoryMock = null!;

    private Mock<IPromotionRepository> _promotionRepositoryMock = null!;
    private Mock<IRepository<User>> _userRepositoryMock = null!;
    private OrderService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IRepository<Product>>();
        _promotionRepositoryMock = new Mock<IPromotionRepository>();
        _userRepositoryMock = new Mock<IRepository<User>>();

        _promotionRepositoryMock
        .Setup(r => r.GetActivePromotions(It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<string?>()))
        .Returns([]);

        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                            .Returns(new User { Id = 1, Role = UserRole.Client });

        _service = new OrderService(
            _orderRepositoryMock.Object,
            _productRepositoryMock.Object,
            _promotionRepositoryMock.Object,
            _userRepositoryMock.Object);
    }

    [TestMethod]
    public void CreateOrder_ValidRequest_ReturnsCorrectTotals()
    {
        var product = new Product { Id = 1, Price = 100.0, CommercialLine = "Pizzas" };

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
            Items = [new OrderItemRequestDTO { ProductId = 1, Quantity = 2 }]
        };

        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(product);

        _orderRepositoryMock
             .Setup(r => r.Add(It.IsAny<Order>()))
             .Returns((Order o) =>
            {
                o.Id = 1;
                return o;
            });

        var result = _service.CreateOrder(request);

        Assert.AreEqual(1, result.ClientId);
        Assert.AreEqual(1, result.OrderId);
        Assert.AreEqual(200.0, result.Subtotal);
        Assert.AreEqual(50.0, result.ShippingCost);
        Assert.AreEqual(294.0, result.Total);
    }

    [TestMethod]
    public void CreateOrder_StandardDelivery_ReturnsCorrectShipping()
    {
        var product = new Product { Id = 1, Price = 100.0, CommercialLine = "Pizzas" };

        var request = new CreateOrderRequestDTO
        {
            ClientId = 1,
            DeliveryType = "Standard",
            Address = new AddressDTO
            {
                Street = "18 de Julio",
                DoorNumber = "1234",
                Apartment = "2B"
            },
            Items = [new OrderItemRequestDTO { ProductId = 1, Quantity = 2 }]
        };

        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(product);

        _orderRepositoryMock
            .Setup(r => r.Add(It.IsAny<Order>()))
            .Returns((Order o) =>
            {
                o.Id = 1;
                return o;
            });

        var result = _service.CreateOrder(request);

        Assert.AreEqual(1, result.ClientId);
        Assert.AreEqual(1, result.OrderId);
        Assert.AreEqual(200.0, result.Subtotal);
        Assert.AreEqual(20.0, result.ShippingCost);
        Assert.AreEqual(264.0, result.Total);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateOrder_EmptyItems_ThrowsException()
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

        _service.CreateOrder(request);
    }

    [TestMethod]
    public void CreateOrder_WithPromotion_AppliesDiscountCorrectly()
    {
        var product = new Product { Id = 1, Price = 100.0, CommercialLine = "Pizzas" };

        var promotion = new Promotion
        {
            Id = 1,
            DiscountPercentage = 10, // 10% de descuento
            Products = [product]
        };

        var request = new CreateOrderRequestDTO
        {
            ClientId = 1,
            DeliveryType = "Standard",
            Address = new AddressDTO { Street = "18 de Julio", DoorNumber = "1234", Apartment = "2B" },
            Items = [new OrderItemRequestDTO { ProductId = 1, Quantity = 2 }]
        };

        _productRepositoryMock.
        Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>())).
        Returns(product);

        _promotionRepositoryMock
            .Setup(r => r.GetActivePromotions(It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Returns([promotion]);

        _orderRepositoryMock.
        Setup(r => r.Add(It.IsAny<Order>())).
        Returns((Order o) =>
        {
            o.Id = 1;
            return o;
        });

        var result = _service.CreateOrder(request);

        var expectedSubtotal = 200.0;
        var expectedDiscountedSubtotal = expectedSubtotal * 0.9;
        var expectedShipping = 20.0;
        var expectedTotal = Math.Round((expectedDiscountedSubtotal * 1.22) + expectedShipping, 2);

        Assert.AreEqual(expectedTotal, result.Total);
    }

    [TestMethod]
    [ExpectedException(typeof(NotFoundException))]
    public void CreateOrder_ProductNotFound_ThrowsException()
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
            Items = [new OrderItemRequestDTO { ProductId = 99, Quantity = 2 }]
        };

        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns((Product)null!);

        _service.CreateOrder(request);
    }

    [TestMethod]
    public void GetClientOrders_WhenCalled_ReturnsOrders()
    {
        var order = new Order
        {
            Id = 1,
            ClientId = 1,
            Status = OrderStatus.Pending,
            Items = [new OrderItem { ProductId = 1, Quantity = 2, Product = new Product { Id = 1, Price = 100.0 } }]
        };

        _orderRepositoryMock
            .Setup(r => r.GetClientOrders(It.IsAny<int>(), It.IsAny<OrderStatus?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .Returns([order]);

        var request = new GetClientOrdersRequestDTO { ClientId = 1 };

        var result = _service.GetClientOrders(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
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
                Status = OrderStatus.Pending,
                Date = new DateTime(2026, 1, 10),
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
            .Setup(r => r.GetOrders(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string?>(), It.IsAny<OrderStatus?>()))
            .Returns(ordersFromRepo);

        var result = _service.GetOrders(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(1, result[0].OrderId);
        Assert.AreEqual("Pending", result[0].Status);
        Assert.AreEqual("Hamburguesa", result[0].Items[0].ProductName);
        Assert.AreEqual(2, result[0].Items[0].Quantity);
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
            DeliveryType = "Express",
            Address = new AddressDTO { Street = "18 de Julio", DoorNumber = "1234" },
            Items = [new OrderItemRequestDTO { ProductId = 1, Quantity = 1 }]
        };

        _service.CreateOrder(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateOrder_InvalidDeliveryType_ThrowsException()
    {
        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(new Product { Id = 1, Price = 100.0 });

        var request = new CreateOrderRequestDTO
        {
            ClientId = 1,
            DeliveryType = "Drone",
            Address = new AddressDTO { Street = "18 de Julio", DoorNumber = "1234" },
            Items = [new OrderItemRequestDTO { ProductId = 1, Quantity = 1 }]
        };

        _service.CreateOrder(request);
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
    [ExpectedException(typeof(ArgumentException))]
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
            DeliveryType = DeliveryType.Express,
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
            DeliveryType = DeliveryType.Express,
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
    [ExpectedException(typeof(ArgumentException))]
    public void DeliverOrder_WhenOrderIsNotOnTheWay_ThrowsException()
    {
        var order = new Order
        {
            Id = 1,
            ClientId = 1,
            Status = OrderStatus.Pending,
            DeliveryType = DeliveryType.Express,
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
    [ExpectedException(typeof(ArgumentException))]
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
    [ExpectedException(typeof(ArgumentException))]
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
    [ExpectedException(typeof(ArgumentException))]
    public void CreateOrder_InactiveProduct_ThrowsException()
    {
        var product = new Product { Id = 1, Price = 100.0, IsActive = false, CommercialLine = "Pizzas", Name = "Pizza Napolitana", Description = "Rica pizza napolitana con tomate y albahaca", Images = "pizza.jpg", Category = "Fritos", Code = "P0001" };

        var request = new CreateOrderRequestDTO
        {
            ClientId = 1,
            DeliveryType = "Express",
            Address = new AddressDTO
            {
                Street = "18 de Julio",
                DoorNumber = "1234"
            },
            Items = [new OrderItemRequestDTO { ProductId = 1, Quantity = 2 }]
        };

        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(product);

        _orderRepositoryMock
            .Setup(r => r.Add(It.IsAny<Order>()))
            .Returns((Order o) =>
            {
                o.Id = 1;
                return o;
            });

        _service.CreateOrder(request);
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
    [ExpectedException(typeof(ArgumentException))]
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
            Images = "pizza.jpg"
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
        var expectedReport = new List<(int Year, int Month, int ClientId, double Total)>
        {
            (2026, 1, 1, 500.0),
            (2026, 1, 2, 300.0)
        };

        _orderRepositoryMock
            .Setup(r => r.GetSalesReport(1, 20))
            .Returns(expectedReport);

        var result = _service.GetSalesReport(1, 20);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(2026, result[0].Year);
        Assert.AreEqual(1, result[0].Month);
        Assert.AreEqual(2, result[0].Clients.Count);
        Assert.AreEqual(500.0, result[0].Clients[0].Total);
        Assert.AreEqual(300.0, result[0].Clients[1].Total);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateOrder_WhenStreetIsEmpty_ThrowsException()
    {
        var request = new CreateOrderRequestDTO
        {
            ClientId = 1,
            DeliveryType = "Express",
            Address = new AddressDTO
            {
                Street = string.Empty,
                DoorNumber = "1234",
                Apartment = "2B"
            },
            Items = [new OrderItemRequestDTO { ProductId = 1, Quantity = 1 }]
        };

        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(new Product { Id = 1, Price = 100.0, CommercialLine = "Pizzas", Name = "Pizza Napolitana", Code = "P0001", Description = "Rica pizza napolitana con tomate y albahaca", Category = "Fritos", Images = "pizza.jpg" });

        _service.CreateOrder(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateOrder_WhenDoorNumberIsEmpty_ThrowsException()
    {
        var request = new CreateOrderRequestDTO
        {
            ClientId = 1,
            DeliveryType = "Express",
            Address = new AddressDTO
            {
                Street = "18 de Julio",
                DoorNumber = string.Empty,
                Apartment = "2B"
            },
            Items = [new OrderItemRequestDTO { ProductId = 1, Quantity = 1 }]
        };

        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(new Product { Id = 1, Price = 100.0, CommercialLine = "Pizzas", Name = "Pizza Napolitana", Code = "P0001", Description = "Rica pizza napolitana con tomate y albahaca", Category = "Fritos", Images = "pizza.jpg" });

        _service.CreateOrder(request);
    }
}
