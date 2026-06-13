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

    private const string BlackFriday = "Black Friday";
    private const string Minutas = "Minutas";
    private const string PizzaNapolitana = "Pizza Napolitana";
    private const string Fritos = "Fritos";
    private const string P0001 = "P0001";
    private const string Desayunos = "Desayunos";
    private const string RicaPizzaNapolitana = "Rica pizza napolitana";
    private const string SemanaTurismo = "Semana Turismo";
    private const string DataSourceMemory = "Data Source=:memory:";
    private const string RicaPizzaNapolitanaConTomateFr = "Rica pizza napolitana con tomate fresco";
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
    public void GetActivePromotions_WhenPromotionIsActive_ReturnsPromotion()
    {
        var promotion = new Promotion
        {
            Name = BlackFriday,
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 12, 30)
        };

        _context!.Promotions.Add(promotion);
        _context.SaveChanges();

        var repository = new PromotionRepository(_context);
        var (items, totalCount) = repository.GetActivePromotions(new DateTime(2026, 4, 1), null, null);

        Assert.IsNotNull(items);
        Assert.AreEqual(1, totalCount);
        Assert.AreEqual(1, items.Count);
    }

    [TestMethod]
    public void GetActivePromotions_WhenNoDateFilter_ReturnsAllPromotions()
    {
        var promotion = new Promotion
        {
            Name = BlackFriday,
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 12, 30)
        };

        _context!.Promotions.Add(promotion);
        _context.SaveChanges();

        var repository = new PromotionRepository(_context);
        var (items, totalCount) = repository.GetActivePromotions(null, null, null);

        Assert.IsNotNull(items);
        Assert.AreEqual(1, totalCount);
        Assert.AreEqual(1, items.Count);
    }

    [TestMethod]
    public void GetActivePromotions_WhenProductLineFilter_ReturnsFilteredPromotions()
    {
        var promotion1 = new Promotion
        {
            Name = BlackFriday,
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 12, 30),
            ProductLine = Minutas
        };

        var promotion2 = new Promotion
        {
            Name = SemanaTurismo,
            DiscountPercentage = 15,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 12, 30),
            ProductLine = Desayunos
        };

        _context!.Promotions.Add(promotion1);
        _context.Promotions.Add(promotion2);
        _context.SaveChanges();

        var repository = new PromotionRepository(_context);
        var (items, totalCount) = repository.GetActivePromotions(null, Minutas, null);

        Assert.AreEqual(1, totalCount);
        Assert.AreEqual(1, items.Count);
        Assert.AreEqual(BlackFriday, items[0].Name);
    }

    [TestMethod]
    public void GetActivePromotions_WhenProductFilter_ReturnsFilteredPromotions()
    {
        var product1 = new Product
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitanaConTomateFr,
            CommercialLine = Minutas,
            Category = Fritos,
            Price = 100.0
        };

        var promotion1 = new Promotion
        {
            Name = BlackFriday,
            DiscountPercentage = 10,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 12, 30),
            ProductLine = Minutas,
            Products = [product1]
        };

        var promotion2 = new Promotion
        {
            Name = SemanaTurismo,
            DiscountPercentage = 15,
            ValidFrom = new DateTime(2026, 1, 25),
            ValidTo = new DateTime(2026, 12, 30),
            ProductLine = Desayunos
        };

        _context!.Promotions.Add(promotion1);
        _context.Promotions.Add(promotion2);
        _context.SaveChanges();

        var repository = new PromotionRepository(_context);
        var (items, totalCount) = repository.GetActivePromotions(null, null, PizzaNapolitana);

        Assert.AreEqual(1, totalCount);
        Assert.AreEqual(1, items.Count);
        Assert.AreEqual(BlackFriday, items[0].Name);
    }

    [TestMethod]
    public void GetPromotionWithProducts_ExistingPromotion_ReturnsPromotionWithProducts()
    {
        var product = new Product
        {
            Code = P0001,
            Name = PizzaNapolitana,
            Description = RicaPizzaNapolitana,
            CommercialLine = Minutas,
            Category = Fritos,
            Price = 100.0
        };

        var promotion = new Promotion
        {
            Name = BlackFriday,
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
}
