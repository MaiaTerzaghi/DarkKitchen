// using System;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Interfaces;
using Moq;

namespace DarkKitchen.BusinessLogicTest.Services;

[TestClass]
public sealed class UserServiceTest
{
    [TestMethod]
    public void Login_WhenValidCredentials_ReturnsToken()
    {
        var user = new User
        {
            Id = 1,
            Email = "juan@email.com",
            Password = "Contrasena1!@#$%"
        };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetByEmail("juan@email.com"))
                          .Returns(user);

        var userService = new UserService(userRepositoryMock.Object);

        var result = userService.Login("juan@email.com", "Contrasena1!@#$%");

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(string));
    }

    [TestMethod]
    public void Login_WhenInvalidCredentials_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetByEmail("mal@email.com"))
                        .Returns((User?)null);

        var userService = new UserService(userRepositoryMock.Object);

        Assert.ThrowsException<Exception>(() =>
            userService.Login("mal@email.com", "contrasenamal"));
    }

    [TestMethod]
    public void Register_WhenValidData_ReturnsId()
    {
        var user = new User
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Client
        };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.AddUser(user))
                        .Returns(1);

        var userService = new UserService(userRepositoryMock.Object);

        var result = userService.Register(user);

        Assert.AreEqual(1, result);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void Register_WhenNameIsEmpty_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var user = new User
        {
            Name = string.Empty,
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Client
        };

        userService.Register(user);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void Register_WhenLastNameIsTooShort_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var user = new User
        {
            Name = "Juan",
            LastName = "Pe",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Client
        };

        userService.Register(user);
    }
}
