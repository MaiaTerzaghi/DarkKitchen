using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Services;

public class AuthService(IRepository<User> userRepository, ISessionRepository sessionRepository, IPasswordManager passwordManager) : IAuthService
{
    private readonly IRepository<User> _userRepository = userRepository;
    private readonly ISessionRepository _sessionRepository = sessionRepository;
    private readonly IPasswordManager _passwordManager = passwordManager;

    public string Login(string email, string password)
    {
        var user = _userRepository.Get(u => u.Email == email);
        var hashedPassword = _passwordManager.ComputeHash(password);

        if(user == null || user.Password != hashedPassword)
        {
            throw new UnauthorizedException("Credenciales inválidas");
        }

        var session = new Session { UserId = user.Id };
        _sessionRepository.Add(session);
        return session.Token;
    }

    public void Logout(string token)
    {
        var session = _sessionRepository.GetSessionByToken(token)
            ?? throw new UnauthorizedException("Token inválido");

        _sessionRepository.Delete(session);
    }
}
