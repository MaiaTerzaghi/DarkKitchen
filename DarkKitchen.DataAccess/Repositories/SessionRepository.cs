using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.DataAccess.Repositories;

public class SessionRepository(DarkKitchenContext context)
    : Repository<Session>(context), ISessionRepository
{
    public Session? GetSessionByToken(string token)
    {
        throw new NotImplementedException();
    }
}
