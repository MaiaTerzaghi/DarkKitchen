using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.WebApi.Controllers;
using DarkKitchen.WebApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test;

[TestClass]
public sealed class PromotionControllerTest
{
    [TestMethod]
    public void GetActivePromotions_WhenCalled_ReturnsOk()
    {
        var promotionServiceMock = new Mock<IPromotionService>();
        promotionServiceMock.Setup(s => s.GetActivePromotions(null, null, null))
                            .Returns([]);

        var controller = new PromotionController(promotionServiceMock.Object);

        var result = controller.GetActivePromotions(new PromotionFilterDTO());

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }
}
