using DarkKitchen.DTOs.Args.Output;

namespace DarkKitchen.IBusinessLogic;

public interface IAuthService
{
    LoginResponseDTO Login(string email, string password);
    void Logout(string token);
}
