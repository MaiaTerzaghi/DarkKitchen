using DarkKitchen.Domain.Entities;
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
    [HttpPost]
    public IActionResult CreateOrder([FromBody] CreateOrderRequestDTO request)
    {
        var requestingUser = (User)HttpContext.Items["RequestingUser"]!;
        request.ClientId = requestingUser.Id;
        var response = _orderService.CreateOrder(request);
        return Ok(response);
    }

    [AuthorizeRoles(UserRole.Client)]
    [HttpGet]
    public IActionResult GetClientOrders([FromQuery] GetClientOrdersRequestDTO request)
    {
        var requestingUser = (User)HttpContext.Items["RequestingUser"]!;
        request.ClientId = requestingUser.Id;
        var orders = _orderService.GetClientOrders(request);
        return Ok(orders);
    }

    // [AuthorizeRoles(UserRole.Dispatcher)]
    [HttpGet("by-date")]
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

    // [AuthorizeRoles(UserRole.Administrative)]
    [HttpPatch("{orderId}/cancel")]
    public IActionResult CancelOrder(int orderId)
    {
        var response = _orderService.CancelOrder(orderId);
        return Ok(response);
    }

    // [AuthorizeRoles(UserRole.Dispatcher)]
    [HttpPatch("{id}/on-the-Way")]
    public IActionResult MarkAsOnTheWay(int id)
    {
        var result = _orderService.MarkAsOnTheWay(id);
        return Ok(result);
    }

    // [AuthorizeRoles(UserRole.Dispatcher)]
    [HttpPatch("{orderId}/not-delivered")]
    public IActionResult MarkAsNotDelivered(int orderId)
    {
        var response = _orderService.MarkAsNotDelivered(orderId);
        return Ok(response);
    }

    [HttpGet("report")]
    public IActionResult GetSalesReport([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var response = _orderService.GetSalesReport(page, pageSize);
        return Ok(response);
    }
}
