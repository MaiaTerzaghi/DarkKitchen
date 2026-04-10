using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogicTest.Services;

[TestClass]
public sealed class PromotionServiceTest
{
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

        var promotionRepositoryMock = new Mock<IPromotionRepository>();
        promotionRepositoryMock.Setup(r => r.GetActivePromotions(null, null, null))
                               .Returns(promotions);

        var promotionService = new PromotionService(promotionRepositoryMock.Object);

        var result = promotionService.GetActivePromotions(null, null, null);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
    }
}
