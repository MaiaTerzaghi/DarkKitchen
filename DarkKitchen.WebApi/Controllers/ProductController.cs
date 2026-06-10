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
    public IActionResult GetProducts([FromQuery] GetProductsManageRequestDTO request)
    {
        var requestingUser = (User)HttpContext.Items["RequestingUser"]!;
        var products = _productService.GetProducts(request, requestingUser.Role);
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
        var requestingUser = (User)HttpContext.Items["RequestingUser"]!;
        var product = _productService.UpdateProduct(id, request, requestingUser.Email);
        return Ok(product);
    }
}
