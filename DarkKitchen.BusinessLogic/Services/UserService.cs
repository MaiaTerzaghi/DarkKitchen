using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain.Interfaces;

namespace DarkKitchen.BusinessLogic.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;

    public string Login(string email, string password)
    {
        var user = _userRepository.GetByEmail(email);
        if (user == null || user.Password != password)
        {
            throw new Exception("Credenciales inválidas");
        }

        return user.Id.ToString();
    }
}
