using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Interfaces;

public interface IUserRepository
{
    User? GetByEmail(string email);
    void AddSession(Session session);
    Session? GetSessionByToken(string token);
    int AddUser(User user);
}
