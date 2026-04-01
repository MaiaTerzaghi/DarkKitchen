using DarkKitchen.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? name, [FromQuery] string? category, [FromQuery] string? line)
    {
        var products = _productService.GetAll(name, category, line);
        return Ok(products);
    }
}
