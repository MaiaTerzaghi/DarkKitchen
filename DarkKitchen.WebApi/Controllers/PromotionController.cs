using DarkKitchen.BusinessLogic.Args.In;
using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain.Enums;
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
}
