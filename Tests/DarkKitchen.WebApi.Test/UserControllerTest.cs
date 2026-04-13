using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IBusinessLogic;
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

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenServiceThrowsException_ReturnsBadRequest()
    {
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(s => s.Register(It.IsAny<User>()))
                    .Throws(new ArgumentException("Error"));

        var controller = new UserController(userServiceMock.Object);
        var request = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%"
        };

        controller.Register(request);
    }

    [TestMethod]
    public void CreateStaffUser_WhenValidData_ReturnsCreated()
    {
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(s => s.CreateStaffUser(It.IsAny<CreateStaffUserRequestDTO>()))
               .Returns(1);

        var controller = new UserController(userServiceMock.Object);
        var request = new CreateStaffUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Administrative
        };

        var result = controller.CreateStaffUser(request);

        Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
    }
}
