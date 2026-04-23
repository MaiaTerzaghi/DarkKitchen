using System.Security.Cryptography;
using System.Text;
using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogic.Security;

public class PasswordManager : IPasswordManager
{
    public string ComputeHash(string password)
    {
        using var sha = SHA256.Create();
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = sha.ComputeHash(passwordBytes);
        return Convert.ToBase64String(hashBytes);
    }
}
