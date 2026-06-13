using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
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

    private const string Pizzas = "Pizzas";
    private const string Standard = "Standard";
    private const string Express = "Express";
    private const string PizzaMargherita = "Pizza Margherita";
    private const string Burgers = "Burgers";
    private const string EnsaladaCaesarPremium = "Ensalada Caesar Premium";
    private const string Ensaladas = "Ensaladas";
    private const string Fritos = "Fritos";
    private const string P0001 = "P0001";
    private const string PizzaNapolitana = "Pizza Napolitana";
    private const string RicaPizzaNapolitanaConTomateYA = "Rica pizza napolitana con tomate y albahaca";
    private PricingService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _productRepositoryMock = new Mock<IRepository<Product>>();
        _promotionRepositoryMock = new Mock<IPromotionRepository>();

        _promotionRepositoryMock
            .Setup(r => r.GetActivePromotions(It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns((new List<Promotion>(), 0));

        _service = new PricingService(
            _productRepositoryMock.Object,
            _promotionRepositoryMock.Object);
    }

    [TestMethod]
    public void CalculateOrderPricing_ValidRequest_ReturnsCorrectTotals()
    {
        var product = new Product { Id = 1, Price = 100.0, CommercialLine = Pizzas };

        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(product);

        var items = new List<OrderItemRequestDTO>
        {
            new OrderItemRequestDTO { ProductId = 1, Quantity = 2 }
        };

        var expressShipping = new ShippingType { Id = 1, Name = Express, Cost = 50.0 };
        var result = _service.CalculateOrderPricing(items, expressShipping);

        Assert.AreEqual(200.0, result.Subtotal);
        Assert.AreEqual(50.0, result.ShippingCost);
        Assert.AreEqual(294.0, result.Total);
    }

    [TestMethod]
    public void CalculateOrderPricing_StandardDelivery_ReturnsCorrectShipping()
    {
        var product = new Product { Id = 1, Price = 100.0, CommercialLine = Pizzas };

        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(product);

        var items = new List<OrderItemRequestDTO>
        {
            new OrderItemRequestDTO { ProductId = 1, Quantity = 2 }
        };

        var standardShipping = new ShippingType { Id = 2, Name = Standard, Cost = 20.0 };
        var result = _service.CalculateOrderPricing(items, standardShipping);

        Assert.AreEqual(200.0, result.Subtotal);
        Assert.AreEqual(20.0, result.ShippingCost);
        Assert.AreEqual(264.0, result.Total);
    }

    [TestMethod]
    public void CalculateOrderPricing_WithPromotion_AppliesDiscountCorrectly()
    {
        var product = new Product { Id = 1, Price = 100.0, CommercialLine = Pizzas };

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
            .Setup(r => r.GetActivePromotions(It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns((new List<Promotion> { promotion }, 1));

        var items = new List<OrderItemRequestDTO>
        {
            new OrderItemRequestDTO { ProductId = 1, Quantity = 2 }
        };

        var standardShipping = new ShippingType { Id = 2, Name = Standard, Cost = 20.0 };
        var result = _service.CalculateOrderPricing(items, standardShipping);

        var expectedSubtotal = 200.0;
        var expectedDiscountedSubtotal = expectedSubtotal * 0.9;
        var expectedTotal = Math.Round((expectedDiscountedSubtotal * 1.22) + 20.0, 2);

        Assert.AreEqual(expectedTotal, result.Total);
    }

    [TestMethod]
    public void CalculateOrderPricing_WithPromotion_AppliesDiscountOnlyToPromotedProduct()
    {
        var pizza = new Product { Id = 1, Price = 100.0, CommercialLine = Pizzas };
        var burger = new Product { Id = 2, Price = 200.0, CommercialLine = Burgers };

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
            .Setup(r => r.GetActivePromotions(It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns((new List<Promotion> { promotion }, 1));

        var items = new List<OrderItemRequestDTO>
        {
            new OrderItemRequestDTO { ProductId = 1, Quantity = 1 },
            new OrderItemRequestDTO { ProductId = 2, Quantity = 1 }
        };

        var standardShipping = new ShippingType { Id = 2, Name = Standard, Cost = 20.0 };
        var result = _service.CalculateOrderPricing(items, standardShipping);

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

        var expressShipping = new ShippingType { Id = 1, Name = Express, Cost = 50.0 };
        _service.CalculateOrderPricing(items, expressShipping);
    }

    [TestMethod]
    public void PreviewOrderPricing_NoPromotion_ReturnsItemsWithoutDiscount()
    {
        var product = new Product { Id = 1, Name = EnsaladaCaesarPremium, Price = 250.0, IsActive = true, CommercialLine = Ensaladas };

        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(product);

        var items = new List<OrderItemRequestDTO>
        {
            new OrderItemRequestDTO { ProductId = 1, Quantity = 2 }
        };

        var shipping = new ShippingType { Id = 1, Name = Express, Cost = 50.0 };
        var result = _service.PreviewOrderPricing(items, shipping);

        Assert.AreEqual(1, result.Items.Count);
        Assert.AreEqual(250.0, result.Items[0].UnitPrice);
        Assert.AreEqual(0, result.Items[0].DiscountPercentage);
        Assert.AreEqual(250.0, result.Items[0].DiscountedUnitPrice);
        Assert.AreEqual(500.0, result.Items[0].ItemTotal);
        Assert.AreEqual(500.0, result.Subtotal);
        Assert.AreEqual(0, result.Discount);
        Assert.AreEqual(50.0, result.ShippingCost);
    }

    [TestMethod]
    public void PreviewOrderPricing_WithPromotion_ReturnsItemsWithDiscount()
    {
        var pizza = new Product { Id = 1, Name = PizzaMargherita, Price = 350.0, IsActive = true, CommercialLine = Pizzas };

        var promo = new Promotion
        {
            Id = 1,
            DiscountPercentage = 35,
            Products = [pizza]
        };

        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(pizza);

        _promotionRepositoryMock
            .Setup(r => r.GetActivePromotions(It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns((new List<Promotion> { promo }, 1));

        var items = new List<OrderItemRequestDTO>
        {
            new OrderItemRequestDTO { ProductId = 1, Quantity = 1 }
        };

        var shipping = new ShippingType { Id = 1, Name = Standard, Cost = 20.0 };
        var result = _service.PreviewOrderPricing(items, shipping);

        Assert.AreEqual(1, result.Items.Count);
        Assert.AreEqual(350.0, result.Items[0].UnitPrice);
        Assert.AreEqual(35, result.Items[0].DiscountPercentage);
        Assert.AreEqual(227.5, result.Items[0].DiscountedUnitPrice);
        Assert.AreEqual(227.5, result.Items[0].ItemTotal);
        Assert.AreEqual(350.0, result.Subtotal);
        Assert.AreEqual(122.5, result.Discount);
    }

    [TestMethod]
    public void PreviewOrderPricing_MultiplePromotions_AppliesBestDiscount()
    {
        var pizza = new Product { Id = 1, Name = PizzaMargherita, Price = 350.0, IsActive = true, CommercialLine = Pizzas };

        var promo15 = new Promotion { Id = 1, DiscountPercentage = 15, Products = [pizza] };
        var promo35 = new Promotion { Id = 2, DiscountPercentage = 35, Products = [pizza] };

        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(pizza);

        _promotionRepositoryMock
            .Setup(r => r.GetActivePromotions(It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns((new List<Promotion> { promo15, promo35 }, 2));

        var items = new List<OrderItemRequestDTO>
        {
            new OrderItemRequestDTO { ProductId = 1, Quantity = 1 }
        };

        var shipping = new ShippingType { Id = 1, Name = Standard, Cost = 20.0 };
        var result = _service.PreviewOrderPricing(items, shipping);

        Assert.AreEqual(35, result.Items[0].DiscountPercentage);
        Assert.AreEqual(227.5, result.Items[0].DiscountedUnitPrice);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CalculateOrderPricing_InactiveProduct_ThrowsException()
    {
        var product = new Product { Id = 1, Price = 100.0, IsActive = false, CommercialLine = Pizzas, Name = PizzaNapolitana, Description = RicaPizzaNapolitanaConTomateYA, Images = "/9j/2Q==", Category = Fritos, Code = P0001 };

        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(product);

        var items = new List<OrderItemRequestDTO>
        {
            new OrderItemRequestDTO { ProductId = 1, Quantity = 2 }
        };

        var expressShipping = new ShippingType { Id = 1, Name = Express, Cost = 50.0 };
        _service.CalculateOrderPricing(items, expressShipping);
    }
}
