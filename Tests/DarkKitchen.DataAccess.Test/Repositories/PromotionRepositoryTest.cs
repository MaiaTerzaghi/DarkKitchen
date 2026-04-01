using DarkKitchen.DataAccess.Context;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test.Repositories;

[TestClass]
public sealed class PromotionRepositoryTest
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
    public void GetActivePromotions_WhenPromotionIsActive_ReturnsPromotion()
    {
        var promotion = new Promotion
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 12, 30)
        };

        _context!.Promotions.Add(promotion);
        _context.SaveChanges();

        var repository = new PromotionRepository(_context);
        var result = repository.GetActivePromotions(new DateTime(2026, 4, 1), null, null);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetActivePromotions_WhenNoDateFilter_ReturnsAllPromotions()
    {
        var promotion = new Promotion
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 12, 30)
        };

        _context!.Promotions.Add(promotion);
        _context.SaveChanges();

        var repository = new PromotionRepository(_context);
        var result = repository.GetActivePromotions(null, null, null);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
    }
}
