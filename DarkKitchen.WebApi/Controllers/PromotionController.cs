using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Filters.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/promotions")]
public class PromotionController(IPromotionService promotionService) : DarkKitchenControllerBase
{
    private readonly IPromotionService _promotionService = promotionService;

    [ClientOrAdministrative]
    [HttpGet]
    public IActionResult GetActivePromotions([FromQuery] PromotionFilterDTO filters, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var requestingUser = GetRequestingUser();
        var date = requestingUser.Role == UserRole.Client
            ? DateTime.Today
            : filters.Date;
        var promotions = _promotionService.GetActivePromotions(date, filters.ProductLine, filters.Product, page, pageSize);
        return Ok(promotions);
    }

    [AdministrativeOnly]
    [HttpPost]
    public IActionResult CreatePromotion([FromBody] CreatePromotionRequestDTO request)
    {
        var requestingUser = GetRequestingUser();
        var response = _promotionService.CreatePromotion(request, requestingUser.Email);
        return Created(string.Empty, response);
    }

    [AdministrativeOnly]
    [HttpPut("{id}")]
    public IActionResult UpdatePromotion(int id, [FromBody] UpdatePromotionRequestDTO request)
    {
        var requestingUser = GetRequestingUser();
        var response = _promotionService.UpdatePromotion(id, request, requestingUser.Email);
        return Ok(response);
    }

    [AdministrativeOnly]
    [HttpPost("{id}/products")]
    public IActionResult AddProductToPromotion(int id, [FromQuery] int productId)
    {
        _promotionService.AddProductToPromotion(id, productId);
        return Ok();
    }

    [AdministrativeOnly]
    [HttpDelete("{id}/products")]
    public IActionResult RemoveProductFromPromotion(int id, [FromQuery] int productId)
    {
        _promotionService.RemoveProductFromPromotion(id, productId);
        return NoContent();
    }
}
