using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Filters.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(IUserService userService) : DarkKitchenControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost]
    public IActionResult CreateUser(CreateUserRequestDTO request)
    {
        var id = _userService.CreateUser(request);
        return Created(string.Empty, new { id });
    }

    [AdministrativeOnly]
    [HttpGet]
    public IActionResult GetUsers([FromQuery] string? name, [FromQuery] string? lastName, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var users = _userService.GetUsers(name, lastName, page, pageSize);
        return Ok(users);
    }

    [AdministrativeOnly]
    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, UpdateUserRequestDTO request)
    {
        var requestingUser = GetRequestingUser();
        var result = _userService.UpdateUser(id, request, requestingUser.Id);
        return Ok(result);
    }

    [AdministrativeOnly]
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var requestingUser = GetRequestingUser();
        _userService.DeleteUser(id, requestingUser.Id);
        return NoContent();
    }
}
