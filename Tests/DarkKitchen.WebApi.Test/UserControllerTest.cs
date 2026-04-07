using DarkKitchen.BusinessLogic.Args.In;
using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain.Entities;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test;

[TestClass]
public sealed class UserControllerTest
{
    [TestMethod]
    public void Register_WhenValidData_ReturnsCreated()
    {
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(s => s.Register(It.IsAny<User>()))
                       .Returns(1);

        var controller = new UserController(userServiceMock.Object);
        var request = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%"
        };

        var result = controller.Register(request);

        Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
    }
}
