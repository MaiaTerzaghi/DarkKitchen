using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.DTOs.Args.Output;
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
        userServiceMock.Setup(s => s.GetUsers(It.IsAny<string?>(), It.IsAny<string?>()))
                    .Returns(users);

        var controller = new UserController(userServiceMock.Object);

        var result = controller.GetUsers(null, null);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void UpdateUser_WhenValidData_ReturnsOk()
    {
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(s => s.UpdateUser(It.IsAny<int>(), It.IsAny<UpdateUserRequestDTO>(), It.IsAny<int>()))
            .Returns(new UserResponseDTO { Id = 1, Name = "Juan", LastName = "Perez", Email = "juan@test.com", Phone = "+59899123456", Role = UserRole.Administrative });

        var controller = new UserController(userServiceMock.Object);
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
}
