using DarkKitchen.BusinessLogic.Security;
using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogicTest.Security;

[TestClass]
public sealed class PasswordManagerTest
{
    private IPasswordManager _passwordManager = null!;

    [TestInitialize]
    public void Setup()
    {
        _passwordManager = new PasswordManager();
    }

    [TestMethod]
    public void ComputeHash_WhenCalledTwiceWithSameInput_ReturnsSameHash()
    {
        var password = "Contrasena1!@#$%";

        var hash1 = _passwordManager.ComputeHash(password);
        var hash2 = _passwordManager.ComputeHash(password);

        Assert.AreEqual(hash1, hash2);
    }
}
