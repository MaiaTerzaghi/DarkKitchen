using DarkKitchen.DataAccess.Context;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test.Repositories;

[TestClass]
public sealed class UserRepositoryTest
{
    private SqliteConnection? _connection;
    private DarkKitchenContext? _context;

    [TestInitialize]
    public void Initialize()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<DarkKitchenContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new DarkKitchenContext(options);
        _context.Database.EnsureCreated();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context?.Dispose();
        _connection?.Dispose();
    }

    [TestMethod]
    public void GetByEmail_WhenUserExists_ReturnsUser()
    {
        var user = new User { Name = "Juan", LastName = "Perez", Email = "juan@test.com", Password = "Contrasena1!@#$%" };
        _context!.Users.Add(user);
        _context.SaveChanges();

        var repository = new UserRepository(_context);
        var result = repository.GetByEmail("juan@test.com");

        Assert.IsNotNull(result);
        Assert.AreEqual("juan@test.com", result.Email);
    }

    [TestMethod]
    public void GetByEmail_WhenUserDoesNotExist_ReturnsNull()
    {
        var repository = new UserRepository(_context!);
        var result = repository.GetByEmail("noexiste@test.com");

        Assert.IsNull(result);
    }

    [TestMethod]
    public void AddSession_WhenValidSession_SavesSession()
    {
        var user = new User { Name = "Juan", LastName = "Perez", Email = "juan@test.com", Password = "Contrasena1!@#$%" };
        _context!.Users.Add(user);
        _context.SaveChanges();

        var repository = new UserRepository(_context);
        var session = new Session { User = user };
        repository.AddSession(session);

        var result = _context.Sessions.FirstOrDefault(s => s.Token == session.Token);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void GetSessionByToken_WhenSessionExists_ReturnsSession()
    {
        var user = new User { Name = "Juan", LastName = "Perez", Email = "juan@test.com", Password = "Contrasena1!@#$%" };
        _context!.Users.Add(user);
        _context.SaveChanges();

        var session = new Session { User = user };
        _context.Sessions.Add(session);
        _context.SaveChanges();

        var repository = new UserRepository(_context);
        var result = repository.GetSessionByToken(session.Token);

        Assert.IsNotNull(result);
        Assert.AreEqual(session.Token, result.Token);
    }

    [TestMethod]
    public void AddUser_WhenValidUser_ReturnsId()
    {
        var user = new User
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@email.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Client
        };

        var repository = new UserRepository(_context!);
        var result = repository.AddUser(user);

        Assert.IsTrue(result > 0);
    }

    [TestMethod]
    public void GetUsers_WhenCalled_ReturnsUsers()
    {
        var user = new User
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Administrative
        };

        _context!.Users.Add(user);
        _context.SaveChanges();

        var repository = new UserRepository(_context);
        var result = repository.GetUsers(null, null);

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetUsers_WhenFilterByName_ReturnsFilteredUsers()
    {
        _context!.Users.AddRange(
            new User { Name = "Juan", LastName = "Perez", Email = "juan@test.com", Phone = "+59899123456", Password = "Contrasena1!@#$%", Role = UserRole.Administrative },
            new User { Name = "Maria", LastName = "Lopez", Email = "maria@test.com", Phone = "+59899123457", Password = "Contrasena2!@#$%", Role = UserRole.Administrative });
        _context.SaveChanges();

        var repository = new UserRepository(_context);
        var result = repository.GetUsers("Juan", null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Juan", result[0].Name);
    }

    [TestMethod]
    public void GetUsers_WhenFilterByLastName_ReturnsFilteredUsers()
    {
        _context!.Users.AddRange(
            new User { Name = "Juan", LastName = "Perez", Email = "juan@test.com", Phone = "+59899123456", Password = "Contrasena1!@#$%", Role = UserRole.Administrative },
            new User { Name = "Maria", LastName = "Lopez", Email = "maria@test.com", Phone = "+59899123457", Password = "Contrasena4!@#$%", Role = UserRole.Administrative });
        _context.SaveChanges();

        var repository = new UserRepository(_context);
        var result = repository.GetUsers(null, "Perez");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Perez", result[0].LastName);
    }

    [TestMethod]
    public void UpdateUser_WhenValidUser_ReturnsUpdatedUser()
    {
        var user = new User
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Administrative
        };

        _context!.Users.Add(user);
        _context.SaveChanges();

        user.Name = "NuevoNombre";

        var repository = new UserRepository(_context);
        var result = repository.UpdateUser(user);

        Assert.AreEqual("NuevoNombre", result.Name);
    }

    [TestMethod]
    public void GI()
    {
        var user = new User
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "juan@test.com",
            Phone = "+59899123456",
            Password = "Contrasena1!@#$%",
            Role = UserRole.Administrative
        };

        _context!.Users.Add(user);
        _context.SaveChanges();

        var repository = new UserRepository(_context);
        repository.DeleteUser(user.Id);

        var result = _context.Users.FirstOrDefault(u => u.Id == user.Id);
        Assert.IsNull(result);
    }
}
