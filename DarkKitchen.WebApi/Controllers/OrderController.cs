using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController(IOrderService orderService) : ControllerBase
{
    private readonly IOrderService _orderService = orderService;

    [AuthorizeRoles(UserRole.Client)]
    [HttpPost("create")]
    public IActionResult CreateOrder([FromBody] CreateOrderRequestDTO request)
    {
        var response = _orderService.CreateOrder(request);
        return Ok(response);
    }

    [AuthorizeRoles(UserRole.Client)]
    [HttpGet]
    public IActionResult GetClientOrders([FromQuery] GetClientOrdersRequestDTO request)
    {
        var orders = _orderService.GetClientOrders(request);
        return Ok(orders);
    }

    // [AuthorizeRoles(UserRole.Dispatcher)]
    [HttpGet]
    public IActionResult GetOrders([FromQuery] GetOrdersRequestDTO request)
    {
        var response = _orderService.GetOrders(request);
        return Ok(response);
    }
}
