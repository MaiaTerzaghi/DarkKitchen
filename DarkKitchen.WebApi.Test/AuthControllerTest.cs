using System;
using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.WebApi.Controllers;
using DarkKitchen.WebApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
namespace DarkKitchen.WebApi.Test;

[TestClass]
public sealed class AuthControllerTest
{
    [TestMethod]
    public void Login_WhenValidCredentials_ReturnsOk()
    {
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(s => s.Login("juan@email.com", "Contrasena1!@#$%"))
                       .Returns("token-generado");

        var controller = new AuthController(userServiceMock.Object);
        var request = new LoginRequestDTO
        {
            Email = "juan@email.com",
            Password = "Contrasena1!@#$%"
        };

        var result = controller.Login(request);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }
}
