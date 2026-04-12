using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogicTest.Services;

[TestClass]
public class OrderServiceTest
{
    private Mock<IOrderRepository> _orderRepositoryMock = null!;
    private Mock<IProductRepository> _productRepositoryMock = null!;

    private Mock<IPromotionRepository> _promotionRepositoryMock = null!;
    private Mock<IUserRepository> _userRepositoryMock = null!;

    private OrderService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _promotionRepositoryMock = new Mock<IPromotionRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();

        _promotionRepositoryMock
        .Setup(r => r.GetActivePromotions(It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<string?>()))
        .Returns([]);

        _userRepositoryMock
        .Setup(r => r.GetById(1))
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
            .Setup(r => r.GetById(1))
            .Returns(product);

        _orderRepositoryMock
            .Setup(r => r.Save(It.IsAny<Order>()))
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
            .Setup(r => r.GetById(1))
            .Returns(product);

        _orderRepositoryMock
            .Setup(r => r.Save(It.IsAny<Order>()))
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
        Setup(r => r.GetById(1)).
        Returns(product);

        _promotionRepositoryMock
            .Setup(r => r.GetActivePromotions(It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Returns([promotion]);

        _orderRepositoryMock.
        Setup(r => r.Save(It.IsAny<Order>())).
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
    [ExpectedException(typeof(ArgumentException))]
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
            .Setup(r => r.GetById(99))
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
            Status = "Pending",
            Items = [new OrderItem { ProductId = 1, Quantity = 2, Product = new Product { Id = 1, Price = 100.0 } }]
        };

        _orderRepositoryMock
            .Setup(r => r.GetClientOrders(It.IsAny<GetClientOrdersRequestDTO>()))
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
                Status = "Pending",
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
            .Setup(r => r.GetOrders(request))
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
    [ExpectedException(typeof(ArgumentException))]
    public void CreateOrder_ClientNotFound_ThrowsException()
    {
        _userRepositoryMock
            .Setup(r => r.GetById(99))
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
            .Setup(r => r.GetById(1))
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
            Status = "Pending"
        };

        _orderRepositoryMock
            .Setup(r => r.GetById(1))
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
    [ExpectedException(typeof(ArgumentException))]
    public void MarkAsPrepared_OrderNotFound_ThrowsException()
    {
        _orderRepositoryMock
            .Setup(r => r.GetById(99))
            .Returns((Order)null!);

        _service.MarkAsPrepared(99);
    }
}
