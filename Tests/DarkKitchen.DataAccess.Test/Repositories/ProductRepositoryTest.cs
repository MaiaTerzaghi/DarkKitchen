using DarkKitchen.DataAccess.Context;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using DarkKitchen.DTOs.Args.In;
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
        _context!.Products.Add(new Product { Code = "P0001", Name = "Pizza Napolitana", Category = "Fritos", CommercialLine = "Minutas", Description = "Rica pizza napolitana con tomate fresco", Price = 100.0 });
        _context.Products.Add(new Product { Code = "P0002", Name = "Pasta bolognese", Category = "Pastas", CommercialLine = "Minutas", Description = "Rica pasta bolognese con carne fresca", Price = 80.0 });
        _context.SaveChanges();

        var repository = new ProductRepository(_context);
        var result = repository.GetAll(null, null, null);

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetAll_WhenFilterByName_ReturnsFilteredProducts()
    {
        _context!.Products.Add(new Product { Code = "P0001", Name = "Pizza Napolitana", Category = "Fritos", CommercialLine = "Minutas", Description = "Rica pizza napolitana con tomate fresco", Price = 100.0 });
        _context.Products.Add(new Product { Code = "P0002", Name = "Pasta bolognese", Category = "Pastas", CommercialLine = "Minutas", Description = "Rica pasta bolognese con carne fresca", Price = 80.0 });
        _context.SaveChanges();

        var repository = new ProductRepository(_context);
        var result = repository.GetAll("Pizza", null, null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Pizza Napolitana", result[0].Name);
    }

    [TestMethod]
    public void GetAll_WhenFilterByCategory_ReturnsFilteredProducts()
    {
        _context!.Products.Add(new Product { Code = "P0001", Name = "Pizza Napolitana", Category = "Fritos", CommercialLine = "Minutas", Description = "Rica pizza napolitana con tomate fresco", Price = 100.0 });
        _context.Products.Add(new Product { Code = "P0002", Name = "Pasta bolognese", Category = "Pastas", CommercialLine = "Minutas", Description = "Rica pasta bolognese con carne fresca", Price = 80.0 });
        _context.SaveChanges();

        var repository = new ProductRepository(_context);
        var result = repository.GetAll(null, "Fritos", null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Pizza Napolitana", result[0].Name);
    }

    [TestMethod]
    public void GetAll_WhenFilterByLine_ReturnsFilteredProducts()
    {
        _context!.Products.Add(new Product { Code = "P0001", Name = "Pizza Napolitana", Category = "Fritos", CommercialLine = "Minutas", Description = "Rica pizza napolitana con tomate fresco", Price = 100.0 });
        _context.Products.Add(new Product { Code = "P0002", Name = "Pasta bolognese", Category = "Pastas", CommercialLine = "Desayunos", Description = "Rica pasta bolognese con carne fresca", Price = 80.0 });
        _context.SaveChanges();

        var repository = new ProductRepository(_context);
        var result = repository.GetAll(null, null, "Minutas");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Pizza Napolitana", result[0].Name);
    }

    [TestMethod]
    public void GetById_ExistingProduct_ReturnsProduct()
    {
        var product = new Product
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Category = "Fritos",
            CommercialLine = "Minutas",
            Description = "Rica pizza napolitana con tomate fresco",
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
    public void Add_WhenValidProduct_ReturnsSavedProduct()
    {
        var product = new Product
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana con tomate y albahaca",
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "pizza.jpg"
        };

        var repository = new ProductRepository(_context!);
        var result = repository.Add(product);

        Assert.IsNotNull(result);
        Assert.AreNotEqual(0, result.Id);
        Assert.AreEqual("P0001", result.Code);
    }

    [TestMethod]
    public void Update_WhenProductExists_ReturnsUpdatedProduct()
    {
        var product = new Product
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana con tomate y albahaca",
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "pizza.jpg"
        };

        _context!.Products.Add(product);
        _context.SaveChanges();

        product.Name = "Pizza Cuatro Quesos";

        var repository = new ProductRepository(_context);
        var result = repository.Update(product);

        Assert.IsNotNull(result);
        Assert.AreEqual("Pizza Cuatro Quesos", result.Name);
    }

    [TestMethod]
    public void GetManage_WhenNoFilters_ReturnsAllProducts()
    {
        _context!.Products.Add(new Product
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana con tomate y albahaca",
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "pizza.jpg",
            IsActive = true
        });
        _context.SaveChanges();

        var repository = new ProductRepository(_context);
        var result = repository.GetManage(new GetProductsManageRequestDTO());

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetManage_WhenFilterByName_ReturnsFilteredProducts()
    {
        _context!.Products.Add(new Product
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana con tomate y albahaca",
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "pizza.jpg",
            IsActive = true
        });
        _context.Products.Add(new Product
        {
            Code = "P0002",
            Name = "Pasta Bolognese",
            Description = "Rica pasta bolognese con carne y tomate",
            Price = 80.0,
            CommercialLine = "Minutas",
            Category = "Pastas",
            Images = "pasta.jpg",
            IsActive = true
        });
        _context.SaveChanges();

        var repository = new ProductRepository(_context);
        var result = repository.GetManage(new GetProductsManageRequestDTO { Name = "Pizza" });

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Pizza Napolitana", result[0].Name);
    }

    [TestMethod]
    public void GetManage_WhenFilterByDescription_ReturnsFilteredProducts()
    {
        _context!.Products.Add(new Product
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana con tomate y albahaca",
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "pizza.jpg",
            IsActive = true
        });
        _context.Products.Add(new Product
        {
            Code = "P0002",
            Name = "Pasta Bolognese",
            Description = "Rica pasta bolognese con carne y tomate",
            Price = 80.0,
            CommercialLine = "Minutas",
            Category = "Pastas",
            Images = "pasta.jpg",
            IsActive = true
        });
        _context.SaveChanges();

        var repository = new ProductRepository(_context);
        var result = repository.GetManage(new GetProductsManageRequestDTO { Description = "napolitana" });

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Pizza Napolitana", result[0].Name);
    }

    [TestMethod]
    public void GetManage_WhenFilterByCategory_ReturnsFilteredProducts()
    {
        _context!.Products.Add(new Product
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana con tomate y albahaca",
            Price = 100.0,
            CommercialLine = "Minutas",
            Category = "Fritos",
            Images = "pizza.jpg",
            IsActive = true
        });
        _context.Products.Add(new Product
        {
            Code = "P0002",
            Name = "Pasta Bolognese",
            Description = "Rica pasta bolognese con carne y tomate",
            Price = 80.0,
            CommercialLine = "Minutas",
            Category = "Pastas",
            Images = "pasta.jpg",
            IsActive = true
        });
        _context.SaveChanges();

        var repository = new ProductRepository(_context);
        var result = repository.GetManage(new GetProductsManageRequestDTO { Category = "Fritos" });

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Pizza Napolitana", result[0].Name);
    }
}
