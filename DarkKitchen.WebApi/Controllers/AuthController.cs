using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.WebApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost("login")]
    public IActionResult Login(LoginRequestDTO request)
    {
        var token = _userService.Login(request.Email, request.Password);
        return Ok(token);
    }
}
