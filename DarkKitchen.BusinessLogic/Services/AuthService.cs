using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Interfaces;

namespace DarkKitchen.BusinessLogic.Services;

public class AuthService(IUserRepository userRepository) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;

    public string Login(string email, string password)
    {
        var user = _userRepository.GetByEmail(email);
        if(user == null || user.Password != password)
        {
            throw new ArgumentException("Credenciales inválidas");
        }

        var session = new Session { User = user };
        _userRepository.AddSession(session);
        return session.Token;
    }
}
