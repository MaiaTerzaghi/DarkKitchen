using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogicTest.Services;

[TestClass]
public sealed class UserServiceTest
{
    [TestMethod]
    public void Register_WhenValidData_ReturnsId()
    {
        var request = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
        };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetByEmail("juan@email.com"))
                        .Returns((User?)null);
        userRepositoryMock.Setup(r => r.AddUser(It.IsAny<User>()))
                        .Returns(1);

        var userService = new UserService(userRepositoryMock.Object);

        var result = userService.Register(request);

        Assert.AreEqual(1, result);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenEmailAlreadyExists_ThrowsException()
    {
        var request = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juanexistente@email.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
        };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetByEmail("juanexistente@email.com"))
                        .Returns(new User { Email = "juanexistente@email.com" });

        var userService = new UserService(userRepositoryMock.Object);

        userService.Register(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenNameIsEmpty_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var request = new RegisterClientDTO
        {
            Name = string.Empty,
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
        };

        userService.Register(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenLastNameIsTooShort_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var request = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Pe",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
        };

        userService.Register(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenEmailIsInvalid_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var request = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "emailinvalido",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
        };

        userService.Register(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenPhoneIsInvalid_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var request = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "123",
            Password = "Contrasena1!@#$%",
        };

        userService.Register(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenPasswordIsInvalid_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var request = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "password",
        };

        userService.Register(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenPasswordIsTooShort_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var user = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Corta1!@#$%",
        };

        userService.Register(user);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenPasswordHasNoUppercase_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var user = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "contrasena1!@#$%",
        };

        userService.Register(user);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenPasswordHasNoLowercase_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var user = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "CONTRASENA1!@#$%",
        };

        userService.Register(user);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenPasswordHasNoNumber_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var user = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena!@#$%&*",
        };

        userService.Register(user);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenPasswordHasNoSymbol_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var user = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena11111",
        };

        userService.Register(user);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenPasswordHasSequence_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var userService = new UserService(userRepositoryMock.Object);

        var user = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena123!@#",
        };

        userService.Register(user);
    }

    [TestMethod]
    public void CreateStaffUser_WhenValidData_ReturnsId()
    {
        var request = new CreateStaffUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Administrative
        };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.AddUser(It.IsAny<User>()))
                        .Returns(1);

        var userService = new UserService(userRepositoryMock.Object);

        var result = userService.CreateStaffUser(request);

        Assert.AreEqual(1, result);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateStaffUser_WhenEmailAlreadyExists_ThrowsArgumentException()
    {
        var request = new CreateStaffUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Administrative
        };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetByEmail("juan@test.com"))
                        .Returns(new User { Email = "juan@test.com" });

        var userService = new UserService(userRepositoryMock.Object);

        userService.CreateStaffUser(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateStaffUser_WhenRoleIsClient_ThrowsArgumentException()
    {
        var request = new CreateStaffUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Client
        };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetByEmail("juan@test.com"))
                        .Returns((User?)null);

        var userService = new UserService(userRepositoryMock.Object);

        userService.CreateStaffUser(request);
    }

    [TestMethod]
    public void GetUsers_WhenCalled_ReturnsUsers()
    {
        var users = new List<User>
        {
            new() { Id = 1, Name = "Juan", LastName = "Perez", Email = "juan@test.com", Phone = "+59899123456", Role = UserRole.Administrative }
        };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetUsers(It.IsAny<string?>(), It.IsAny<string?>()))
                        .Returns(users);

        var userService = new UserService(userRepositoryMock.Object);

        var result = userService.GetUsers(null, null);

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void UpdateUser_WhenValidData_ReturnsUpdatedUser()
    {
        var request = new UpdateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Administrative
        };

        var existingUser = new User
        {
            Id = 1,
            Name = "OldName",
            LastName = "OldLastName",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Administrative
        };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetById(1)).Returns(existingUser);
        userRepositoryMock.Setup(r => r.UpdateUser(It.IsAny<User>())).Returns(existingUser);

        var userService = new UserService(userRepositoryMock.Object);

        var result = userService.UpdateUser(1, request, 2);

        Assert.AreEqual("Juan", result.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdateUser_WhenUserNotFound_ThrowsArgumentException()
    {
        var request = new UpdateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Administrative
        };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetById(1)).Returns((User?)null);

        var userService = new UserService(userRepositoryMock.Object);

        userService.UpdateUser(1, request, 2);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdateUser_WhenUserModifiesHimself_ThrowsArgumentException()
    {
        var request = new UpdateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Administrative
        };

        var existingUser = new User
        {
            Id = 1,
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Administrative
        };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetById(1)).Returns(existingUser);

        var userService = new UserService(userRepositoryMock.Object);

        userService.UpdateUser(1, request, 1);
    }

    [TestMethod]
    public void DeleteUser_WhenValidId_DeletesUser()
    {
        var existingUser = new User
        {
            Id = 1,
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Administrative
        };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetById(1)).Returns(existingUser);

        var userService = new UserService(userRepositoryMock.Object);

        userService.DeleteUser(1, 2);

        userRepositoryMock.Verify(r => r.DeleteUser(1), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void DeleteUser_WhenUserNotFound_ThrowsArgumentException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetById(1)).Returns((User?)null);

        var userService = new UserService(userRepositoryMock.Object);

        userService.DeleteUser(1, 2);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void DeleteUser_WhenUserDeletesHimself_ThrowsArgumentException()
    {
        var existingUser = new User
        {
            Id = 1,
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Administrative
        };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetById(1)).Returns(existingUser);

        var userService = new UserService(userRepositoryMock.Object);

        userService.DeleteUser(1, 1);
    }
}
