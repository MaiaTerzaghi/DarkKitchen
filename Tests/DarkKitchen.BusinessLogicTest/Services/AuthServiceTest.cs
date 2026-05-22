using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.DTOs.Args.Output;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogicTest.Services;

[TestClass]
public sealed class AuthServiceTest
{
    [TestMethod]
    public void Login_WhenValidCredentials_ReturnsToken()
    {
        var hashedPassword = "hashed-contrasena";
        var user = new User
        {
            Id = 1,
            Email = "juan@email.com",
            Password = hashedPassword
        };

        var userRepositoryMock = new Mock<IRepository<User>>();
        var sessionRepositoryMock = new Mock<ISessionRepository>();
        var passwordManagerMock = new Mock<IPasswordManager>();

        userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                          .Returns(user);

        sessionRepositoryMock.Setup(r => r.Add(It.IsAny<Session>()))
                    .Returns(new Session { User = user });

        passwordManagerMock.Setup(p => p.ComputeHash("Contrasena1!@#$%"))
                    .Returns(hashedPassword);

        var authService = new AuthService(userRepositoryMock.Object, sessionRepositoryMock.Object, passwordManagerMock.Object);

        var result = authService.Login("juan@email.com", "Contrasena1!@#$%");

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(LoginResponseDTO));
        Assert.IsNotNull(result.Token);
        Assert.IsNotNull(result.Role);
    }

    [TestMethod]
    [ExpectedException(typeof(UnauthorizedException))]
    public void Login_WhenInvalidCredentials_ThrowsException()
    {
        var userRepositoryMock = new Mock<IRepository<User>>();
        var sessionRepositoryMock = new Mock<ISessionRepository>();
        var passwordManagerMock = new Mock<IPasswordManager>();

        userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                        .Returns((User?)null);

        var authService = new AuthService(userRepositoryMock.Object, sessionRepositoryMock.Object, passwordManagerMock.Object);

        authService.Login("mal@test.com", "Contrasena1!@#$%");
    }

    [TestMethod]
    public void Logout_WhenValidToken_DeletesSession()
    {
        var session = new Session { Token = "token-valido", UserId = 1 };

        var userRepositoryMock = new Mock<IRepository<User>>();
        var sessionRepositoryMock = new Mock<ISessionRepository>();
        var passwordManagerMock = new Mock<IPasswordManager>();

        sessionRepositoryMock.Setup(r => r.GetSessionByToken("token-valido"))
                             .Returns(session);

        var authService = new AuthService(userRepositoryMock.Object, sessionRepositoryMock.Object, passwordManagerMock.Object);

        authService.Logout("token-valido");

        sessionRepositoryMock.Verify(r => r.Delete(session), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(UnauthorizedException))]
    public void Logout_WhenInvalidToken_ThrowsException()
    {
        var userRepositoryMock = new Mock<IRepository<User>>();
        var sessionRepositoryMock = new Mock<ISessionRepository>();
        var passwordManagerMock = new Mock<IPasswordManager>();

        sessionRepositoryMock.Setup(r => r.GetSessionByToken("token-invalido"))
                             .Returns((Session?)null);

        var authService = new AuthService(userRepositoryMock.Object, sessionRepositoryMock.Object, passwordManagerMock.Object);

        authService.Logout("token-invalido");
    }
}
