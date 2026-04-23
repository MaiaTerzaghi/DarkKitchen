using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
namespace DarkKitchen.BusinessLogic.Services;

public class SessionService(ISessionRepository sessionRepository) : ISessionService
{
    private readonly ISessionRepository _sessionRepository = sessionRepository;

    public User GetUserFromToken(string token)
    {
        var session = _sessionRepository.GetSessionByToken(token);
        if(session == null || session.User == null)
        {
            throw new ArgumentException("Token inválido");
        }

        return session.User;
    }
}
