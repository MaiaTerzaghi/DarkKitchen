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
public class PricingServiceTest
{
    private Mock<IRepository<Product>> _productRepositoryMock = null!;
    private Mock<IPromotionRepository> _promotionRepositoryMock = null!;
    private PricingService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _productRepositoryMock = new Mock<IRepository<Product>>();
        _promotionRepositoryMock = new Mock<IPromotionRepository>();

        _promotionRepositoryMock
            .Setup(r => r.GetActivePromotions(It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Returns([]);

        _service = new PricingService(
            _productRepositoryMock.Object,
            _promotionRepositoryMock.Object);
    }

    [TestMethod]
    public void CalculateOrderPricing_ValidRequest_ReturnsCorrectTotals()
    {
        var product = new Product { Id = 1, Price = 100.0, CommercialLine = "Pizzas" };

        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(product);

        var items = new List<OrderItemRequestDTO>
        {
            new OrderItemRequestDTO { ProductId = 1, Quantity = 2 }
        };

        var result = _service.CalculateOrderPricing(items, DeliveryType.Express);

        Assert.AreEqual(200.0, result.Subtotal);
        Assert.AreEqual(50.0, result.ShippingCost);
        Assert.AreEqual(294.0, result.Total);
    }

    [TestMethod]
    public void CalculateOrderPricing_StandardDelivery_ReturnsCorrectShipping()
    {
        var product = new Product { Id = 1, Price = 100.0, CommercialLine = "Pizzas" };

        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(product);

        var items = new List<OrderItemRequestDTO>
        {
            new OrderItemRequestDTO { ProductId = 1, Quantity = 2 }
        };

        var result = _service.CalculateOrderPricing(items, DeliveryType.Standard);

        Assert.AreEqual(200.0, result.Subtotal);
        Assert.AreEqual(20.0, result.ShippingCost);
        Assert.AreEqual(264.0, result.Total);
    }

    [TestMethod]
    public void CalculateOrderPricing_WithPromotion_AppliesDiscountCorrectly()
    {
        var product = new Product { Id = 1, Price = 100.0, CommercialLine = "Pizzas" };

        var promotion = new Promotion
        {
            Id = 1,
            DiscountPercentage = 10,
            Products = [product]
        };

        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(product);

        _promotionRepositoryMock
            .Setup(r => r.GetActivePromotions(It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Returns([promotion]);

        var items = new List<OrderItemRequestDTO>
        {
            new OrderItemRequestDTO { ProductId = 1, Quantity = 2 }
        };

        var result = _service.CalculateOrderPricing(items, DeliveryType.Standard);

        var expectedSubtotal = 200.0;
        var expectedDiscountedSubtotal = expectedSubtotal * 0.9;
        var expectedTotal = Math.Round((expectedDiscountedSubtotal * 1.22) + 20.0, 2);

        Assert.AreEqual(expectedTotal, result.Total);
    }

    [TestMethod]
    public void CalculateOrderPricing_WithPromotion_AppliesDiscountOnlyToPromotedProduct()
    {
        var pizza = new Product { Id = 1, Price = 100.0, CommercialLine = "Pizzas" };
        var burger = new Product { Id = 2, Price = 200.0, CommercialLine = "Burgers" };

        var promotion = new Promotion
        {
            Id = 1,
            DiscountPercentage = 10,
            Products = [pizza]
        };

        _productRepositoryMock
            .SetupSequence(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(pizza)
            .Returns(burger);

        _promotionRepositoryMock
            .Setup(r => r.GetActivePromotions(It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Returns([promotion]);

        var items = new List<OrderItemRequestDTO>
        {
            new OrderItemRequestDTO { ProductId = 1, Quantity = 1 },
            new OrderItemRequestDTO { ProductId = 2, Quantity = 1 }
        };

        var result = _service.CalculateOrderPricing(items, DeliveryType.Standard);

        var pizzaSubtotal = 100.0;
        var burgerSubtotal = 200.0;
        var discountOnPizza = pizzaSubtotal * 0.10;
        var discountedTotal = pizzaSubtotal + burgerSubtotal - discountOnPizza;
        var expectedTotal = Math.Round((discountedTotal * 1.22) + 20.0, 2);

        Assert.AreEqual(expectedTotal, result.Total);
    }

    [TestMethod]
    [ExpectedException(typeof(NotFoundException))]
    public void CalculateOrderPricing_ProductNotFound_ThrowsException()
    {
        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns((Product)null!);

        var items = new List<OrderItemRequestDTO>
        {
            new OrderItemRequestDTO { ProductId = 99, Quantity = 2 }
        };

        _service.CalculateOrderPricing(items, DeliveryType.Express);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CalculateOrderPricing_InactiveProduct_ThrowsException()
    {
        var product = new Product { Id = 1, Price = 100.0, IsActive = false, CommercialLine = "Pizzas", Name = "Pizza Napolitana", Description = "Rica pizza napolitana con tomate y albahaca", Images = "pizza.jpg", Category = "Fritos", Code = "P0001" };

        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(product);

        var items = new List<OrderItemRequestDTO>
        {
            new OrderItemRequestDTO { ProductId = 1, Quantity = 2 }
        };

        _service.CalculateOrderPricing(items, DeliveryType.Express);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CalculateOrderPricing_InvalidDeliveryType_ThrowsException()
    {
        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(new Product { Id = 1, Price = 100.0 });

        var items = new List<OrderItemRequestDTO>
        {
            new OrderItemRequestDTO { ProductId = 1, Quantity = 1 }
        };

        _service.CalculateOrderPricing(items, (DeliveryType)99);
    }
}
