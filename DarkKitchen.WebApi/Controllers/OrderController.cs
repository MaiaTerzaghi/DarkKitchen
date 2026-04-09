using DarkKitchen.BusinessLogic.Args.In;
using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain.Enums;
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

    [AuthorizeRoles(UserRole.Dispatcher)]
    [HttpGet]
    public IActionResult GetOrders([FromQuery] GetOrdersRequestDTO request)
    {
        var response = _orderService.GetOrders(request);
        return Ok(response);
    }
}
