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
        userRepositoryMock.Setup(r => r.GetByEmail("juan@email.com"))
                        .Returns((User?)null);
        userRepositoryMock.Setup(r => r.AddUser(user))
                        .Returns(1);

        var userService = new UserService(userRepositoryMock.Object);

        var result = userService.Register(user);

        Assert.AreEqual(1, result);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void Register_WhenEmailAlreadyExists_ThrowsException()
    {
        var user = new User
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juanexistente@email.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Client
        };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetByEmail("juanexistente@email.com"))
                        .Returns(user);

        var userService = new UserService(userRepositoryMock.Object);

        userService.Register(user);
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

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void Register_WhenEmailIsInvalid_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var user = new User
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "emailinvalido",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Client
        };

        userService.Register(user);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void Register_WhenPhoneIsInvalid_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var user = new User
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "123",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Client
        };

        userService.Register(user);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void Register_WhenPasswordIsInvalid_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var user = new User
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "password",
            Role = UserRole.Client
        };

        userService.Register(user);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void Register_WhenPasswordIsTooShort_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var user = new User
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Corta1!@#$%",
            Role = UserRole.Client
        };

        userService.Register(user);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void Register_WhenPasswordHasNoUppercase_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var user = new User
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "contrasena1!@#$%",
            Role = UserRole.Client
        };

        userService.Register(user);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void Register_WhenPasswordHasNoLowercase_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var user = new User
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "CONTRASENA1!@#$%",
            Role = UserRole.Client
        };

        userService.Register(user);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void Register_WhenPasswordHasNoNumber_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var user = new User
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena!@#$%&*",
            Role = UserRole.Client
        };

        userService.Register(user);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void Register_WhenPasswordHasNoSymbol_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var user = new User
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena11111",
            Role = UserRole.Client
        };

        userService.Register(user);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void Register_WhenPasswordHasSequence_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var user = new User
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena123!@#",
            Role = UserRole.Client
        };

        userService.Register(user);
    }
}
