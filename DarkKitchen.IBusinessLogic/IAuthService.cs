namespace DarkKitchen.IBusinessLogic;

public interface IAuthService
{
    string Login(string email, string password);
    void Logout(string token);
}
