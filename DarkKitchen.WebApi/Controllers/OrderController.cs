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
    [HttpPost("preview")]
    public IActionResult PreviewOrder([FromBody] PreviewOrderRequestDTO request)
    {
        var response = _orderService.PreviewOrder(request.Items, request.ShippingType);
        return Ok(response);
    }

    [AuthorizeRoles(UserRole.Client)]
    [HttpPost]
    public IActionResult CreateOrder([FromBody] CreateOrderRequestDTO request)
    {
        var requestingUser = (User)HttpContext.Items["RequestingUser"]!;
        var response = _orderService.CreateOrder(request, requestingUser.Id);
        return CreatedAtAction(nameof(GetOrderDetail), new { id = response.OrderId }, response);
    }

    [AuthorizeRoles(UserRole.Client)]
    [HttpGet]
    public IActionResult GetClientOrders([FromQuery] GetClientOrdersRequestDTO request)
    {
        var requestingUser = (User)HttpContext.Items["RequestingUser"]!;
        var orders = _orderService.GetClientOrders(request, requestingUser.Id);
        return Ok(orders);
    }

    [AuthorizeRoles(UserRole.Dispatcher, UserRole.Administrative)]
    [HttpGet("by-date")]
    public IActionResult GetOrders([FromQuery] GetOrdersRequestDTO request)
    {
        var response = _orderService.GetOrders(request);
        return Ok(response);
    }

    [AuthorizeRoles(UserRole.Dispatcher, UserRole.Administrative)]
    [HttpPatch("{id}/prepared")]
    public IActionResult MarkAsPrepared(int id)
    {
        var response = _orderService.MarkAsPrepared(id);
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
    [HttpPatch("{id}/cancel")]
    public IActionResult CancelOrder(int id)
    {
        var response = _orderService.CancelOrder(id);
        return Ok(response);
    }

    [AuthorizeRoles(UserRole.Dispatcher)]
    [HttpPatch("{id}/on-the-way")]
    public IActionResult MarkAsOnTheWay(int id)
    {
        var result = _orderService.MarkAsOnTheWay(id);
        return Ok(result);
    }

    [AuthorizeRoles(UserRole.Dispatcher)]
    [HttpPatch("{id}/not-delivered")]
    public IActionResult MarkAsNotDelivered(int id)
    {
        var response = _orderService.MarkAsNotDelivered(id);
        return Ok(response);
    }

    [AuthorizeRoles(UserRole.Dispatcher)]
    [HttpPatch("{id}/delayed")]
    public IActionResult MarkAsDelayed(int id)
    {
        var response = _orderService.MarkAsDelayed(id);
        return Ok(response);
    }

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpGet("report")]
    public IActionResult GetSalesReport([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var response = _orderService.GetSalesReport(page, pageSize);
        return Ok(response);
    }

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpGet("top-products")]
    public IActionResult GetTopProducts([FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo)
    {
        var response = _orderService.GetTopProducts(dateFrom, dateTo);
        return Ok(response);
    }

    [AuthorizeRoles(UserRole.Dispatcher)]
    [HttpGet("dispatcher")]
    public IActionResult GetDispatcherOrders()
    {
        var response = _orderService.GetDispatcherOrders();
        return Ok(response);
    }
}
