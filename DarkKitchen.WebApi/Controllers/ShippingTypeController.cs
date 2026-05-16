using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/shipping-types")]
public class ShippingTypeController(IShippingTypeService shippingTypeService) : ControllerBase
{
    private readonly IShippingTypeService _shippingTypeService = shippingTypeService;

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpGet]
    public IActionResult GetAll()
    {
        var shippingTypes = _shippingTypeService.GetAll();
        return Ok(shippingTypes);
    }
}
