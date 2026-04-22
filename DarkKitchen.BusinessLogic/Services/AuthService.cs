using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Services;

public class AuthService(IRepository<User> userRepository, ISessionRepository sessionRepository) : IAuthService
{
    private readonly IRepository<User> _userRepository = userRepository;
#pragma warning disable CA1823
    private readonly ISessionRepository _sessionRepository = sessionRepository;
#pragma warning restore CA1823
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
