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
    private PromotionService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _promotionRepositoryMock = new Mock<IPromotionRepository>();
        _service = new PromotionService(_promotionRepositoryMock.Object);
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
}
