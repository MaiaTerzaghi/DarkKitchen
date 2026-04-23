using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Services;

public class AuthService(IRepository<User> userRepository, ISessionRepository sessionRepository) : IAuthService
{
    private readonly IRepository<User> _userRepository = userRepository;
    private readonly ISessionRepository _sessionRepository = sessionRepository;
    public string Login(string email, string password)
    {
        var user = _userRepository.Get(u => u.Email == email);
        if(user == null || user.Password != password)
        {
            throw new ArgumentException("Credenciales inválidas");
        }

        var session = new Session { User = user };
        _sessionRepository.Add(session);
        return session.Token;
    }
}
