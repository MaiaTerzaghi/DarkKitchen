using DarkKitchen.BusinessLogic.Security;
using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogicTest.Security;

[TestClass]
public sealed class PasswordManagerTest
{
    private const string Contrasena1 = "Contrasena1!@#$%";
    private const string Otracontrasena1 = "OtraContrasena1!";

    private IPasswordManager _passwordManager = null!;

    [TestInitialize]
    public void Setup()
    {
        _passwordManager = new PasswordManager();
    }

    [TestMethod]
    public void ComputeHash_WhenCalledTwiceWithSameInput_ReturnsSameHash()
    {
        var password = Contrasena1;

        var hash1 = _passwordManager.ComputeHash(password);
        var hash2 = _passwordManager.ComputeHash(password);

        Assert.AreEqual(hash1, hash2);
    }

    [TestMethod]
    public void ComputeHash_WhenDifferentInputs_ReturnsDifferentHashes()
    {
        var password1 = Contrasena1;
        var password2 = Otracontrasena1;

        var hash1 = _passwordManager.ComputeHash(password1);
        var hash2 = _passwordManager.ComputeHash(password2);

        Assert.AreNotEqual(hash1, hash2);
    }
}
