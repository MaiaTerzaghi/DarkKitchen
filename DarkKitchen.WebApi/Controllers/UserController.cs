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

    [HttpPost]
    public IActionResult Register(RegisterClientDTO request)
    {
        var id = _userService.Register(request);

        return Ok(id);
    }

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpPost("staff")]
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
        var requestingUser = (User)HttpContext.Items["RequestingUser"]!;
        var result = _userService.UpdateUser(id, request, requestingUser.Id);
        return Ok(result);
    }

    [AuthorizeRoles(UserRole.Administrative)]
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var requestingUser = (User)HttpContext.Items["RequestingUser"]!;
        _userService.DeleteUser(id, requestingUser.Id);
        return Ok(new { id });
    }
}
