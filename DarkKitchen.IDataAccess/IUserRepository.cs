using DarkKitchen.Domain.Entities;

namespace DarkKitchen.IDataAccess;
public interface IUserRepository
{
    User? GetByEmail(string email);
    void AddSession(Session session);
    Session? GetSessionByToken(string token);
    int AddUser(User user);
}
