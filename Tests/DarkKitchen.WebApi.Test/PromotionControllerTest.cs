using DarkKitchen.Domain.Entities;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test;

[TestClass]
public sealed class PromotionControllerTest
{
    private Mock<IPromotionService> _promotionServiceMock = null!;
    private PromotionController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _promotionServiceMock = new Mock<IPromotionService>();
        _controller = new PromotionController(_promotionServiceMock.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
        _controller.HttpContext.Items["RequestingUser"] = new User { Id = 1, Email = "admin@email.com" };
    }

    [TestMethod]
    public void GetActivePromotions_WhenCalled_ReturnsOk()
    {
        _promotionServiceMock
            .Setup(s => s.GetActivePromotions(null, null, null))
            .Returns([]);

        var result = _controller.GetActivePromotions(new PromotionFilterDTO());

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetActivePromotions_WhenServiceThrowsException_ReturnsBadRequest()
    {
        _promotionServiceMock
            .Setup(s => s.GetActivePromotions(It.IsAny<DateTime?>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Throws(new ArgumentException("Error"));

        _controller.GetActivePromotions(new PromotionFilterDTO());
    }

    [TestMethod]
    public void CreatePromotion_ValidRequest_ReturnsOkWithPromotion()
    {
        var request = new CreatePromotionRequestDTO
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30)
        };

        var expectedResponse = new PromotionResponseDTO
        {
            Id = 1,
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30)
        };

        _promotionServiceMock
            .Setup(s => s.CreatePromotion(request, It.IsAny<string>()))
            .Returns(expectedResponse);

        var result = _controller.CreatePromotion(request);

        Assert.IsInstanceOfType(result, typeof(CreatedResult));
        var okResult = (CreatedResult)result;
        var response = (PromotionResponseDTO)okResult.Value!;
        Assert.AreEqual(expectedResponse.Id, response.Id);
        Assert.AreEqual(expectedResponse.Name, response.Name);
    }

    [TestMethod]
    public void UpdatePromotion_ValidRequest_ReturnsOkWithPromotion()
    {
        var promotionId = 1;

        var request = new UpdatePromotionRequestDTO
        {
            Name = "Black Friday Updated",
            DiscountPercentage = 20,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30)
        };

        var expectedResponse = new PromotionResponseDTO
        {
            Id = promotionId,
            Name = "Black Friday Updated",
            DiscountPercentage = 20,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30)
        };

        _promotionServiceMock
            .Setup(s => s.UpdatePromotion(promotionId, request, It.IsAny<string>()))
            .Returns(expectedResponse);

        var result = _controller.UpdatePromotion(promotionId, request);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        var response = (PromotionResponseDTO)okResult.Value!;
        Assert.AreEqual(expectedResponse.Id, response.Id);
        Assert.AreEqual(expectedResponse.Name, response.Name);
    }

    [TestMethod]
    public void AddProductToPromotion_ValidRequest_ReturnsOk()
    {
        var promotionId = 1;
        var productId = 1;

        _promotionServiceMock
            .Setup(s => s.AddProductToPromotion(promotionId, productId));

        var result = _controller.AddProductToPromotion(promotionId, productId);

        Assert.IsInstanceOfType(result, typeof(OkResult));
    }

    [TestMethod]
    public void RemoveProductFromPromotion_ValidRequest_ReturnsOk()
    {
        var promotionId = 1;
        var productId = 1;

        _promotionServiceMock
            .Setup(s => s.RemoveProductFromPromotion(promotionId, productId));

        var result = _controller.RemoveProductFromPromotion(promotionId, productId);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }

    [TestMethod]
    public void CreatePromotion_PassesResponsibleUserFromContextToService()
    {
        _promotionServiceMock
            .Setup(s => s.CreatePromotion(It.IsAny<CreatePromotionRequestDTO>(), It.IsAny<string>()))
            .Returns(new PromotionResponseDTO());

        var request = new CreatePromotionRequestDTO();
        _controller.CreatePromotion(request);

        _promotionServiceMock.Verify(s => s.CreatePromotion(request, "admin@email.com"), Times.Once);
    }

    [TestMethod]
    public void UpdatePromotion_PassesResponsibleUserFromContextToService()
    {
        _promotionServiceMock
            .Setup(s => s.UpdatePromotion(It.IsAny<int>(), It.IsAny<UpdatePromotionRequestDTO>(), It.IsAny<string>()))
            .Returns(new PromotionResponseDTO());

        var request = new UpdatePromotionRequestDTO();
        _controller.UpdatePromotion(5, request);

        _promotionServiceMock.Verify(s => s.UpdatePromotion(5, request, "admin@email.com"), Times.Once);
    }
}
