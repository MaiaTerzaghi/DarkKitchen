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
        _context!.Products.Add(new Product { Code = "P001", Name = "Pizza", Category = "Fritos", CommercialLine = "Minutas", Description = "Rica pizza", Price = 100 });
        _context.Products.Add(new Product { Code = "P002", Name = "Pasta", Category = "Pastas", CommercialLine = "Minutas", Description = "Rica pasta", Price = 80 });
        _context.SaveChanges();

        var repository = new ProductRepository(_context);
        var result = repository.GetAll(null, null, null);

        Assert.AreEqual(2, result.Count);
    }
}
