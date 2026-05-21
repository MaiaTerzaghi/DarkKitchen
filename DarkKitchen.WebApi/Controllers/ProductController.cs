using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
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
    public IActionResult GetAll([FromQuery] string? name, [FromQuery] string? category, [FromQuery] string? line, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var products = _productService.GetAll(name, category, line, page, pageSize);
        return Ok(products);
    }

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpPost]
    public IActionResult CreateProduct([FromBody] CreateProductRequestDTO request)
    {
        var requestingUser = (User)HttpContext.Items["RequestingUser"]!;
        var product = _productService.CreateProduct(request, requestingUser.Email);
        return Created(" ", product);
    }

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpPut("{id}")]
    public IActionResult UpdateProduct(int id, [FromBody] UpdateProductRequestDTO request)
    {
        var product = _productService.UpdateProduct(id, request);
        return Ok(product);
    }

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpGet("manage")]
    public IActionResult GetManage([FromQuery] GetProductsManageRequestDTO request)
    {
        var products = _productService.GetManage(request);
        return Ok(products);
    }
}
