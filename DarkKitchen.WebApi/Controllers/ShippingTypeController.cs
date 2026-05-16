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

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var shippingType = _shippingTypeService.GetById(id);
        return Ok(shippingType);
    }

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpPost]
    public IActionResult Create([FromBody] CreateShippingTypeRequestDTO request)
    {
        var shippingType = _shippingTypeService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = shippingType.Id }, shippingType);
    }

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdateShippingTypeRequestDTO request)
    {
        throw new NotImplementedException();
    }
}
