using DarkKitchen.Domain.Entities;

namespace DarkKitchen.IDataAccess;
public interface IUserRepository
{
    User? GetByEmail(string email);
    User? GetById(int id);
    void AddSession(Session session);
    Session? GetSessionByToken(string token);
    int AddUser(User user);
    List<User> GetUsers(string? name, string? lastName);
}
