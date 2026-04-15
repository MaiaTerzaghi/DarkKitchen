using DarkKitchen.DataAccess.Context;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test.Repositories;

[TestClass]
public sealed class RepositoryTest
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
    public void Add_ValidEntity_ReturnsSavedEntityWithId()
    {
        var promotion = new Promotion
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30)
        };

        var repository = new Repository<Promotion>(_context!);
        var result = repository.Add(promotion);

        Assert.IsNotNull(result);
        Assert.AreNotEqual(0, result.Id);
        Assert.AreEqual("Black Friday", result.Name);
    }

    [TestMethod]
    public void Get_ExistingEntity_ReturnsEntity()
    {
        var promotion = new Promotion
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30)
        };

        _context!.Promotions.Add(promotion);
        _context.SaveChanges();

        var repository = new Repository<Promotion>(_context);
        var result = repository.Get(p => p.Id == promotion.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(promotion.Id, result.Id);
    }

    [TestMethod]
    public void Update_ExistingEntity_ReturnsUpdatedEntity()
    {
        var promotion = new Promotion
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30)
        };

        _context!.Promotions.Add(promotion);
        _context.SaveChanges();

        promotion.Name = "Black Friday Updated";

        var repository = new Repository<Promotion>(_context);
        var result = repository.Update(promotion);

        Assert.IsNotNull(result);
        Assert.AreEqual("Black Friday Updated", result.Name);
    }
}
