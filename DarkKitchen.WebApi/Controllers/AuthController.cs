using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IBusinessLogic;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/sessions")]
public class SessionsController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost]
    public IActionResult Login(LoginRequestDTO request)
    {
        var response = _authService.Login(request.Email, request.Password);
        return Ok(response);
    }

    [HttpDelete]
    public IActionResult Logout([FromHeader] string authorization)
    {
        _authService.Logout(authorization);
        return NoContent();
    }
}
