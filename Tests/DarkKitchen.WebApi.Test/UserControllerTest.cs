using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test;

[TestClass]
public sealed class UserControllerTest
{
    [TestMethod]
    public void CreateUser_WhenValidData_ReturnsCreated()
    {
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(s => s.CreateUser(It.IsAny<CreateUserRequestDTO>()))
                       .Returns(1);

        var controller = new UserController(userServiceMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var request = new CreateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%"
        };

        var result = controller.CreateUser(request);

        Assert.IsInstanceOfType(result, typeof(CreatedResult));
        userServiceMock.Verify(s => s.CreateUser(request), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateUser_WhenServiceThrowsException_Throws()
    {
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(s => s.CreateUser(It.IsAny<CreateUserRequestDTO>()))
                    .Throws(new ArgumentException("Error"));

        var controller = new UserController(userServiceMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var request = new CreateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%"
        };

        controller.CreateUser(request);
    }

    [TestMethod]
    public void CreateUser_WhenRoleProvided_ReturnsCreated()
    {
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(s => s.CreateUser(It.IsAny<CreateUserRequestDTO>()))
               .Returns(1);

        var controller = new UserController(userServiceMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var request = new CreateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Administrative
        };

        var result = controller.CreateUser(request);

        Assert.IsInstanceOfType(result, typeof(CreatedResult));
        userServiceMock.Verify(s => s.CreateUser(request), Times.Once);
    }

    [TestMethod]
    public void GetUsers_WhenCalled_ReturnsOk()
    {
        var users = new List<UserResponseDTO>
        {
            new UserResponseDTO
            {
                Id = 1,
                Name = "Juan",
                LastName = "Perez",
                Email = "juan@test.com",
                Phone = "+59899123456",
                Role = UserRole.Administrative
            }
        };

        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(s => s.GetUsers(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(users);

        var controller = new UserController(userServiceMock.Object);

        var result = controller.GetUsers(null, null, 1, 20);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void UpdateUser_WhenValidData_ReturnsOk()
    {
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(s => s.UpdateUser(It.IsAny<int>(), It.IsAny<UpdateUserRequestDTO>(), It.IsAny<int>()))
            .Returns(new UserResponseDTO { Id = 1, Name = "Juan", LastName = "Perez", Email = "juan@test.com", Phone = "+59899123456", Role = UserRole.Administrative });

        var controller = new UserController(userServiceMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        controller.HttpContext.Items["RequestingUser"] = new User { Id = 2 };

        var request = new UpdateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Administrative
        };

        var result = controller.UpdateUser(1, request);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void DeleteUser_WhenValidId_ReturnsOk()
    {
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(s => s.DeleteUser(It.IsAny<int>(), It.IsAny<int>()));

        var controller = new UserController(userServiceMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        controller.HttpContext.Items["RequestingUser"] = new User { Id = 2 };

        var result = controller.DeleteUser(1);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }
}
