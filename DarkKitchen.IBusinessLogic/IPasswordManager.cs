namespace DarkKitchen.IBusinessLogic;

public interface IPasswordManager
{
    string ComputeHash(string password);
}
