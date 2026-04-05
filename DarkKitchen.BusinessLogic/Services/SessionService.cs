using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Interfaces;

namespace DarkKitchen.BusinessLogic.Services;

public class SessionService(IUserRepository userRepository) : ISessionService
{
    private readonly IUserRepository _userRepository = userRepository;

    public User GetUserFromToken(string token)
    {
        var session = _userRepository.GetSessionByToken(token);

        return session.User;
    }
}
