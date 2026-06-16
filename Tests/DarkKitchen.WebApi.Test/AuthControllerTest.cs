using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
namespace DarkKitchen.WebApi.Test;

[TestClass]
public sealed class AuthControllerTest
{
    private const string Contrasena1 = "Contrasena1!@#$%";
    private const string JuanEmailCom = "juan@email.com";
    private const string TokenValido = "token-valido";
    private const string Administrative = "Administrative";
    private const string CredencialesInvalidas = "Credenciales inválidas";
    private const string TokenGenerado = "token-generado";

    [TestMethod]
    public void Login_WhenValidCredentials_ReturnsOk()
    {
        var authServiceMock = new Mock<IAuthService>();
        authServiceMock.Setup(s => s.Login(JuanEmailCom, Contrasena1))
                       .Returns(new LoginResponseDTO { Token = TokenGenerado, Role = Administrative });

        var controller = new SessionsController(authServiceMock.Object);
        var request = new LoginRequestDTO
        {
            Email = JuanEmailCom,
            Password = Contrasena1
        };

        var result = controller.Login(request);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Login_WhenServiceThrowsException_ReturnsBadRequest()
    {
        var authServiceMock = new Mock<IAuthService>();
        authServiceMock.Setup(s => s.Login(It.IsAny<string>(), It.IsAny<string>()))
                    .Throws(new ArgumentException(CredencialesInvalidas));

        var controller = new SessionsController(authServiceMock.Object);
        var request = new LoginRequestDTO
        {
            Email = JuanEmailCom,
            Password = Contrasena1
        };

        controller.Login(request);
    }

    [TestMethod]
    public void Logout_WhenValidToken_ReturnsOk()
    {
        var authServiceMock = new Mock<IAuthService>();
        authServiceMock.Setup(s => s.Logout(TokenValido));

        var controller = new SessionsController(authServiceMock.Object);

        var result = controller.Logout(TokenValido);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }
}
