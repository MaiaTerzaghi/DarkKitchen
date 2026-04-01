using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Interfaces;

namespace DarkKitchen.BusinessLogic.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;

    public string Login(string email, string password)
    {
        var user = _userRepository.GetByEmail(email);
        if(user == null || user.Password != password)
        {
            throw new Exception("Datos inválidos");
        }

        var session = new Session { User = user };
        _userRepository.AddSession(session);
        return session.Token;
    }

     public int Register(User user)
    {
        throw new NotImplementedException();
    }
}
