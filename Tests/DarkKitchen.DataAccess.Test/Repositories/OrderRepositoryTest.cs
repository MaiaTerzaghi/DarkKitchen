using DarkKitchen.DataAccess.Context;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test.Repositories;

[TestClass]
public sealed class OrderRepositoryTest
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

        _context.Users.AddRange(
            new User
            {
                Id = 1,
                Name = "Juan",
                LastName = "Perez",
                Email = "client1@test.com",
                Phone = "+59899000000",
                Password = "hash",
                Role = UserRole.Client
            },
            new User
            {
                Id = 2,
                Name = "Ana",
                LastName = "Lopez",
                Email = "client2@test.com",
                Phone = "+59899000001",
                Password = "hash",
                Role = UserRole.Client
            });

        _context.ShippingTypes.Add(new ShippingType
        {
            Id = 1,
            Name = "Express",
            Cost = 50.0
        });

        _context.SaveChanges();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context?.Dispose();
        _connection?.Dispose();
    }

    [TestMethod]
    public void GetClientOrders_WhenClientHasOrders_ReturnsOrders()
    {
        var order = new Order
        {
            ClientId = 1,
            ShippingTypeId = 1,
            Status = OrderStatus.Pending,
            Street = "18 de Julio",
            DoorNumber = "1234",
            Items = []
        };

        _context!.Orders.Add(order);
        _context.SaveChanges();

        var repository = new OrderRepository(_context);
        var result = repository.GetClientOrders(1, null, null, null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(1, result[0].ClientId);
    }

    [TestMethod]
    public void GetOrders_FilterByDateRange_ReturnsOrdersWithinRange()
    {
        var order1 = new Order
        {
            ClientId = 1,
            ShippingTypeId = 1,
            Status = OrderStatus.Pending,
            Street = "18 de Julio",
            DoorNumber = "1234",
            Date = new DateTime(2026, 1, 10),
            Items = []
        };

        var order2 = new Order
        {
            ClientId = 2,
            ShippingTypeId = 1,
            Status = OrderStatus.Pending,
            Street = "Av. Italia",
            DoorNumber = "5678",
            Date = new DateTime(2026, 3, 10),
            Items = []
        };

        _context!.Orders.AddRange(order1, order2);
        _context.SaveChanges();

        var repository = new OrderRepository(_context);
        var result = repository.GetOrders(new DateTime(2026, 1, 1), new DateTime(2026, 1, 31), null, null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(order1.Id, result[0].Id);
    }

    [TestMethod]
    public void GetOrders_FilterByStreet_ReturnsOrdersMatchingStreet()
    {
        var order1 = new Order
        {
            ClientId = 1,
            ShippingTypeId = 1,
            Status = OrderStatus.Pending,
            Street = "18 de Julio",
            DoorNumber = "1234",
            Date = new DateTime(2026, 1, 10),
            Items = []
        };

        var order2 = new Order
        {
            ClientId = 2,
            ShippingTypeId = 1,
            Status = OrderStatus.Pending,
            Street = "Av. Italia",
            DoorNumber = "5678",
            Date = new DateTime(2026, 1, 15),
            Items = []
        };

        _context!.Orders.AddRange(order1, order2);
        _context.SaveChanges();

        var repository = new OrderRepository(_context);
        var result = repository.GetOrders(new DateTime(2026, 1, 1), new DateTime(2026, 1, 31), "18 de Julio", null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("18 de Julio", result[0].Street);
    }

    [TestMethod]
    public void GetOrders_FilterByStatus_ReturnsOrdersMatchingStatus()
    {
        var order1 = new Order
        {
            ClientId = 1,
            ShippingTypeId = 1,
            Status = OrderStatus.Pending,
            Street = "18 de Julio",
            DoorNumber = "1234",
            Date = new DateTime(2026, 1, 10),
            Items = []
        };

        var order2 = new Order
        {
            ClientId = 2,
            ShippingTypeId = 1,
            Status = OrderStatus.Delivered,
            Street = "Av. Italia",
            DoorNumber = "5678",
            Date = new DateTime(2026, 1, 15),
            Items = []
        };

        _context!.Orders.AddRange(order1, order2);
        _context.SaveChanges();

        var repository = new OrderRepository(_context);
        var result = repository.GetOrders(new DateTime(2026, 1, 1), new DateTime(2026, 1, 31), null, OrderStatus.Pending);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(OrderStatus.Pending, result[0].Status);
    }

    [TestMethod]
    public void GetById_ExistingOrder_ReturnsOrder()
    {
        var order = new Order
        {
            ClientId = 1,
            ShippingTypeId = 1,
            Status = OrderStatus.Pending,
            Street = "18 de Julio",
            DoorNumber = "1234",
            Items = []
        };

        _context!.Orders.Add(order);
        _context.SaveChanges();

        var repository = new OrderRepository(_context);
        var result = repository.GetOrderById(order.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(order.Id, result.Id);
        Assert.AreEqual(OrderStatus.Pending, result.Status);
    }

    [TestMethod]
    public void GetTopProducts_ValidRequest_ReturnsProductsInDateRange()
    {
        var product = new Product
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana",
            CommercialLine = "Minutas",
            Category = "Fritos",
            Price = 100.0,
            Images = "/9j/2Q=="
        };

        var orderInRange = new Order
        {
            ClientId = 1,
            ShippingTypeId = 1,
            Status = OrderStatus.Delivered,
            Street = "18 de Julio",
            DoorNumber = "1234",
            Date = new DateTime(2026, 1, 10),
            Items = [new OrderItem { Product = product, Quantity = 5 }]
        };

        var orderOutOfRange = new Order
        {
            ClientId = 1,
            ShippingTypeId = 1,
            Status = OrderStatus.Delivered,
            Street = "18 de Julio",
            DoorNumber = "1234",
            Date = new DateTime(2026, 3, 10),
            Items = [new OrderItem { Product = product, Quantity = 10 }]
        };

        _context!.Orders.AddRange(orderInRange, orderOutOfRange);
        _context.SaveChanges();

        var repository = new OrderRepository(_context!);
        var result = repository.GetTopProducts(
            o => o.Date >= new DateTime(2026, 1, 1) && o.Date <= new DateTime(2026, 1, 31),
            5);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Pizza Napolitana", result[0].Product.Name);
        Assert.AreEqual(5, result[0].Quantity);
    }

    [TestMethod]
    public void GetTopProducts_ValidRequest_ReturnsProductsOrderedByQuantity()
    {
        var product1 = new Product
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana",
            CommercialLine = "Minutas",
            Category = "Fritos",
            Price = 100.0,
            Images = "/9j/2Q=="
        };

        var product2 = new Product
        {
            Code = "P0002",
            Name = "Pasta Bolognesa",
            Description = "Rica pasta bolognesa",
            CommercialLine = "Minutas",
            Category = "Pastas",
            Price = 80.0,
            Images = "/9j/2Q=="
        };

        var order = new Order
        {
            ClientId = 1,
            ShippingTypeId = 1,
            Status = OrderStatus.Delivered,
            Street = "18 de Julio",
            DoorNumber = "1234",
            Date = new DateTime(2026, 1, 10),
            Items =
            [
                new OrderItem { Product = product1, Quantity = 5 },
                new OrderItem { Product = product2, Quantity = 10 }
            ]
        };

        _context!.Orders.Add(order);
        _context.SaveChanges();

        var repository = new OrderRepository(_context!);
        var result = repository.GetTopProducts(
            o => o.Date >= new DateTime(2026, 1, 1) && o.Date <= new DateTime(2026, 1, 31),
            5);

        Assert.AreEqual("Pasta Bolognesa", result[0].Product.Name);
        Assert.AreEqual("Pizza Napolitana", result[1].Product.Name);
    }

    [TestMethod]
    public void GetTopProducts_ValidRequest_ReturnsLimitedResults()
    {
        for(var i = 1; i <= 6; i++)
        {
            var product = new Product
            {
                Code = $"P000{i}",
                Name = $"Producto {i} largo nombre",
                Description = "Descripcion larga del producto",
                CommercialLine = "Minutas",
                Category = "Fritos",
                Price = 100.0,
                Images = "/9j/2Q=="
            };

            var order = new Order
            {
                ClientId = 1,
                ShippingTypeId = 1,
                Status = OrderStatus.Delivered,
                Street = "18 de Julio",
                DoorNumber = "1234",
                Date = new DateTime(2026, 1, 10),
                Items = [new OrderItem { Product = product, Quantity = i }]
            };

            _context!.Orders.Add(order);
        }

        _context!.SaveChanges();

        var repository = new OrderRepository(_context!);
        var result = repository.GetTopProducts(
            o => o.Date >= new DateTime(2026, 1, 1) && o.Date <= new DateTime(2026, 1, 31),
            5);

        Assert.AreEqual(5, result.Count);
    }

    [TestMethod]
    public void GetSalesReport_ValidRequest_ReturnsSalesGroupedByYearMonthAndClient()
    {
        var product = new Product
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana",
            CommercialLine = "Minutas",
            Category = "Fritos",
            Price = 100.0,
            Images = "/9j/2Q=="
        };

        var order1 = new Order
        {
            ClientId = 1,
            ShippingTypeId = 1,
            Status = OrderStatus.Delivered,
            Street = "18 de Julio",
            DoorNumber = "1234",
            Date = new DateTime(2026, 1, 10),
            Items = [new OrderItem { Product = product, Quantity = 1 }]
        };

        var order2 = new Order
        {
            ClientId = 2,
            ShippingTypeId = 1,
            Status = OrderStatus.Delivered,
            Street = "18 de Julio",
            DoorNumber = "1234",
            Date = new DateTime(2026, 1, 15),
            Items = [new OrderItem { Product = product, Quantity = 1 }]
        };

        _context!.Orders.AddRange(order1, order2);
        _context.SaveChanges();

        var repository = new OrderRepository(_context!);
        var result = repository.GetSalesReport(1, 20);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(2026, result[0].Year);
        Assert.AreEqual(1, result[0].Month);
        Assert.AreEqual(1, result[0].ClientId);
    }

    [TestMethod]
    public void GetSalesReport_WithPagination_ReturnsCorrectPage()
    {
        for(var i = 1; i <= 25; i++)
        {
            var product = new Product
            {
                Code = $"P{i:D4}",
                Name = $"Producto {i} largo nombre",
                Description = "Descripcion larga del producto para cumplir validacion",
                CommercialLine = "Minutas",
                Category = "Fritos",
                Price = 100.0,
                Images = "/9j/2Q=="
            };

            _context!.Orders.Add(new Order
            {
                ClientId = 1,
                ShippingTypeId = 1,
                Status = OrderStatus.Delivered,
                Street = "18 de Julio",
                DoorNumber = "1234",
                Date = new DateTime(2000 + i, 1, 1),
                Items = [new OrderItem { Product = product, Quantity = 1 }]
            });
        }

        _context!.SaveChanges();

        var repository = new OrderRepository(_context!);
        var result = repository.GetSalesReport(2, 20);

        Assert.AreEqual(5, result.Count);
    }

    [TestMethod]
    public void GetSalesReport_ValidRequest_ReturnsTotalCorrectly()
    {
        var product = new Product
        {
            Code = "P0001",
            Name = "Pizza Napolitana",
            Description = "Rica pizza napolitana",
            CommercialLine = "Minutas",
            Category = "Fritos",
            Price = 100.0,
            Images = "/9j/2Q=="
        };

        var order = new Order
        {
            ClientId = 1,
            ShippingTypeId = 1,
            Status = OrderStatus.Delivered,
            Street = "18 de Julio",
            DoorNumber = "1234",
            Date = new DateTime(2026, 1, 10),
            Total = 500.0,
            Items = [new OrderItem { Product = product, Quantity = 3 }]
        };

        _context!.Orders.Add(order);
        _context.SaveChanges();

        var repository = new OrderRepository(_context!);
        var result = repository.GetSalesReport(1, 20);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(500.0, result[0].Total);
    }
}
