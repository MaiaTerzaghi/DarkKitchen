using DarkKitchen.DataAccess.Context;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test.Repositories;

[TestClass]
public sealed class ProductRepositoryTest
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

    [TestMethod]
    public void GetAll_WhenNoFilters_ReturnsAllProducts()
    {
        _context!.Products.Add(new Product { Code = "P001", Name = "Pizza", Category = "Fritos", CommercialLine = "Minutas", Description = "Rica pizza", Price = 100.0 });
        _context.Products.Add(new Product { Code = "P002", Name = "Pasta", Category = "Pastas", CommercialLine = "Minutas", Description = "Rica pasta", Price = 80.0 });
        _context.SaveChanges();

        var repository = new ProductRepository(_context);
        var result = repository.GetAll(null, null, null);

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetAll_WhenFilterByName_ReturnsFilteredProducts()
    {
        _context!.Products.Add(new Product { Code = "P001", Name = "Pizza", Category = "Fritos", CommercialLine = "Minutas", Description = "Rica pizza", Price = 100.0 });
        _context.Products.Add(new Product { Code = "P002", Name = "Pasta", Category = "Pastas", CommercialLine = "Minutas", Description = "Rica pasta", Price = 80.0 });
        _context.SaveChanges();

        var repository = new ProductRepository(_context);
        var result = repository.GetAll("Pizza", null, null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Pizza", result[0].Name);
    }

    [TestMethod]
    public void GetAll_WhenFilterByCategory_ReturnsFilteredProducts()
    {
        _context!.Products.Add(new Product { Code = "P001", Name = "Pizza", Category = "Fritos", CommercialLine = "Minutas", Description = "Rica pizza", Price = 100.0 });
        _context.Products.Add(new Product { Code = "P002", Name = "Pasta", Category = "Pastas", CommercialLine = "Minutas", Description = "Rica pasta", Price = 80.0 });
        _context.SaveChanges();

        var repository = new ProductRepository(_context);
        var result = repository.GetAll(null, "Fritos", null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Pizza", result[0].Name);
    }

    [TestMethod]
    public void GetAll_WhenFilterByLine_ReturnsFilteredProducts()
    {
        _context!.Products.Add(new Product { Code = "P001", Name = "Pizza", Category = "Fritos", CommercialLine = "Minutas", Description = "Rica pizza", Price = 100.0 });
        _context.Products.Add(new Product { Code = "P002", Name = "Pasta", Category = "Pastas", CommercialLine = "Desayunos", Description = "Rica pasta", Price = 80.0 });
        _context.SaveChanges();

        var repository = new ProductRepository(_context);
        var result = repository.GetAll(null, null, "Minutas");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Pizza", result[0].Name);
    }

    [TestMethod]
    public void GetById_ExistingProduct_ReturnsProduct()
    {
        var product = new Product
        {
            Code = "P001",
            Name = "Pizza",
            Category = "Fritos",
            CommercialLine = "Minutas",
            Description = "Rica pizza",
            Price = 100.0
        };

        _context!.Products.Add(product);
        _context.SaveChanges();

        var repository = new ProductRepository(_context);
        var result = repository.GetById(product.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(product.Id, result.Id);
    }

    [TestMethod]
    public void ProductHasActivePromotion_WhenProductHasActivePromotion_ReturnsTrue()
    {
        var product = new Product
        {
            Code = "P001",
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
