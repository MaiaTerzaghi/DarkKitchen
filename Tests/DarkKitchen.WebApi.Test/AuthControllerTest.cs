using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
namespace DarkKitchen.WebApi.Test;

[TestClass]
public sealed class AuthControllerTest
{
    [TestMethod]
    public void Login_WhenValidCredentials_ReturnsOk()
    {
        var authServiceMock = new Mock<IAuthService>();
        authServiceMock.Setup(s => s.Login("juan@email.com", "Contrasena1!@#$%"))
                       .Returns("token-generado");

        var controller = new AuthController(authServiceMock.Object);
        var request = new LoginRequestDTO
        {
            Email = "juan@email.com",
            Password = "Contrasena1!@#$%"
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
                    .Throws(new ArgumentException("Credenciales inválidas"));

        var controller = new AuthController(authServiceMock.Object);
        var request = new LoginRequestDTO
        {
            Email = "juan@email.com",
            Password = "Contrasena1!@#$%"
        };

        controller.Login(request);
    }

    [TestMethod]
    public void Logout_WhenValidToken_ReturnsOk()
    {
        var authServiceMock = new Mock<IAuthService>();
        authServiceMock.Setup(s => s.Logout("token-valido"));

        var controller = new AuthController(authServiceMock.Object);

        var result = controller.Logout("token-valido");

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }
}
