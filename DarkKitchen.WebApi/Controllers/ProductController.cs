using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Filters.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController(IProductService productService) : DarkKitchenControllerBase
{
    private readonly IProductService _productService = productService;

    [ClientOrAdministrative]
    [HttpGet]
    public IActionResult GetProducts([FromQuery] GetProductsManageRequestDTO request)
    {
        var requestingUser = GetRequestingUser();
        var products = _productService.GetProducts(request, requestingUser.Role);
        return Ok(products);
    }

    [AdministrativeOnly]
    [HttpPost]
    public IActionResult CreateProduct([FromBody] CreateProductRequestDTO request)
    {
        var requestingUser = GetRequestingUser();
        var product = _productService.CreateProduct(request, requestingUser.Email);
        return Created(" ", product);
    }

    [AdministrativeOnly]
    [HttpPut("{id}")]
    public IActionResult UpdateProduct(int id, [FromBody] UpdateProductRequestDTO request)
    {
        var requestingUser = GetRequestingUser();
        var product = _productService.UpdateProduct(id, request, requestingUser.Email);
        return Ok(product);
    }
}
