using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Interfaces;
using Moq;

namespace DarkKitchen.BusinessLogicTest.Services;

[TestClass]
public sealed class SessionServiceTest
{
    [TestMethod]
    public void GetUserFromToken_WhenValidToken_ReturnsUser()
    {
        var user = new User { Id = 1, Email = "juan@email.com", Role = UserRole.Client };
        var session = new Session { Token = "valid-token", User = user };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetSessionByToken("valid-token"))
                          .Returns(session);

        var sessionService = new SessionService(userRepositoryMock.Object);
        var result = sessionService.GetUserFromToken("valid-token");

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
    }
}
