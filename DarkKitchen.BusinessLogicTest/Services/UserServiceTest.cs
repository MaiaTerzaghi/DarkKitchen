using System;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DarkKitchen.BusinessLogic.Test.Services;

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
    }
}
