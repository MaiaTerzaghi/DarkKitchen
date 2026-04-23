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

    [TestMethod]
    public void GetActivePromotions_WhenProductLineFilter_ReturnsFilteredPromotions()
    {
        var promotion1 = new Promotion
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 12, 30),
            ProductLine = "Minutas"
        };

        var promotion2 = new Promotion
        {
            Name = "Semana Turismo",
            DiscountPercentage = 15,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 12, 30),
            ProductLine = "Desayunos"
        };

        _context!.Promotions.Add(promotion1);
        _context.Promotions.Add(promotion2);
        _context.SaveChanges();

        var repository = new PromotionRepository(_context);
        var result = repository.GetActivePromotions(null, "Minutas", null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Black Friday", result[0].Name);
    }

    [TestMethod]
    public void GetActivePromotions_WhenProductFilter_ReturnsFilteredPromotions()
    {
        var product1 = new Product
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana con tomate fresco",
            CommercialLine = "Minutas",
            Category = "Fritos",
            Price = 100.0
        };

        var promotion1 = new Promotion
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 12, 30),
            ProductLine = "Minutas",
            Products = [product1]
        };

        var promotion2 = new Promotion
        {
            Name = "Semana Turismo",
            DiscountPercentage = 15,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 12, 30),
            ProductLine = "Desayunos"
        };

        _context!.Promotions.Add(promotion1);
        _context.Promotions.Add(promotion2);
        _context.SaveChanges();

        var repository = new PromotionRepository(_context);
        var result = repository.GetActivePromotions(null, null, "Pizza Napolitana");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Black Friday", result[0].Name);
    }

    [TestMethod]
    public void GetPromotionWithProducts_ExistingPromotion_ReturnsPromotionWithProducts()
    {
        var product = new Product
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana",
            CommercialLine = "Minutas",
            Category = "Fritos",
            Price = 100.0
        };

        var promotion = new Promotion
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 1, 30),
            Products = [product]
        };

        _context!.Promotions.Add(promotion);
        _context.SaveChanges();

        var repository = new PromotionRepository(_context);
        var result = repository.GetPromotionWithProducts(promotion.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Products.Count);
    }

    [TestMethod]
    public void ProductHasActivePromotion_WhenProductHasActivePromotion_ReturnsTrue()
    {
        var product = new Product
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana",
            CommercialLine = "Minutas",
            Category = "Fritos",
            Price = 100.0
        };

        var promotion = new Promotion
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            ValidFrom = DateTime.Today.AddDays(-1),
            ValidTo = DateTime.Today.AddDays(1),
            Products = [product]
        };

        _context!.Promotions.Add(promotion);
        _context.SaveChanges();

        var repository = new PromotionRepository(_context);
        var result = repository.ProductHasActivePromotion(product.Id);

        Assert.IsTrue(result);
    }
}
