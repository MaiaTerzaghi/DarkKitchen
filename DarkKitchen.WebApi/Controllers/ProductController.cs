using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;

    [AuthorizeRoles(UserRole.Client, UserRole.Administrative)]
    [HttpGet]
    public IActionResult GetAll([FromQuery] string? name, [FromQuery] string? category, [FromQuery] string? line)
    {
        var products = _productService.GetAll(name, category, line);

        var response = products.Select(p => new ProductResponseDTO
        {
            Code = p.Code,
            Name = p.Name,
            Price = p.Price,
            CommercialLine = p.CommercialLine,
            Category = p.Category,
            Images = p.Images,
        });

        return Ok(response);
    }

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpPost]
    public IActionResult CreateProduct([FromBody] CreateProductRequestDTO request)
    {
        var product = _productService.CreateProduct(request);
        return CreatedAtAction(nameof(CreateProduct), new { id = product.Code }, product);
    }
}
