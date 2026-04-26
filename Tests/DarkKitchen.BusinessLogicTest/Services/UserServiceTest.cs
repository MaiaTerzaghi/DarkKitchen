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

        var user = new User { Id = 1 };

        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                        .Returns((User?)null);
        _userRepositoryMock.Setup(r => r.Add(It.IsAny<User>()))
                        .Returns(user);

        var result = _service.Register(request);

        Assert.AreEqual(1, result);
    }

    [TestMethod]
    [ExpectedException(typeof(ConflictException))]
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

        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                        .Returns(new User { Email = "juanexistente@email.com" });

        _service.Register(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenNameIsEmpty_ThrowsException()
    {
        var request = new RegisterClientDTO
        {
            Name = string.Empty,
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
        };

        _service.Register(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenLastNameIsTooShort_ThrowsException()
    {
        var request = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Pe",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
        };

        _service.Register(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenEmailIsInvalid_ThrowsException()
    {
        var request = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "emailinvalido",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
        };

        _service.Register(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenPhoneIsInvalid_ThrowsException()
    {
        var request = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "123",
            Password = "Contrasena1!@#$%",
        };

        _service.Register(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenPasswordIsInvalid_ThrowsException()
    {
        var request = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "password",
        };

        _service.Register(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenPasswordIsTooShort_ThrowsException()
    {
        var user = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Corta1!@#$%",
        };

        _service.Register(user);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenPasswordHasNoUppercase_ThrowsException()
    {
        var user = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "contrasena1!@#$%",
        };

        _service.Register(user);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenPasswordHasNoLowercase_ThrowsException()
    {
        var user = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "CONTRASENA1!@#$%",
        };

        _service.Register(user);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenPasswordHasNoNumber_ThrowsException()
    {
        var user = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena!@#$%&*",
        };

        _service.Register(user);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenPasswordHasNoSymbol_ThrowsException()
    {
        var user = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena11111",
        };

        _service.Register(user);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Register_WhenPasswordHasSequence_ThrowsException()
    {
        var user = new RegisterClientDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena123!@#",
        };

        _service.Register(user);
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

        var user = new User { Id = 1 };

        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                            .Returns((User?)null);
        _userRepositoryMock.Setup(r => r.Add(It.IsAny<User>()))
                            .Returns(user);

        var result = _service.CreateStaffUser(request);

        Assert.AreEqual(1, result);
    }

    [TestMethod]
    [ExpectedException(typeof(ConflictException))]
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

        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                        .Returns(new User { Email = "juan@test.com" });

        _service.CreateStaffUser(request);
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

        _userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                        .Returns((User?)null);

        _service.CreateStaffUser(request);
    }

    [TestMethod]
    public void GetUsers_WhenCalled_ReturnsUsers()
    {
        var users = new List<User>
        {
            new() { Id = 1, Name = "Juan", LastName = "Perez", Email = "juan@test.com", Phone = "+59899123456", Role = UserRole.Administrative }
        };

        _userRepositoryMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>(), null, false, 1, 20))
                            .Returns(users);

        var result = _service.GetUsers(null, null);

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
    public void Register_WhenValidData_SavesUserWithHashedPassword()
    {
        var plainPassword = "Contrasena1!@#$%";
        var hashedPassword = "hashed-contrasena";

        var request = new RegisterClientDTO
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

        _service.Register(request);

        _userRepositoryMock.Verify(r => r.Add(It.Is<User>(u => u.Password == hashedPassword)), Times.Once);
    }

    [TestMethod]
    public void CreateStaffUser_WhenValidData_SavesUserWithHashedPassword()
    {
        var plainPassword = "Contrasena1!@#$%";
        var hashedPassword = "hashed-contrasena";

        var request = new CreateStaffUserRequestDTO
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

        _service.CreateStaffUser(request);

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
