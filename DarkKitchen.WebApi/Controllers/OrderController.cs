using DarkKitchen.BusinessLogic.Args.In;
using DarkKitchen.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController(IOrderService orderService) : ControllerBase
{
    // TDD Red: se suprime el warning de campo no utilizado temporalmente.
    // _orderService será utilizado en el paso Green cuando se implemente CreateOrder.
#pragma warning disable CA1823, CS9113
    private readonly IOrderService _orderService = orderService;
#pragma warning restore CA1823, CS9113

    [HttpPost("create")]
    public IActionResult CreateOrder([FromBody] CreateOrderRequestDTO request)
    {
        throw new NotImplementedException();
    }
}
