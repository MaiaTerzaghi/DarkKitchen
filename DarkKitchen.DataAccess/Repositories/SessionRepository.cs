using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class SessionRepository(DarkKitchenContext context)
    : Repository<Session>(context), ISessionRepository
{
    public Session? GetSessionByToken(string token)
    {
        return context.Sessions
            .Include(s => s.User)
            .FirstOrDefault(s => s.Token == token);
    }
}
