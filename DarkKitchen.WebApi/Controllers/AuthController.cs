using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IBusinessLogic;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public IActionResult Login(LoginRequestDTO request)
    {
        var token = _authService.Login(request.Email, request.Password);
        return Ok(token);
    }

    [HttpDelete("logout")]
    public IActionResult Logout([FromHeader] string authorization)
    {
        throw new NotImplementedException();
    }
}
