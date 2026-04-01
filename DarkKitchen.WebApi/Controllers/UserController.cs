using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.WebApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost("register")]
    public IActionResult Register(RegisterClientDTO request)
    {
        var user = new User
        {
            Name = request.Name,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Password = request.Password,
            Role = UserRole.Client
        };

        var id = _userService.Register(user);
        return CreatedAtAction(nameof(Register), new { id }, null);
    }
}
