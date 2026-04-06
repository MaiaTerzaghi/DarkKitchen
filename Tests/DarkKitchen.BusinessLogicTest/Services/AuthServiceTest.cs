using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Interfaces;
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

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetByEmail("juan@email.com"))
                          .Returns(user);

        var authService = new AuthService(userRepositoryMock.Object);

        var result = authService.Login("juan@email.com", "Contrasena1!@#$%");

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(string));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Login_WhenInvalidCredentials_ThrowsException()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetByEmail("mal@email.com"))
                        .Returns((User?)null);

        var authService = new AuthService(userRepositoryMock.Object);

        authService.Login("mal@test.com", "Contrasena1!@#$%");
    }
}
