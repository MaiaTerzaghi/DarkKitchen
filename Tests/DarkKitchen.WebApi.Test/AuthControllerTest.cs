// using System;
using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.WebApi.Controllers;
using DarkKitchen.WebApi.DTOs;
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

        var result = controller.Login(request);

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }
}
