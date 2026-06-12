using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.DTOs.Args.In;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogicTest.Services;

[TestClass]
public sealed class UserServiceTest
{
    private Mock<IRepository<User>> _userRepositoryMock = null!;
    private Mock<IPasswordManager> _passwordManagerMock = null!;
    private UserService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IRepository<User>>();
        _passwordManagerMock = new Mock<IPasswordManager>();
        _service = new UserService(_userRepositoryMock.Object, _passwordManagerMock.Object);
    }

    [TestMethod]
    public void CreateUser_WhenValidData_ReturnsId()
    {
        var request = new CreateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
        };

        var user = new User { Id = 1 };

        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                        .Returns((User?)null);
        _userRepositoryMock.Setup(r => r.Add(It.IsAny<User>()))
                        .Returns(user);

        var result = _service.CreateUser(request);

        Assert.AreEqual(1, result);
    }

    [TestMethod]
    [ExpectedException(typeof(ConflictException))]
    public void CreateUser_WhenEmailAlreadyExists_ThrowsException()
    {
        var request = new CreateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juanexistente@email.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
        };

        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                        .Returns(new User { Email = "juanexistente@email.com" });

        _service.CreateUser(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateUser_WhenNameIsEmpty_ThrowsException()
    {
        var request = new CreateUserRequestDTO
        {
            Name = string.Empty,
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
        };

        _service.CreateUser(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateUser_WhenLastNameIsTooShort_ThrowsException()
    {
        var request = new CreateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Pe",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
        };

        _service.CreateUser(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateUser_WhenEmailIsInvalid_ThrowsException()
    {
        var request = new CreateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "emailinvalido",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
        };

        _service.CreateUser(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateUser_WhenPhoneIsInvalid_ThrowsException()
    {
        var request = new CreateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "123",
            Password = "Contrasena1!@#$%",
        };

        _service.CreateUser(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateUser_WhenPasswordIsInvalid_ThrowsException()
    {
        var request = new CreateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "password",
        };

        _service.CreateUser(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateUser_WhenPasswordIsTooShort_ThrowsException()
    {
        var user = new CreateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Corta1!@#$%",
        };

        _service.CreateUser(user);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateUser_WhenPasswordHasNoUppercase_ThrowsException()
    {
        var user = new CreateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "contrasena1!@#$%",
        };

        _service.CreateUser(user);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateUser_WhenPasswordHasNoLowercase_ThrowsException()
    {
        var user = new CreateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "CONTRASENA1!@#$%",
        };

        _service.CreateUser(user);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateUser_WhenPasswordHasNoNumber_ThrowsException()
    {
        var user = new CreateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena!@#$%&*",
        };

        _service.CreateUser(user);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateUser_WhenPasswordHasNoSymbol_ThrowsException()
    {
        var user = new CreateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena11111",
        };

        _service.CreateUser(user);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateUser_WhenPasswordHasSequence_ThrowsException()
    {
        var user = new CreateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena123!@#",
        };

        _service.CreateUser(user);
    }

    [TestMethod]
    public void CreateUser_WhenStaffWithValidData_ReturnsId()
    {
        var request = new CreateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Administrative
        };

        var user = new User { Id = 1 };

        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                            .Returns((User?)null);
        _userRepositoryMock.Setup(r => r.Add(It.IsAny<User>()))
                            .Returns(user);

        var result = _service.CreateUser(request);

        Assert.AreEqual(1, result);
    }

    [TestMethod]
    [ExpectedException(typeof(ConflictException))]
    public void CreateUser_WhenStaffEmailAlreadyExists_ThrowsException()
    {
        var request = new CreateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Administrative
        };

        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                        .Returns(new User { Email = "juan@test.com" });

        _service.CreateUser(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateUser_WhenRoleIsClient_ThrowsArgumentException()
    {
        var request = new CreateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Client
        };

        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                        .Returns((User?)null);

        _service.CreateUser(request);
    }

    [TestMethod]
    public void GetUsers_WhenCalled_ReturnsUsers()
    {
        var users = new List<User>
        {
            new() { Id = 1, Name = "Juan", LastName = "Perez", Email = "juan@test.com", Phone = "+59899123456", Role = UserRole.Administrative }
        };

        _userRepositoryMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>(), null, false, 1, 20))
                            .Returns((users, users.Count));

        var result = _service.GetUsers(null, null);

        Assert.AreEqual(1, result.Items.Count);
    }

    [TestMethod]
    public void GetUsers_WhenCalled_ExcludesClients()
    {
        var users = new List<User>
        {
            new() { Id = 1, Name = "Admin", LastName = "User", Email = "admin@test.com", Phone = "+59899111111", Role = UserRole.Administrative },
            new() { Id = 2, Name = "Prep", LastName = "User", Email = "prep@test.com", Phone = "+59899222222", Role = UserRole.Dispatcher }
        };

        _userRepositoryMock.Setup(r => r.GetAll(
            It.Is<Expression<Func<User, bool>>>(expr => true),
            null, false, 1, 20))
            .Returns((users, users.Count));

        var result = _service.GetUsers(null, null);

        Assert.AreEqual(2, result.Items.Count);
        Assert.IsTrue(result.Items.All(u => u.Role != UserRole.Client));
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
            Password = null,
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

        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                        .Returns(existingUser);
        _userRepositoryMock.Setup(r => r.Update(It.IsAny<User>())).Returns(existingUser);

        var result = _service.UpdateUser(1, request, 2);

        Assert.AreEqual("Juan", result.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(NotFoundException))]
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

        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                      .Returns((User?)null);

        _service.UpdateUser(1, request, 2);
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

        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(existingUser);

        _service.UpdateUser(1, request, 1);
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

        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(existingUser);

        _service.DeleteUser(1, 2);

        _userRepositoryMock.Verify(r => r.Delete(existingUser), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(NotFoundException))]
    public void DeleteUser_WhenUserNotFound_ThrowsArgumentException()
    {
        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                        .Returns((User?)null);

        _service.DeleteUser(1, 2);
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

        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                        .Returns(existingUser);
        _service.DeleteUser(1, 1);
    }

    [TestMethod]
    public void CreateUser_WhenValidData_SavesUserWithHashedPassword()
    {
        var plainPassword = "Contrasena1!@#$%";
        var hashedPassword = "hashed-contrasena";

        var request = new CreateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = plainPassword,
        };

        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                        .Returns((User?)null);
        _userRepositoryMock.Setup(r => r.Add(It.IsAny<User>()))
                        .Returns(new User { Id = 1 });
        _passwordManagerMock.Setup(p => p.ComputeHash(plainPassword))
                        .Returns(hashedPassword);

        _service.CreateUser(request);

        _userRepositoryMock.Verify(r => r.Add(It.Is<User>(u => u.Password == hashedPassword)), Times.Once);
    }

    [TestMethod]
    public void CreateUser_WhenStaffWithValidData_SavesUserWithHashedPassword()
    {
        var plainPassword = "Contrasena1!@#$%";
        var hashedPassword = "hashed-contrasena";

        var request = new CreateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = plainPassword,
            Role = UserRole.Administrative
        };

        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                        .Returns((User?)null);
        _userRepositoryMock.Setup(r => r.Add(It.IsAny<User>()))
                        .Returns(new User { Id = 1 });
        _passwordManagerMock.Setup(p => p.ComputeHash(plainPassword))
                        .Returns(hashedPassword);

        _service.CreateUser(request);

        _userRepositoryMock.Verify(r => r.Add(It.Is<User>(u => u.Password == hashedPassword)), Times.Once);
    }

    [TestMethod]
    public void UpdateUser_WhenValidData_UpdatesUserWithHashedPassword()
    {
        var plainPassword = "Contrasena1!@#$%";
        var hashedPassword = "hashed-contrasena";

        var request = new UpdateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = plainPassword,
            Role = UserRole.Administrative
        };

        var existingUser = new User
        {
            Id = 1,
            Name = "OldName",
            LastName = "OldLastName",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = "old-hashed-password",
            Role = UserRole.Administrative
        };

        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                        .Returns(existingUser);
        _userRepositoryMock.Setup(r => r.Update(It.IsAny<User>())).Returns(existingUser);
        _passwordManagerMock.Setup(p => p.ComputeHash(plainPassword))
                        .Returns(hashedPassword);

        _service.UpdateUser(1, request, 2);

        _userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.Password == hashedPassword)), Times.Once);
    }

    [TestMethod]
    public void UpdateUser_WhenPasswordIsNull_DoesNotUpdatePassword()
    {
        var request = new UpdateUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = null,
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

        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                        .Returns(existingUser);
        _userRepositoryMock.Setup(r => r.Update(It.IsAny<User>())).Returns(existingUser);

        var result = _service.UpdateUser(1, request, 2);

        Assert.AreEqual("Contrasena1!@#$%", existingUser.Password);
        Assert.AreEqual("Juan", result.Name);
    }
}
