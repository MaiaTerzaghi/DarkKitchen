using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class UserRepository(DarkKitchenContext context) : IUserRepository
{
    private readonly DarkKitchenContext _context = context;

    public User? GetByEmail(string email)
    {
        return _context.Users.FirstOrDefault(u => u.Email == email);
    }

    public void AddSession(Session session)
    {
        _context.Sessions.Add(session);
        _context.SaveChanges();
    }

    public Session? GetSessionByToken(string token)
    {
        return _context.Sessions
        .Include(s => s.User)
        .FirstOrDefault(s => s.Token == token);
    }

    public int AddUser(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
        return user.Id;
    }
}
