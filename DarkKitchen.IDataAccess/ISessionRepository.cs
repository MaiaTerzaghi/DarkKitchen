using DarkKitchen.Domain.Entities;

namespace DarkKitchen.IDataAccess;

public interface ISessionRepository : IRepository<Session>
{
    Session? GetSessionByToken(string token);
}
