using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController(IOrderService orderService) : DarkKitchenControllerBase
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
        var requestingUser = GetRequestingUser();
        var response = _orderService.CreateOrder(request, requestingUser.Id);
        return CreatedAtAction(nameof(GetOrderDetail), new { id = response.OrderId }, response);
    }

    [AuthorizeRoles(UserRole.Client, UserRole.Administrative, UserRole.Dispatcher)]
    [HttpGet]
    public IActionResult GetOrders([FromQuery] GetOrdersRequestDTO request)
    {
        var requestingUser = GetRequestingUser();
        var orders = _orderService.GetOrders(request, requestingUser.Role, requestingUser.Id);
        return Ok(orders);
    }

    [AuthorizeRoles(UserRole.Dispatcher, UserRole.Administrative)]
    [HttpGet("{id}")]
    public IActionResult GetOrderDetail(int id)
    {
        var order = _orderService.GetOrderDetail(id);
        return Ok(order);
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

    [AuthorizeRoles(UserRole.Dispatcher, UserRole.Administrative)]
    [HttpPatch("{id}/status")]
    public IActionResult ChangeStatus(int id, [FromBody] ChangeOrderStatusRequestDTO request)
    {
        var requestingUser = (User)HttpContext.Items["RequestingUser"]!;
        var response = _orderService.ChangeStatus(id, request.Status, requestingUser.Role);
        return Ok(response);
    }
}
