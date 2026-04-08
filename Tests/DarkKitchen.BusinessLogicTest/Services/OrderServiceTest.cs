using DarkKitchen.BusinessLogic.Args.In;
using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using Moq;

namespace DarkKitchen.BusinessLogicTest.Services;

[TestClass]
public class OrderServiceTest
{
    private Mock<IOrderRepository> _orderRepositoryMock = null!;
    private Mock<IProductRepository> _productRepositoryMock = null!;

    private Mock<IPromotionRepository> _promotionRepositoryMock = null!;
    private OrderService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _promotionRepositoryMock = new Mock<IPromotionRepository>();
        _service = new OrderService(
            _orderRepositoryMock.Object,
            _productRepositoryMock.Object);
        /*_promotionRepositoryMock.Object*/
    }

    [TestMethod]
    public void CreateOrder_ValidRequest_ReturnsCorrectTotals()
    {
        var clientId = Guid.NewGuid();
        var product = new Product { Id = 1, Price = 100.0, CommercialLine = "Pizzas" };

        var request = new CreateOrderRequestDTO
        {
            ClientId = clientId,
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

        Assert.AreEqual(clientId, result.ClientId);
        Assert.AreEqual(1, result.OrderId);
        Assert.AreEqual(200.0, result.Subtotal);
        Assert.AreEqual(50.0, result.ShippingCost);
        Assert.AreEqual(294.0, result.Total);
    }

    [TestMethod]
    public void CreateOrder_StandardDelivery_ReturnsCorrectShipping()
    {
        var clientId = Guid.NewGuid();
        var product = new Product { Id = 1, Price = 100.0, CommercialLine = "Pizzas" };

        var request = new CreateOrderRequestDTO
        {
            ClientId = clientId,
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

        Assert.AreEqual(clientId, result.ClientId);
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
            ClientId = Guid.NewGuid(),
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
}
