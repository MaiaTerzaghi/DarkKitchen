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
    [HttpGet("date")]
    public IActionResult GetOrders([FromQuery] GetOrdersRequestDTO request)
    {
        var response = _orderService.GetOrders(request);
        return Ok(response);
    }

    // [AuthorizeRoles(UserRole.Dispatcher, UserRole.Administrative)]
    [HttpPatch("{orderId}/prepared")]
    public IActionResult MarkAsPrepared(int orderId)
    {
        var response = _orderService.MarkAsPrepared(orderId);
        return Ok(response);
    }

    [AuthorizeRoles(UserRole.Dispatcher, UserRole.Administrative)]
    [HttpGet("{id}")]
    public IActionResult GetOrderDetail(int id)
    {
        var order = _orderService.GetOrderDetail(id);
        return Ok(order);
    }

    [AuthorizeRoles(UserRole.Dispatcher)]
    [HttpPatch("{id}/deliver")]
    public IActionResult DeliverOrder(int id)
    {
        var result = _orderService.DeliverOrder(id);
        return Ok(result);
    }

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpPatch("{orderId}/cancel")]
    public IActionResult CancelOrder(int orderId)
    {
        throw new NotImplementedException();
    }
}
