using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Filters.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController(IOrderService orderService) : DarkKitchenControllerBase
{
    private readonly IOrderService _orderService = orderService;

    [ClientOnly]
    [HttpPost("preview")]
    public IActionResult PreviewOrder([FromBody] PreviewOrderRequestDTO request)
    {
        var response = _orderService.PreviewOrder(request.Items, request.ShippingType);
        return Ok(response);
    }

    [ClientOnly]
    [HttpPost]
    public IActionResult CreateOrder([FromBody] CreateOrderRequestDTO request)
    {
        var requestingUser = GetRequestingUser();
        var response = _orderService.CreateOrder(request, requestingUser.Id);
        return CreatedAtAction(nameof(GetOrderDetail), new { id = response.OrderId }, response);
    }

    [AnyRole]
    [HttpGet]
    public IActionResult GetOrders([FromQuery] GetOrdersRequestDTO request)
    {
        var requestingUser = GetRequestingUser();
        var orders = _orderService.GetOrders(request, requestingUser.Role, requestingUser.Id);
        return Ok(orders);
    }

    [AdministrativeOrDispatcher]
    [HttpGet("{id}")]
    public IActionResult GetOrderDetail(int id)
    {
        var order = _orderService.GetOrderDetail(id);
        return Ok(order);
    }

    [AdministrativeOnly]
    [HttpGet("report")]
    public IActionResult GetSalesReport([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var response = _orderService.GetSalesReport(page, pageSize);
        return Ok(response);
    }

    [AdministrativeOnly]
    [HttpGet("top-products")]
    public IActionResult GetTopProducts([FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo)
    {
        var response = _orderService.GetTopProducts(dateFrom, dateTo);
        return Ok(response);
    }

    [DispatcherOnly]
    [HttpGet("dispatcher")]
    public IActionResult GetDispatcherOrders()
    {
        var response = _orderService.GetDispatcherOrders();
        return Ok(response);
    }

    [AdministrativeOrDispatcher]
    [HttpPatch("{id}/status")]
    public IActionResult ChangeStatus(int id, [FromBody] ChangeOrderStatusRequestDTO request)
    {
        var requestingUser = GetRequestingUser();
        var response = _orderService.ChangeStatus(id, request.Status, requestingUser.Role);
        return Ok(response);
    }
}
