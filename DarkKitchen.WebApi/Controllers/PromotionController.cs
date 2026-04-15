using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/promotions")]
public class PromotionController(IPromotionService promotionService) : ControllerBase
{
    private readonly IPromotionService _promotionService = promotionService;

    [AuthorizeRoles(UserRole.Client, UserRole.Administrative)]
    [HttpGet]
    public IActionResult GetActivePromotions([FromQuery] PromotionFilterDTO filters)
    {
        var promotions = _promotionService.GetActivePromotions(filters.Date, filters.ProductLine, filters.Product);
        return Ok(promotions);
    }

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpPost]
    public IActionResult CreatePromotion([FromBody] CreatePromotionRequestDTO request)
    {
        var response = _promotionService.CreatePromotion(request);
        return Ok(response);
    }

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpPut("{id}")]
    public IActionResult UpdatePromotion(int id, [FromBody] UpdatePromotionRequestDTO request)
    {
        throw new NotImplementedException();
    }
}
