using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogicTest.Services;

[TestClass]
public sealed class AuthServiceTest
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

        var userRepositoryMock = new Mock<IRepository<User>>();
        var sessionRepositoryMock = new Mock<ISessionRepository>();

        userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                          .Returns(user);

        sessionRepositoryMock.Setup(r => r.Add(It.IsAny<Session>()))
                    .Returns(new Session { User = user });

        var authService = new AuthService(userRepositoryMock.Object, sessionRepositoryMock.Object);

        var result = authService.Login("juan@email.com", "Contrasena1!@#$%");

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(string));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Login_WhenInvalidCredentials_ThrowsException()
    {
        var userRepositoryMock = new Mock<IRepository<User>>();
        var sessionRepositoryMock = new Mock<ISessionRepository>();
        userRepositoryMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
                        .Returns((User?)null);

        var authService = new AuthService(userRepositoryMock.Object, sessionRepositoryMock.Object);

        authService.Login("mal@test.com", "Contrasena1!@#$%");
    }
}
