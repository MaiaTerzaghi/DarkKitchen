using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost("register")]

    // IActionResult me devuelve ademas de lo que yo quiero un codigo HTTP, como 200=OK
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

        return CreatedAtAction(nameof(Register), new { id }, new { id });
    }

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpPost]
    public IActionResult CreateStaffUser(CreateStaffUserRequestDTO request)
    {
        var id = _userService.CreateStaffUser(request);
        return CreatedAtAction(nameof(CreateStaffUser), new { id }, new { id });
    }

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpGet]
    public IActionResult GetUsers([FromQuery] string? name, [FromQuery] string? lastName)
    {
        var users = _userService.GetUsers(name, lastName);
        return Ok(users);
    }

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, UpdateUserRequestDTO request)
    {
        _userService.UpdateUser(id, request, 0);
        return Ok(new { id });
    }
}
