using DarkKitchen.DataAccess.Context;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test.Repositories;

[TestClass]
public sealed class SessionRepositoryTest
{
    private SqliteConnection? _connection;

    private const string Val59891234567 = "+59891234567";
    private const string Contrasena1 = "Contrasena1@%#!";
    private const string DataSourceMemory = "Data Source=:memory:";
    private const string Juan = "Juan";
    private const string Perez = "Perez";
    private const string JuanTestCom = "juan@test.com";
    private DarkKitchenContext? _context;

    [TestInitialize]
    public void Initialize()
    {
        _connection = new SqliteConnection(DataSourceMemory);
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
    public void GetSessionByToken_ExistingToken_ReturnsSessionWithUser()
    {
        var user = new User
        {
            Name = Juan,
            LastName = Perez,
            Email = JuanTestCom,
            Password = Contrasena1,
            Phone = Val59891234567
        };

        var session = new Session { User = user };

        _context!.Sessions.Add(session);
        _context.SaveChanges();

        var repository = new SessionRepository(_context);
        var result = repository.GetSessionByToken(session.Token);

        Assert.IsNotNull(result);
        Assert.AreEqual(session.Token, result.Token);
        Assert.IsNotNull(result.User);
        Assert.AreEqual(user.Email, result.User.Email);
    }
}
