using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain.Entities;

namespace DarkKitchen.BusinessLogic.Services;

public class SessionService(IUserRepository userRepository) : ISessionService
{
    private readonly IUserRepository _userRepository = userRepository;

    public User GetUserFromToken(string token)
    {
        var session = _userRepository.GetSessionByToken(token);
        if(session == null || session.User == null)
        {
            throw new ArgumentException("Token inválido");
        }

        return session.User;
    }
}
