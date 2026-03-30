using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Interfaces;

namespace DarkKitchen.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly List<User> _users =
    [
        new User { Id = 1, Email = "juan@email.com", Password = "Contrasena1!@#$%" },
    ];

    private readonly List<Session> _sessions = [];

    public User? GetByEmail(string email)
    {
        return _users.FirstOrDefault(u => u.Email == email);
    }

    public void AddSession(Session session)
    {
        _sessions.Add(session);
    }

    public Session? GetSessionByToken(string token)
    {
        return _sessions.FirstOrDefault(s => s.Token == token);
    }
}
