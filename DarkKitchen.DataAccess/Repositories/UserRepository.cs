using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class UserRepository(DarkKitchenContext context) : IUserRepository
{
    private readonly DarkKitchenContext _context = context;

    public User? GetByEmail(string email)
    {
        return _context.Users.FirstOrDefault(u => u.Email == email);
    }

    public User? GetById(int id)
    {
        return _context.Users.FirstOrDefault(u => u.Id == id);
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

    public List<User> GetUsers(string? name, string? lastName)
    {
        var users = _context.Users.ToList();

        if(name != null)
        {
            users = users.Where(u => u.Name.Contains(name)).ToList();
        }

        if(lastName != null)
        {
            users = users.Where(u => u.LastName.Contains(lastName)).ToList();
        }

        return users;
    }

    public User UpdateUser(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
        return user;
    }

    public void DeleteUser(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        _context.Users.Remove(user!);
        _context.SaveChanges();
    }
}
