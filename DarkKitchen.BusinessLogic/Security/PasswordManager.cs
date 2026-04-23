using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogic.Security;

public class PasswordManager : IPasswordManager
{
    public string ComputeHash(string password)
    {
        return "hash";
    }
}
