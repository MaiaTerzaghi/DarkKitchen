using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogicTest.Services;

[TestClass]
public sealed class PromotionServiceTest
{
    private Mock<IPromotionRepository> _promotionRepositoryMock = null!;
    private Mock<IProductRepository> _productRepositoryMock = null!;

    private PromotionService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _promotionRepositoryMock = new Mock<IPromotionRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _service = new PromotionService(_promotionRepositoryMock.Object, _productRepositoryMock.Object);
    }

    [TestMethod]
    public void GetActivePromotions_WhenCalled_ReturnsPromotions()
    {
        var promotions = new List<Promotion>
        {
            new Promotion
            {
                Id = 1,
                Name = "Black Friday",
                DiscountPercentage = 10,
                ValidFrom = new DateTime(2026, 1, 25),
                ValidTo = new DateTime(2026, 1, 30)
            }
        };

        _promotionRepositoryMock
            .Setup(r => r.GetActivePromotions(null, null, null))
            .Returns(promotions);

        var result = _service.GetActivePromotions(null, null, null);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void CreatePromotion_ValidRequest_ReturnsPromotionResponse()
    {
        var request = new CreatePromotionRequestDTO
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30)
        };

        var savedPromotion = new Promotion
        {
            Id = 1,
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30)
        };

        _promotionRepositoryMock
            .Setup(r => r.Add(It.IsAny<Promotion>()))
            .Returns(savedPromotion);

        var result = _service.CreatePromotion(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual("Black Friday", result.Name);
        Assert.AreEqual(10, result.DiscountPercentage);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreatePromotion_InvalidDateRange_ThrowsException()
    {
        var request = new CreatePromotionRequestDTO
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 30),
            ValidTo = new DateTime(2026, 1, 25)
        };

        _service.CreatePromotion(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreatePromotion_InvalidDiscountPercentage_ThrowsException()
    {
        var request = new CreatePromotionRequestDTO
        {
            Name = "Black Friday",
            DiscountPercentage = 0,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30)
        };

        _service.CreatePromotion(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreatePromotion_EmptyName_ThrowsException()
    {
        var request = new CreatePromotionRequestDTO
        {
            Name = string.Empty,
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30)
        };

        _service.CreatePromotion(request);
    }

    [TestMethod]
    public void UpdatePromotion_ValidRequest_ReturnsUpdatedPromotion()
    {
        var promotion = new Promotion
        {
            Id = 1,
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30)
        };

        var request = new UpdatePromotionRequestDTO
        {
            Name = "Black Friday Updated",
            DiscountPercentage = 20,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30)
        };

        _promotionRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns(promotion);

        _promotionRepositoryMock
            .Setup(r => r.Update(It.IsAny<Promotion>()))
            .Returns(promotion);

        var result = _service.UpdatePromotion(1, request);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual("Black Friday Updated", result.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdatePromotion_PromotionNotFound_ThrowsException()
    {
        _promotionRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns((Promotion)null!);

        var request = new UpdatePromotionRequestDTO
        {
            Name = "Black Friday Updated",
            DiscountPercentage = 20,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30)
        };

        _service.UpdatePromotion(99, request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdatePromotion_InvalidDateRange_ThrowsException()
    {
        var promotion = new Promotion
        {
            Id = 1,
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30)
        };

        _promotionRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns(promotion);

        var request = new UpdatePromotionRequestDTO
        {
            Name = "Black Friday Updated",
            DiscountPercentage = 20,
            ValidFrom = new DateTime(2026, 1, 30),
            ValidTo = new DateTime(2026, 1, 25)
        };

        _service.UpdatePromotion(1, request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdatePromotion_InvalidDiscountPercentage_ThrowsException()
    {
        var promotion = new Promotion
        {
            Id = 1,
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30)
        };

        _promotionRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns(promotion);

        var request = new UpdatePromotionRequestDTO
        {
            Name = "Black Friday Updated",
            DiscountPercentage = 0,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30)
        };

        _service.UpdatePromotion(1, request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdatePromotion_EmptyName_ThrowsException()
    {
        var promotion = new Promotion
        {
            Id = 1,
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30)
        };

        _promotionRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns(promotion);

        var request = new UpdatePromotionRequestDTO
        {
            Name = string.Empty,
            DiscountPercentage = 20,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30)
        };

        _service.UpdatePromotion(1, request);
    }

    [TestMethod]
    public void AddProductToPromotion_ValidRequest_AddsProductToPromotion()
    {
        var promotion = new Promotion
        {
            Id = 1,
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30),
            Products = []
        };

        var product = new Product { Id = 1, Name = "Pizza Napolitana", Price = 100 };

        _promotionRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns(promotion);

        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(product);

        _promotionRepositoryMock
            .Setup(r => r.Update(It.IsAny<Promotion>()))
            .Returns(promotion);

        _service.AddProductToPromotion(1, 1);

        Assert.AreEqual(1, promotion.Products.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AddProductToPromotion_PromotionNotFound_ThrowsException()
    {
        _promotionRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns((Promotion)null!);

        _service.AddProductToPromotion(99, 1);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AddProductToPromotion_ProductNotFound_ThrowsException()
    {
        var promotion = new Promotion
        {
            Id = 1,
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30),
            Products = []
        };

        _promotionRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns(promotion);

        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns((Product)null!);

        _service.AddProductToPromotion(1, 99);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AddProductToPromotion_ProductAlreadyHasPromotion_ThrowsException()
    {
        var product = new Product { Id = 1, Name = "Pizza", Price = 100 };

        var newPromotion = new Promotion
        {
            Id = 1,
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = DateTime.Today.AddDays(-1),
            ValidTo = DateTime.Today.AddDays(1),
            Products = []
        };

        _promotionRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns(newPromotion);

        _productRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(product);

        _promotionRepositoryMock
            .Setup(r => r.ProductHasActivePromotion(1))
            .Returns(true);

        _service.AddProductToPromotion(1, 1);
    }

    [TestMethod]
    public void RemoveProductFromPromotion_ValidRequest_RemovesProductFromPromotion()
    {
        var product = new Product { Id = 1, Name = "Pizza Napolitana", Price = 100 };

        var promotion = new Promotion
        {
            Id = 1,
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30),
            Products = [product]
        };

        _promotionRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns(promotion);

        _promotionRepositoryMock
            .Setup(r => r.Update(It.IsAny<Promotion>()))
            .Returns(promotion);

        _service.RemoveProductFromPromotion(1, 1);

        Assert.AreEqual(0, promotion.Products.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RemoveProductFromPromotion_PromotionNotFound_ThrowsException()
    {
        _promotionRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns((Promotion)null!);

        _service.RemoveProductFromPromotion(99, 1);
    }
}
